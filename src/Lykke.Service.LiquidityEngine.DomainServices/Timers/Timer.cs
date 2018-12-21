using System;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;

namespace Lykke.Service.LiquidityEngine.DomainServices.Timers
{
    public abstract class Timer
    {
        protected ILog Log;

        private bool _started;
        private Task _task;
        private CancellationTokenSource _cancellationTokenSource;

        public void Start()
        {
            if (_started)
                return;

            _started = true;

            _cancellationTokenSource = new CancellationTokenSource();

            _task = ExecuteAsync(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            if (!_started)
                return;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            _task?.GetAwaiter().GetResult();
            _task?.Dispose();

            _task = null;
            _cancellationTokenSource = null;

            OnStop();

            _started = false;
        }

        protected virtual void OnStop()
        {
        }

        protected abstract Task OnExecuteAsync(CancellationToken cancellation);

        protected abstract Task<TimeSpan> GetDelayAsync();

        private async Task ExecuteAsync(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                try
                {
                    await OnExecuteAsync(cancellation);
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }

                try
                {
                    TimeSpan delay = await GetDelayAsync();

                    if (delay == TimeSpan.Zero)
                        delay = TimeSpan.FromMilliseconds(1000);
                    
                    await Task.Delay(delay, cancellation);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }
    }
}
