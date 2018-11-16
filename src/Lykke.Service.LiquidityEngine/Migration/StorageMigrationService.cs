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
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IMigrationOperation[] _migrationOperations;
        private readonly ILog _log;

        public StorageMigrationService(
            IVersionControlRepository versionControlRepository,
            IInstrumentRepository instrumentRepository,
            IMigrationOperation[] migrationOperations,
            ILogFactory logFactory)
        {
            _versionControlRepository = versionControlRepository;
            _instrumentRepository = instrumentRepository;
            _migrationOperations = migrationOperations;
            _log = logFactory.CreateLog(this);
        }

        public async Task MigrateStorageAsync()
        {
            await MigrateLevels();

            IReadOnlyList<IMigrationOperation> operations = _migrationOperations
                .OrderBy(o => o.ApplyToVersion)
                .ToArray();

            ValidateOperations(operations);

            SystemVersion systemVersion = await _versionControlRepository.GetAsync();

            if (systemVersion == null)
            {
                systemVersion = new SystemVersion { VersionNumber = 0 };
            }
            
            _log.InfoWithDetails("Read current storage version.", systemVersion);

            foreach (IMigrationOperation operation in operations)
            {
                if (systemVersion.VersionNumber == operation.ApplyToVersion)
                {
                    _log.InfoWithDetails($"Applying migration operation '{operation.GetType().FullName}'.", operation);

                    await operation.ApplyAsync();

                    _log.InfoWithDetails("Migration operation executed. Updating storage version.", operation);

                    systemVersion.VersionNumber = operation.UpdatedVersion;

                    await _versionControlRepository.InsertOrReplaceAsync(systemVersion);
                }
            }
        }

        /// <summary>
        /// Checks levels' identifier and fills them if they are empty.
        /// </summary>
        /// <returns></returns>
        private async Task MigrateLevels()
        {
            IReadOnlyCollection<Instrument> instruments = await _instrumentRepository.GetAllAsync();

            foreach (Instrument instrument in instruments)
            {
                if (instrument.Levels.Any(o => string.IsNullOrEmpty(o.Id)))
                {
                    _log.InfoWithDetails("Migrating instrument levels.", instrument);

                    IReadOnlyCollection<InstrumentLevel> levels = instrument.Levels
                        .Select(o => new InstrumentLevel
                        {
                            Id = !string.IsNullOrEmpty(o.Id) ? o.Id : Guid.NewGuid().ToString(),
                            Number = o.Number,
                            Markup = o.Markup,
                            Volume = o.Volume
                        })
                        .ToArray();

                    instrument.UpdateLevels(levels);

                    await _instrumentRepository.UpdateAsync(instrument);

                    _log.InfoWithDetails("Instrument updated.", instrument);
                }
            }
        }

        private static void ValidateOperations(IReadOnlyCollection<IMigrationOperation> migrationOperations)
        {
            if (migrationOperations.Count == 0)
            {
                return;
            }

            migrationOperations.Aggregate((prev, next) =>
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
        }
    }
}
