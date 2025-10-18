using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Utils
{
    public class TokenBucket
    {

        private readonly Object _lock = new object();

        private readonly double _capacity; // max number of tokens

        private readonly double _refillRate; // tokens per second

        private double _tokens; // current number of tokens

        private DateTime _lastRefillTimestamp; // last time tokens were added

        public TokenBucket(double capacity, double refillRate)
        {
            _capacity = capacity;
            _refillRate = refillRate;
            _tokens = capacity; // start full
            _lastRefillTimestamp = DateTime.UtcNow;
        }

        public async Task AcquireToken() 
        {
            while (true) 
            {
                double waitTimeMs;

                lock (_lock) 
                {
                    RefillTokens();
                    if (_tokens >= 1) 
                    {
                        _tokens -= 1;
                        return; // token acquired
                    }

                    double needTokens = 1 - _tokens;
                    waitTimeMs  = (needTokens / _refillRate) * 1000; // convert to milliseconds

                    _lastRefillTimestamp = _lastRefillTimestamp.AddMilliseconds(waitTimeMs);
                }

                // Wait outside the lock to allow other threads to attempt acquisition
                await Task.Delay((int)waitTimeMs);
            }
        
        }


        private void RefillTokens()
        { 
            var now = DateTime.UtcNow;
            var timeElapsed = now - _lastRefillTimestamp;

            var tokensToAdd = timeElapsed.TotalSeconds * _refillRate;

            _tokens = Math.Min(_capacity, _tokens + tokensToAdd);
            _lastRefillTimestamp = now;


        }

    }
}
