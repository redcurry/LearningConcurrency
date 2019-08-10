using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Snippets.Tests.AsyncBasics
{
    [TestFixture]
    public class TestRetry
    {
        [Test]
        public async Task ServiceSucceedsOnFirstTry()
        {
            var service = Substitute.For<ISomeService>();
            service.ExecuteAsync().Returns(x => TestHelpers.DelayResult(42, TimeSpan.FromSeconds(1)));

            var result = await UseServiceWithRetry(service);

            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public async Task ServiceSucceedsOnSecondTry()
        {
            var tries = 0;

            var service = Substitute.For<ISomeService>();
            service.ExecuteAsync().Returns(x =>
            {
                tries++;
                if (tries != 2) throw new Exception();  // Throw exception until tries == 2
                return TestHelpers.DelayResult(42, TimeSpan.FromSeconds(1));
            });

            var result = await UseServiceWithRetry(service);

            Assert.That(result, Is.EqualTo(42));
        }

        // Simple retry implementation; prefer to use library like Polly
        public async Task<int> UseServiceWithRetry(ISomeService service)
        {
            var nextDelay = TimeSpan.FromSeconds(1);
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    return await service.ExecuteAsync();
                }
                catch
                {
                    // Ignore exception, so try again
                }

                await Task.Delay(nextDelay);
                nextDelay = nextDelay + nextDelay;  // Double the next delay
            }

            // Try one last time, letting any exceptions to propagate
            return await service.ExecuteAsync();
        }
    }
}