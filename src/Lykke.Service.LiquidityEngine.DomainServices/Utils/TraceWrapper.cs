using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;

namespace Lykke.Service.LiquidityEngine.DomainServices.Utils
{
    public static class TraceWrapper
    {
        public static async Task TraceExecutionTimeAsync(string operation, Func<Task> func, ILog log)
        {
            var sw = new Stopwatch();
            
            sw.Start();

            log.Info($"Begin operation: {operation}");

            try
            {
                await func();
            }
            finally
            {
                sw.Stop();
                log.Info($"End operation: {operation}", new {sw.ElapsedMilliseconds});
            }
        }
    }
}
