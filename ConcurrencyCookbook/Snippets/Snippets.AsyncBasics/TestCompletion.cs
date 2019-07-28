using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace Snippets.AsyncBasics
{
    [TestFixture]
    public class TestCompletion
    {
        [Test]
        public void TaskCompletesByThrowingException()
        {
            // Useful when mocking async services that need to throw an exception

            var service = Substitute.For<ISomeService>();
            service.ExecuteAsync().Returns(x => Task.FromException<int>(new NotImplementedException()));

            Assert.That(async () => await service.ExecuteAsync(), Throws.InstanceOf<NotImplementedException>());
        }

        [Test]
        public void TaskCompletesFromCancellation()
        {
            // Useful for mocking a service that may (or may not) cancel its task

            var service = Substitute.For<ISomeService>();
            service.ExecuteWithCancellationAsync(Arg.Any<CancellationToken>()).Returns(x =>
            {
                var cancellationToken = x.Arg<CancellationToken>();  // Get substituted method parameter
                return cancellationToken.IsCancellationRequested
                    ? Task.FromCanceled<int>(cancellationToken)
                    : Task.FromResult(42);
            });

            // Test that service throws TaskCanceledException if the cancellation token is cancelled
            var cancelResult = service.ExecuteWithCancellationAsync(new CancellationToken(true));
            Assert.That(async () => await cancelResult, Throws.InstanceOf<TaskCanceledException>());

            // Test that service returns result if the cancellation token is not cancelled
            var normalResult = service.ExecuteWithCancellationAsync(new CancellationToken(false));
            Assert.That(async () => await normalResult, Is.EqualTo(42));
        }
    }

    // Implement an asynchronous service using synchronous code
    // Note: This class is not used in any tests, just shows how to implement it
    public class SynchronousService : ISomeService
    {
        public Task<int> ExecuteAsync()
        {
            try
            {
                var computation = 42;  // Synchronous code
                return Task.FromResult(computation);
            }
            catch (Exception e)
            {
                // Return the exception as a task
                return Task.FromException<int>(e);
            }
        }

        public Task<int> ExecuteWithCancellationAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}