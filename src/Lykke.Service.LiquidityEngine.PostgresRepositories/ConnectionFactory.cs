using System.Threading.Tasks;
using Npgsql;

namespace Lykke.Service.LiquidityEngine.PostgresRepositories
{
    public class ConnectionFactory
    {
        private readonly string _connectionString;

        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal DataContext CreateDataContext()
        {
            return new DataContext(_connectionString);
        }

        internal async Task<NpgsqlConnection> CreateNpgsqlConnection()
        {
            var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            return conn;
        }
    }
}
