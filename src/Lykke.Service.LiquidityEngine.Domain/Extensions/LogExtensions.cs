using System;
using System.Runtime.CompilerServices;
using Common;
using Common.Log;
using Lykke.Common.Log;

namespace Lykke.Service.LiquidityEngine.Domain.Extensions
{
    public static class LogExtensions
    {
        public static void InfoWithDetails(this ILog log,
            string message,
            object context,
            string prefix = "data",
            [CallerMemberName] string process = nameof(InfoWithDetails))
        {
            log.Info(message, GetContext(context, prefix), process: process);
        }

        public static void ErrorWithDetails(this ILog log,
            Exception exception,
            object context,
            string prefix = "data",
            [CallerMemberName] string process = nameof(ErrorWithDetails))
        {
            log.Error(exception, context: GetContext(context, prefix), process: process);
        }

        public static void ErrorWithDetails(this ILog log,
            Exception exception,
            string message,
            object context,
            string prefix = "data",
            [CallerMemberName] string process = nameof(ErrorWithDetails))
        {
            log.Error(exception, message, GetContext(context, prefix), process: process);
        }

        public static void WarningWithDetails(this ILog log,
            string message,
            object context,
            string prefix = "data",
            [CallerMemberName] string process = nameof(WarningWithDetails))
        {
            log.Warning(message, context: GetContext(context, prefix), process: process);
        }

        public static void WarningWithDetails(this ILog log,
            string message,
            Exception exception,
            object context,
            string prefix = "data",
            [CallerMemberName] string process = nameof(WarningWithDetails))
        {
            log.Warning(message, exception, GetContext(context, prefix), process: process);
        }
        
        private static string GetContext(object context, string prefix)
        {
            if (context is string)
                return (string) context;

            return $"{prefix}: {context.ToJson()}";
        }
    }
}
