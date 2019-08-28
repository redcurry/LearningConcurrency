using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Snippets.ConcurrentDataStructures
{
    public class ConcurrentQueueTest
    {
        public void Test()
        {
            var queue = new ConcurrentQueue<string>();

            var files = new DirectoryInfo(@"C:\Data").GetFiles("*.csv", SearchOption.AllDirectories);
            foreach (var file in files)
                queue.Enqueue(file.FullName);

            var consumers = new Task[4];
            for (int i = 0; i < consumers.Length; i++)
                consumers[i] = Task.Run(() => Consume(queue));

            Task.WaitAll(consumers);
        }

        private void Consume(ConcurrentQueue<string> queue)
        {
            while (queue.TryDequeue(out string file))
                Console.WriteLine($"{Task.CurrentId}: Processing {file}");
        }
    }
}
