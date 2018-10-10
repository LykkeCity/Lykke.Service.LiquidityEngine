using System;
using System.Collections.Generic;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Logging
{
    [UsedImplicitly]
    public class QuoteThresholdLogService : IQuoteThresholdLogService
    {
        private readonly ILog _log;
        private readonly object _sync = new object();
        private readonly Dictionary<string, DateTime> _logTime = new Dictionary<string, DateTime>();

        private readonly TimeSpan _logInterval = TimeSpan.FromMinutes(1);

        public QuoteThresholdLogService(ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
        }

        public void Error(Quote lastQuote, Quote currentQuote, decimal threshold)
        {
            lock (_sync)
            {
                DateTime logTime = DateTime.MinValue;

                if (_logTime.ContainsKey(lastQuote.AssetPair))
                    logTime = _logTime[lastQuote.AssetPair];

                if (logTime.Add(_logInterval) < DateTime.UtcNow)
                {
                    _logTime[lastQuote.AssetPair] = DateTime.UtcNow;

                    _log.ErrorWithDetails(null, "Invalid quote", new
                    {
                        lastQuote,
                        currentQuote,
                        threshold
                    });
                }
            }
        }
    }
}
