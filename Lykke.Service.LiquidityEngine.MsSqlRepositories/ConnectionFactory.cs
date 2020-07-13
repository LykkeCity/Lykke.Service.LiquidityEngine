namespace Lykke.Service.LiquidityEngine.MsSqlRepositories
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
    }
}
