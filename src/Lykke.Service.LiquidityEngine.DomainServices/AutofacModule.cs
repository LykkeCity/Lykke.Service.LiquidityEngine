using Autofac;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.AssetPairLinks;
using Lykke.Service.LiquidityEngine.DomainServices.Balances;
using Lykke.Service.LiquidityEngine.DomainServices.Instruments;
using Lykke.Service.LiquidityEngine.DomainServices.Settings;

namespace Lykke.Service.LiquidityEngine.DomainServices
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly string _instanceName;
        private readonly string _walletId;

        public AutofacModule(
            string instanceName,
            string walletId)
        {
            _instanceName = instanceName;
            _walletId = walletId;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AssetPairLinkService>()
                .As<IAssetPairLinkService>()
                .SingleInstance();

            builder.RegisterType<InstrumentService>()
                .As<IInstrumentService>()
                .SingleInstance();

            builder.RegisterType<BalanceService>()
                .As<IBalanceService>()
                .SingleInstance();
            
            builder.RegisterType<SettingsService>()
                .As<ISettingsService>()
                .WithParameter(new NamedParameter("instanceName", _instanceName))
                .WithParameter(new NamedParameter("walletId", _walletId))
                .SingleInstance();
        }
    }
}
