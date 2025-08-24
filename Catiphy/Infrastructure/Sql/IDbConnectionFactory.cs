using System.Data;

namespace Catiphy.Infrastructure.Sql
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
