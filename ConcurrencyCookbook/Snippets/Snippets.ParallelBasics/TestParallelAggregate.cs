using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Snippets.ParallelBasics
{
    [TestFixture]
    public class TestParallelAggregate
    {
        [Test]
        public void ParallelSumWithLock()
        {
            var values = Enumerable.Range(0, 100);

            var sum = ParallelSum(values);

            Assert.That(sum, Is.EqualTo(99 * 100 / 2));
        }

        [Test]
        public void ParallelSumWithLinq()
        {
            var values = Enumerable.Range(0, 100);

            // LINQ has built-in support for many operations
            var sum = values.AsParallel().Sum();

            Assert.That(sum, Is.EqualTo(99 * 100 / 2));
        }

        [Test]
        public void ParallelSumWithAggregate()
        {
            var values = Enumerable.Range(0, 100);

            // Use Aggregate for custom operation
            var sum = values.AsParallel().Aggregate(
                seed: 0, func: (localSum, item) => localSum + item);

            Assert.That(sum, Is.EqualTo(99 * 100 / 2));
        }

        private int ParallelSum(IEnumerable<int> values)
        {
            var mutex = new object();
            var result = 0;
            Parallel.ForEach(
                source: values,
                localInit: () => 0,
                body: (item, state, localValue) => localValue + item,
                localFinally: localValue =>
                {
                    // Need to use lock here to synchronize results
                    lock (mutex)
                        result += localValue;
                });
            return result;
        }
    }
}