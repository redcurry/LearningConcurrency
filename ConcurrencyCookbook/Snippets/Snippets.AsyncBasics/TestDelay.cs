using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Snippets.AsyncBasics
{
    [TestFixture]
    public class TestDelay
    {
        [Test]
        public async Task ServiceSucceeds()
        {
            // In a real scenario, I wouldn't be testing that the service succeeds;
            // I'd be testing something else, which uses the service as part of the test

            var service = new FakeService();

            var result = await service.ExecuteAsync();

            Assert.That(result, Is.EqualTo(42));
        }
    }

    // Create a fake service that returns fixed results after a delay
    public class FakeService : ISomeService
    {
        public Task<int> ExecuteAsync()
        {
            // Succeeds asynchronously by returning the given result after 1 second
            return TestHelpers.DelayResult(42, TimeSpan.FromSeconds(1));
        }
    }
}
