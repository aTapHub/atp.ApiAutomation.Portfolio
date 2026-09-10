using System;
using System.Threading;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Utils
{
    public class RateLimiter : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly Timer _refillTimer;
        private readonly int _capacity;

        public RateLimiter(int capacity, TimeSpan refillInterval)
        {
            _capacity = capacity;
            _semaphore = new SemaphoreSlim(capacity, capacity);
            _refillTimer = new Timer(Refill, null, refillInterval, refillInterval);
        }

        public Task WaitAsync(CancellationToken ct = default) => _semaphore.WaitAsync(ct);

        private void Refill(object? state)
        {
            var permitsToRelease = _capacity - _semaphore.CurrentCount;
            if (permitsToRelease > 0)
            {
                _semaphore.Release(permitsToRelease);
            }
        }

        public void Dispose()
        {
            _refillTimer.Dispose();
            _semaphore.Dispose();
        }
    }
}
