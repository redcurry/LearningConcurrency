using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Snippets.AsyncBasics
{
    [TestFixture]
    public class TestExceptions
    {
        [Test]
        public async Task CatchException()
        {
            // Method is executed, and the exception is stored in the Task
            var task = ThrowExceptionAsync();

            try
            {
                // Await the task to re-throw the exception and catch it
                await task;
            }
            catch (InvalidOperationException)
            {
                Assert.Pass();
                return;
            }

            // Fail if no exception is caught
            Assert.Fail();
        }

        [Test]
        public async Task CatchMultipleExceptions()
        {
            var task1 = ThrowExceptionAsync();
            var task2 = ThrowExceptionAsync();

            // Execute all tasks, storing all exceptions in the composite Task
            var tasks = Task.WhenAll(task1, task2);

            try
            {
                // Await the task to re-throw the exception and catch it
                await tasks;
            }
            catch
            {
                // Note: catch catches the first exception thrown,
                // but the actual Task contains all the exceptions

                var allExceptions = tasks.Exception;

                Assert.That(allExceptions, Is.Not.Null);
                Assert.That(allExceptions.InnerExceptions.Count, Is.EqualTo(2));
                return;
            }

            // Fail if no exception is caught
            Assert.Fail();
        }

        public async Task ThrowExceptionAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            throw new InvalidOperationException();
        }
    }
}