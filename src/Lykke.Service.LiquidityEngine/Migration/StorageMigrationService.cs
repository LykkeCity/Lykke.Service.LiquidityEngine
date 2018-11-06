using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.Migration
{
    [UsedImplicitly]
    public class StorageMigrationService
    {
        private readonly IVersionControlRepository _versionControlRepository;
        private readonly IMigrationOperation[] _migrationOperations;
        private readonly ILog _log;

        public StorageMigrationService(
            IVersionControlRepository versionControlRepository,
            IMigrationOperation[] migrationOperations,
            ILogFactory logFactory)
        {
            _versionControlRepository = versionControlRepository;
            _migrationOperations = migrationOperations;
            _log = logFactory.CreateLog(this);
        }

        public async Task MigrateStorageAsync()
        {
            IReadOnlyList<MigrationOperationDescription> operations = ValidateOperations(_migrationOperations);

            SystemVersion systemVersion = await _versionControlRepository.GetAsync();
            int versionNumber = systemVersion.VersionNumber;

            _log.InfoWithDetails("Read current storage version.", systemVersion);

            foreach (MigrationOperationDescription operation in operations)
            {
                if (versionNumber == operation.ApplyToVersion)
                {
                    _log.InfoWithDetails("Applying migration operation.", operation);

                    await operation.Operation.ApplyAsync();
                    versionNumber = operation.UpdatedVersion;

                    _log.InfoWithDetails("Migration operation executed", operation);
                }
            }

            if (versionNumber != systemVersion.VersionNumber)
            {
                systemVersion.VersionNumber = versionNumber;

                _log.InfoWithDetails("Updating storage version", systemVersion);

                await _versionControlRepository.UpdateAsync(systemVersion);
            }
        }

        private static IReadOnlyList<MigrationOperationDescription> ValidateOperations(IReadOnlyCollection<IMigrationOperation> migrationOperations)
        {
            if (migrationOperations.Count == 0)
            {
                return new MigrationOperationDescription[0];
            }

            IReadOnlyList<MigrationOperationDescription> operations = migrationOperations
                .Select(o =>
                {
                    StorageMigrationOperationAttribute migrationAttribute =
                        (StorageMigrationOperationAttribute)Attribute.GetCustomAttribute(o.GetType(),
                            typeof(StorageMigrationOperationAttribute));

                    if (migrationAttribute == null)
                    {
                        throw new InvalidOperationException("MigrationOperationAttribute is not found.");
                    }

                    return new MigrationOperationDescription
                    {
                        Name = migrationAttribute.Name,
                        ApplyToVersion = migrationAttribute.ApplyToVersion,
                        UpdatedVersion = migrationAttribute.UpdatedVersion,
                        Operation = o
                    };
                })
                .OrderBy(o => o.ApplyToVersion)
                .ToArray();

            operations.Aggregate((prev, next) =>
            {
                if (prev.ApplyToVersion == next.ApplyToVersion)
                {
                    throw new InvalidOperationException("Migration operations have same ApplyTo versions.");
                }

                if (prev.UpdatedVersion > next.ApplyToVersion)
                {
                    throw new InvalidOperationException("Migration operations have inconsistent updated versions.");
                }

                return next;
            });

            return operations;
        }

        private class MigrationOperationDescription
        {
            public string Name { get; set; }

            public int ApplyToVersion { get; set; }

            public int UpdatedVersion { get; set; }

            public IMigrationOperation Operation { get; set; }
        }
    }
}
