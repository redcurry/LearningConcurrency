using System;
using System.Threading;
using System.Threading.Tasks;
using Snippets.ConcurrentDataStructures;

namespace Snippets.ConsoleTest
{
    public class Program
    {
        static void Main(string[] args)
        {
//            LazyRun1();
//            LazyRun2();
            LazyRun3();

            Console.ReadLine();
        }

        private static void LazyRun1()
        {
            var lazyPerson = new Lazy<Person>(LazyThreadSafetyMode.None);

            var p1 = Task.Run(() => lazyPerson.Value);
            var p2 = Task.Run(() => lazyPerson.Value);

            Console.WriteLine(ReferenceEquals(p1.Result, p2.Result));
        }

        private static void LazyRun2()
        {
            var lazyPerson = new Lazy<Person>(LazyThreadSafetyMode.PublicationOnly);

            var p1 = Task.Run(() => lazyPerson.Value);
            var p2 = Task.Run(() => lazyPerson.Value);

            Console.WriteLine(ReferenceEquals(p1.Result, p2.Result));
        }

        private static void LazyRun3()
        {
            var lazyPerson = new Lazy<Person2>(() => new Person2("Andy"));

            Console.WriteLine(lazyPerson.Value);
        }
    }
}
