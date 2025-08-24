using Catiphy.Infrastructure.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
            //conecta a la BD 
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
    }

    public IDbConnection Create()
    {
        var conn = new SqlConnection(_connectionString);
        return conn;
    }
}
