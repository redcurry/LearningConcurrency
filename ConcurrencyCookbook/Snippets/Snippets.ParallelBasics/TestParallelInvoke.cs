using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Snippets.ParallelBasics
{
    [TestFixture]
    public class TestParallelInvoke
    {
        [Test]
        public void ProcessArrayInParallel()
        {
            var max = 1000000;
            var numbers = Enumerable.Range(0, max).ToArray();
            var arePrimes = new bool[max];

            // Can send each Action individually, or as an IEnumerable,
            // and can send a CancellationToken
            Parallel.Invoke(
                () => ProcessPartialArray(numbers, arePrimes, 0, numbers.Length / 2),
                () => ProcessPartialArray(numbers, arePrimes, numbers.Length / 2, numbers.Length));

            Assert.That(arePrimes[13],  Is.EqualTo(true));
            Assert.That(arePrimes[100], Is.EqualTo(false));
            Assert.That(arePrimes[225], Is.EqualTo(false));
        }

        private void ProcessPartialArray(int[] array, bool[] arePrimes, int begin, int end)
        {
            for (int i = begin; i < end; i++)
                arePrimes[i] = IsPrime(array[i]);
        }

        private bool IsPrime(int n)
        {
            var sqrtN = Math.Sqrt(n);

            for (int i = 2; i <= sqrtN; i++)
            {
                if (n % i == 0)
                    return false;
            }

            return true;
        }
    }
}