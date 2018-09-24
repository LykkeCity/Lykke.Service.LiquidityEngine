using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Rest;
using Polly;

namespace Lykke.Service.LiquidityEngine.DomainServices.Utils
{
    public static class RetriesWrapper
    {
        private const int DefaultRetriesCount = 3;

        public static Task<T> RunWithRetriesAsync<T>(Func<Task<T>> method, int? retriesCount = null)
        {
            return Policy
                .Handle<Exception>(FilterRetryExceptions)
                .WaitAndRetryAsync(retriesCount ?? DefaultRetriesCount, GetRetryDelay)
                .ExecuteAsync(async () => await method());
        }

        private static bool FilterRetryExceptions(Exception exception)
        {
            if (exception is HttpOperationException httpOperationException)
            {
                return httpOperationException.Response.StatusCode == HttpStatusCode.InternalServerError ||
                       httpOperationException.Response.StatusCode == HttpStatusCode.BadGateway ||
                       httpOperationException.Response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                       httpOperationException.Response.StatusCode == HttpStatusCode.GatewayTimeout ||
                       httpOperationException.Response.StatusCode == HttpStatusCode.RequestTimeout;
            }

            return true;
        }

        private static TimeSpan GetRetryDelay(int retryAttempt)
        {
            return TimeSpan.FromMilliseconds(500 * retryAttempt);
        }
    }
}
