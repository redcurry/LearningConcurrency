using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Snippets.Tests.AsyncBasics
{
    [TestFixture]
    public class TestWhenAll
    {
        [Test]
        public async Task ReturnsAllResults()
        {
            // All tasks must return the same type of value (int)
            var tasks = new List<Task<int>>();

            for (int i = 0; i < 100; i++)
                tasks.Add(TestHelpers.DelayResult(i, TimeSpan.FromSeconds(1)));

            // Returns an array of results from the executed tasks
            var results = await Task.WhenAll(tasks);

            Assert.That(results, Contains.Item(42));
        }

        [Test]
        public async Task ReturnsAllResultsUsingLinq()
        {
            var numbers = Enumerable.Range(0, 100);
            var tasks = numbers.Select(i => TestHelpers.DelayResult(i, TimeSpan.FromSeconds(1)));

            // Use "ToArray" to evaluate the IEnumerable and start the tasks
            var taskArray = tasks.ToArray();

            // Returns an array of results from the executed tasks
            var results = await Task.WhenAll(taskArray);

            Assert.That(results, Contains.Item(42));
        }

        [Test]
        public async Task ReturnsThreadIds()
        {
            // Just experiment and see how many threads are being used

            var tasks = new List<Task<int>>();

            for (int i = 0; i < 100; i++)
                tasks.Add(Task.Run(() => Thread.CurrentThread.ManagedThreadId));

            var results = await Task.WhenAll(tasks);

            Assert.That(results.Length, Is.EqualTo(100));
        }
    }
}