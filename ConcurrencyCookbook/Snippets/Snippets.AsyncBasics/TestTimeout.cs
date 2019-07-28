using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Snippets.AsyncBasics
{
    // Tests a helper method that simulates cancelling a task after a timeout.
    // This is useful when the service operation doesn't provide cancellation itself.

    [TestFixture]
    public class TestTimeout
    {
        [Test]
        public async Task ServiceCompletes()
        {
            var service = Substitute.For<ISomeService>();
            service.ExecuteAsync().Returns(x => TestHelpers.DelayResult(42, TimeSpan.FromSeconds(1)));

            var result = await UseServiceWithTimeout(service);

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public async Task ServiceDoesNotComplete()
        {
            var service = Substitute.For<ISomeService>();
            service.ExecuteAsync().Returns(x => TestHelpers.DelayResult(42, TimeSpan.FromSeconds(10)));

            var result = await UseServiceWithTimeout(service);

            Assert.That(result, Is.EqualTo(-1));
        }

        // Used when the service operation doesn't support cancellation directly
        public async Task<int> UseServiceWithTimeout(ISomeService service)
        {
            // Cancellation token source that cancels itself after 3 seconds
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            // Create service and timeout tasks
            var serviceTask = service.ExecuteAsync();
            var timeoutTask = Task.Delay(Timeout.Infinite, cts.Token);

            // Run both tasks and get the one that completes first
            var completedTask = await Task.WhenAny(serviceTask, timeoutTask);

            // If it's the timeout task, return a sentinel value indicated failure
            if (completedTask == timeoutTask)
                return -1;

            // Otherwise, get the result from the service task and return it
            return await serviceTask;
        }
    }
}