using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Snippets.AsyncBasics
{
    [TestFixture]
    public class TestWhenAny
    {
        [Test]
        public async Task ReturnQuickestResult()
        {
            var random = new Random();
            var randomTimesInMs = Enumerable.Range(0, 100).Select(x => random.Next(100, 1000)).ToArray();

            // Create tasks that are delayed by the random time,
            // and whose integer result is that random time itself
            var tasks = randomTimesInMs.Select(t => TestHelpers.DelayResult(t, TimeSpan.FromMilliseconds(t)));

            // Start tasks
            var taskArray = tasks.ToArray();

            // Get the first task to complete
            var firstTask = await Task.WhenAny(taskArray);

            // Retrieve the task's value
            // Note: Consider cancelling other tasks if they're resource-intensive because they're still running
            var result = await firstTask;

            // The result should be the the minimum time (because it's the quickest to finish)
            // Note: However, tests show this isn't always true: sometimes the result
            // is close to the minimum, but not exact; probably due to overhead of starting tasks
            Assert.That(result, Is.EqualTo(randomTimesInMs.Min()));
        }

        [Test]
        public async Task TestTaskWithException()
        {
            var taskWithException = Task.FromException<int>(new Exception());
            var normalTask = TestHelpers.DelayResult(42, TimeSpan.FromSeconds(1));

            // The first task (with exception) won't cause an exception here
            var firstTask = await Task.WhenAny(taskWithException, normalTask);

            try
            {
                // Try to retrieve the task's value (but there's an exception)
                var result = await firstTask;

                // Should never get here because above should throw an exception
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.Pass();
            }
        }
    }
}