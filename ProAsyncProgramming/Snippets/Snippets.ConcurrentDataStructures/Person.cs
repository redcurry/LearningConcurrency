using System;
using System.Threading;

namespace Snippets.ConcurrentDataStructures
{
    public class Person
    {
        public Person()
        {
            Thread.Sleep(2000);
            Console.WriteLine("Created");
        }

        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString() =>
            $"Name: {Name}, Age: {Age}";
    }

    public class Person2
    {
        public Person2(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString() =>
            $"Name: {Name}, Age: {Age}";
    }
}
