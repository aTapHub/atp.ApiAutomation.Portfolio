using System;
using Polly;
using Polly.Wrap;

namespace atp.ApiAutomation.Framework.Utils
{
    public static class ResiliencePolicies
    {
        public static AsyncPolicy CreateDefaultPolicy()
        {
            var retry = Policy.Handle<TransientHttpException>()
                              .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

            var circuit = Policy.Handle<TransientHttpException>()
                                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));

            var timeout = Policy.TimeoutAsync(TimeSpan.FromSeconds(10));

            // circuit outer, retry inner, timeout innermost
            return Policy.WrapAsync(circuit, retry, timeout);
        }
    }
}



public class TransientHttpException : Exception
    {
        public TransientHttpException(string message) : base(message) { }
    }
