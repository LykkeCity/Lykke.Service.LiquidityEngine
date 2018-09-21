using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lykke.Service.LiquidityEngine.Tests
{
    [TestClass]
    public class TestAssembly
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void AutoMapper_OK()
        {
            Assert.IsTrue(true);
        }
    }
}
