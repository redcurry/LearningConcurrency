using System.Linq;
using NUnit.Framework;

namespace Snippets.Tests.ParallelBasics
{
    [TestFixture]
    public class TestPlinq
    {
        [Test]
        public void SelectInParallel()
        {
            var values = Enumerable.Range(0, 100);

            var result = values.AsParallel().Select(x => x * 2).ToArray();

            // Because AsParallel() doesn't preserve the order,
            // we can only ensure that the value is in the result
            Assert.That(result, Contains.Item(99 * 2));
        }

        [Test]
        public void OrderedSelectInParallel()
        {
            var values = Enumerable.Range(0, 100);

            var result = values.AsParallel().AsOrdered().Select(x => x * 2).ToArray();

            // Order is preserved
            Assert.That(result[50], Is.EqualTo(50 * 2));
        }
    }
}