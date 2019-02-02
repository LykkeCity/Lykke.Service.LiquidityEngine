using System;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.LiquidityEngine.Tests
{
    [TestClass]
    public class RefitTest
    {
        [TestMethod]
        public async Task Test()
        {
            var generator = HttpClientGenerator.HttpClientGenerator
                .BuildForUrl("http://localhost:5000")
                //.BuildForUrl("http://liquidity-engine.finances.svc.cluster.local")
                .WithoutRetries()
                .WithoutCaching()
                .Create();

            var client = generator.Generate<IPnLStopLossEnginesApi>();

            try
            {
                await client.EnableAsync("46616c4a-0129-40a4-88d4-3f17b1ea1297");
            }
            catch (Exception exception)
            {
            }
        }
    }
}
