using Catiphy.Application.Dtos;
using Catiphy.Infrastructure.Sql;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Catiphy.Infrastructure.Repositories;

public sealed class SearchHistoryRepository : ISearchHistoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    // Límite defensivo para paginación (evita traer páginas enormes por error)
    private const int MaxPageSize = 100;

    public SearchHistoryRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InsertAsync(
        DateTime searchedAtUtc,
        string factText,
        string threeWords,
        string gifUrl)
    {
        if (string.IsNullOrWhiteSpace(factText))
            throw new ArgumentException("FactText is required.", nameof(factText));
        if (string.IsNullOrWhiteSpace(threeWords))
            throw new ArgumentException("ThreeWords is required.", nameof(threeWords));
        if (string.IsNullOrWhiteSpace(gifUrl))
            throw new ArgumentException("GifUrl is required.", nameof(gifUrl));

        // Obtenemos la conexión concreta y abrimos de forma async
        using var conn = (SqlConnection)_connectionFactory.Create();
        await conn.OpenAsync().ConfigureAwait(false);

        const string sql = @"
                                INSERT INTO dbo.SearchHistory (SearchedAtUtc, FactText, ThreeWords, GifUrl)
                                VALUES (@searchedAtUtc, @factText, @threeWords, @gifUrl);";

        using var cmd = new SqlCommand(sql, conn)
        {
            CommandType = CommandType.Text,
            CommandTimeout = 30
        };

        cmd.Parameters.Add(new SqlParameter("@searchedAtUtc", SqlDbType.DateTime2) { Value = searchedAtUtc });
        cmd.Parameters.Add(new SqlParameter("@factText", SqlDbType.NVarChar, -1) { Value = factText });      // -1 = NVARCHAR(MAX)
        cmd.Parameters.Add(new SqlParameter("@threeWords", SqlDbType.NVarChar, 128) { Value = threeWords });
        cmd.Parameters.Add(new SqlParameter("@gifUrl", SqlDbType.NVarChar, 512) { Value = gifUrl });

        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }


    public async Task<(IEnumerable<SearchHistoryItemDtoDto> Items, int Total)> GetPagedAsync(
        int skip,
        int take)
    {
        if (skip < 0) skip = 0;
        if (take <= 0) take = 10;
        if (take > MaxPageSize) take = MaxPageSize;

        using var conn = (SqlConnection)_connectionFactory.Create();
        await conn.OpenAsync().ConfigureAwait(false);

        // 1) Total de filas
        int total;
        using (var countCmd = new SqlCommand("SELECT COUNT(1) FROM dbo.SearchHistory;", conn)
        {
            CommandType = CommandType.Text,
            CommandTimeout = 30
        })
        {
            var scalar = await countCmd.ExecuteScalarAsync().ConfigureAwait(false);
            total = scalar is int i ? i : Convert.ToInt32(scalar ?? 0);
        }

        // 2) Página ordenada por fecha DESC
        const string pageSql = @"
                                    SELECT SearchedAtUtc, FactText, ThreeWords, GifUrl
                                    FROM dbo.SearchHistory
                                    ORDER BY SearchedAtUtc DESC
                                    OFFSET @skip ROWS
                                    FETCH NEXT @take ROWS ONLY;";

        var items = new List<SearchHistoryItemDtoDto>(take);

        using (var pageCmd = new SqlCommand(pageSql, conn)
        {
            CommandType = CommandType.Text,
            CommandTimeout = 30
        })
        {
            pageCmd.Parameters.Add(new SqlParameter("@skip", SqlDbType.Int) { Value = skip });
            pageCmd.Parameters.Add(new SqlParameter("@take", SqlDbType.Int) { Value = take });

            using var reader = await pageCmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess)
                                            .ConfigureAwait(false);
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                // Ordinales en el mismo orden que el SELECT
                var searchedAtUtc = reader.GetDateTime(0);
                var factText = reader.GetString(1);
                var threeWords = reader.GetString(2);
                var gifUrl = reader.GetString(3);

                items.Add(new SearchHistoryItemDtoDto(searchedAtUtc, factText, threeWords, gifUrl));
            }
        }



        return (items, total);
    }

    public async Task<IReadOnlyList<SearchHistoryItemDtoDto>> GetAllAsync()
    {
        var list = new List<SearchHistoryItemDtoDto>();

        await using var conn = (SqlConnection)_connectionFactory.Create();
        await conn.OpenAsync();

        const string sql = @"
            SELECT SearchedAtUtc, FactText, ThreeWords, GifUrl
            FROM dbo.SearchHistory
            ORDER BY SearchedAtUtc DESC;";

        await using var cmd = new SqlCommand(sql, conn);

        await using var rd = await cmd.ExecuteReaderAsync();
        var ordDate = rd.GetOrdinal("SearchedAtUtc");
        var ordFact = rd.GetOrdinal("FactText");
        var ordThree = rd.GetOrdinal("ThreeWords");
        var ordGif = rd.GetOrdinal("GifUrl");

        while (await rd.ReadAsync())
        {
            var searchedAt = rd.GetDateTime(ordDate);
            var fact = rd.IsDBNull(ordFact) ? "" : rd.GetString(ordFact);
            var three = rd.IsDBNull(ordThree) ? "" : rd.GetString(ordThree);
            var gif = rd.IsDBNull(ordGif) ? "" : rd.GetString(ordGif);

            // Usa tu record/DTO existente:
            list.Add(new SearchHistoryItemDtoDto(searchedAt, fact, three, gif));
        }

        return list;
    }


}
