using DataLayer.EfCode;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace Benchmarking
{
    internal static class SqliteHelper
    {
        public static DbContextOptions<EfCoreContext> GetSqliteInMemoryOptions()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open();

            var builder = new DbContextOptionsBuilder<EfCoreContext>();
            builder.UseSqlite(connection);
            return builder.Options;
        }
    }
}
