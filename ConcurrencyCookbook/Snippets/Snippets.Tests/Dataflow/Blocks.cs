using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NUnit.Framework;

namespace Snippets.Tests.Dataflow
{
    [TestFixture]
    public class Blocks
    {
        [Test]
        public void LinkToTransformBlock()
        {
            var multiply = new TransformBlock<int, int>(item => item * 2);
            var subtract = new TransformBlock<int, int>(item => item - 2);

            multiply.LinkTo(subtract);

            // Send messages to the multiply block
            multiply.Post(10);
            multiply.Post(20);
            multiply.Post(50);

            // Read the output from the subtract block and test result
            Assert.That(subtract.Receive(), Is.EqualTo(18));
            Assert.That(subtract.Receive(), Is.EqualTo(38));
            Assert.That(subtract.Receive(), Is.EqualTo(98));
        }

        [Test]
        public async Task HandleException()
        {
            try
            {
                var multiply = new TransformBlock<int, int>(item =>
                {
                    // Block lambda map throw an exception
                    if (item == 1)
                        throw new InvalidOperationException();
                    return item * 2;
                });

                multiply.Post(1);
                multiply.Post(2);

                // Await block's Completion property to catch exception
                await multiply.Completion;
            }
            catch (InvalidOperationException)
            {
                // Expect to receive an exception
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task HandleExceptionPropagation()
        {
            try
            {
                var multiply = new TransformBlock<int, int>(item =>
                {
                    // Throw exception inside block lambda
                    if (item == 1)
                        throw new InvalidOperationException();
                    return item * 2;
                });

                var subtract = new TransformBlock<int, int>(item => item - 2);

                // Link with the option to PropagateCompletion
                multiply.LinkTo(subtract, new DataflowLinkOptions {PropagateCompletion = true});

                multiply.Post(1);

                // Await to catch any exceptions
                await subtract.Completion;
            }

            // Because the exception is propagated to linked blocks,
            // it's wrapped in an AggregateException
            catch (AggregateException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

    }
}
