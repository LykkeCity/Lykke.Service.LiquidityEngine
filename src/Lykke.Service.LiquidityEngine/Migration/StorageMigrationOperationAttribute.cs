using System;

namespace Lykke.Service.LiquidityEngine.Migration
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class StorageMigrationOperationAttribute : Attribute
    {
        public string Name { get; set; }

        public int ApplyToVersion { get; set; }

        public int UpdatedVersion { get; set; }
    }
}
