using Autofac;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.PostgresRepositories.Repositories;

namespace Lykke.Service.LiquidityEngine.PostgresRepositories
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

            builder.RegisterType<ExternalTradeRepository>()
                .Named<IExternalTradeRepository>("ExternalTradeRepositoryPostgres")
                .SingleInstance();

            builder.RegisterType<InternalTradeRepository>()
                .Named<IInternalTradeRepository>("InternalTradeRepositoryPostgres")
                .SingleInstance();

            builder.RegisterType<PositionRepository>()
                .Keyed<IPositionRepository>("PositionRepositoryPostgres")
                .SingleInstance();
        }
    }
}
