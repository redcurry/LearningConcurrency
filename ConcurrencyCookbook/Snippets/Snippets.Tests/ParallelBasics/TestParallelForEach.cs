using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Snippets.Tests.ParallelBasics
{
    [TestFixture]
    public class TestParallelForEach
    {
        [Test]
        public void TestCalculateSquareRoots()
        {
            var arePrimes = ArePrimesParallel(1000);

            Assert.That(arePrimes[13],  Is.EqualTo(true));
            Assert.That(arePrimes[100], Is.EqualTo(false));
            Assert.That(arePrimes[225], Is.EqualTo(false));
        }

        [Test]
        public void TestParallelIsFaster()
        {
            var then1 = DateTime.Now;
            var arePrimes1 = ArePrimesParallel(10000000);
            var now1 = DateTime.Now;
            var delta1 = now1 - then1;

            var then2 = DateTime.Now;
            var arePrimes2 = ArePrimesSequential(10000000);
            var now2 = DateTime.Now;
            var delta2 = now2 - then2;

            // Parallel takes about 2 seconds; sequential takes about 6 seconds
            Assert.That(now1 - then1, Is.LessThan(now2 - then2));
        }

        public bool[] ArePrimesParallel(int max)
        {
            var numbers = Enumerable.Range(0, max);
            var arePrimes = new bool[max];

            Parallel.ForEach(numbers, n => arePrimes[n] = IsPrime(n));

            return arePrimes;
        }

        public bool[] ArePrimesSequential(int max)
        {
            var numbers = Enumerable.Range(0, max);
            var arePrimes = new bool[max];

            foreach (var n in numbers)
                arePrimes[n] = IsPrime(n);

            return arePrimes;
        }

        public bool IsPrime(int n)
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
