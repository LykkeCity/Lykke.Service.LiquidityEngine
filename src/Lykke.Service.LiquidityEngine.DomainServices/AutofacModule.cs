using Autofac;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.AssetPairLinks;
using Lykke.Service.LiquidityEngine.DomainServices.Instruments;

namespace Lykke.Service.LiquidityEngine.DomainServices
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        public AutofacModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AssetPairLinkService>()
                .As<IAssetPairLinkService>()
                .SingleInstance();
            
            builder.RegisterType<InstrumentService>()
                .As<IInstrumentService>()
                .SingleInstance();
        }
    }
}
