using Autofac;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.MsSqlRepositories.Repositories;

namespace Lykke.Service.LiquidityEngine.MsSqlRepositories
{
    public class AutofacModule : Module
    {
        private readonly string _connectionString;

        public AutofacModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConnectionFactory>()
                .AsSelf()
                .WithParameter(TypedParameter.From(_connectionString))
                .SingleInstance();

            builder.RegisterType<OrderBookUpdateReportRepository>()
                .As<IOrderBookUpdateReportRepository>()
                .SingleInstance();
        }
    }
}
