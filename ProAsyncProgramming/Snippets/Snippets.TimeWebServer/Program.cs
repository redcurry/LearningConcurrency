using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Snippets.TimeWebServer
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://+:9000/Time/");

            // If this throws an "Access is denied" exception,
            // exit and run Visual Studio as Administrator
            listener.Start();

            var requestQueue = new BlockingCollection<HttpListenerContext>(new ConcurrentQueue<HttpListenerContext>());
            var producer = Task.Run(() => Producer(requestQueue, listener));

            var consumers = new Task[4];
            for (int i = 0; i < consumers.Length; i++)
                consumers[i] = Task.Run(() => Consumer(requestQueue));

            Console.WriteLine("Listening...");

            // Instead of terminating with Console.ReadLine(),
            // which terminates threads forcefully,
            // wait for the producer and consumers to finish
            producer.Wait();
            Task.WaitAll(consumers);

            // Use this here after producer/consumer threads have been shut down
            Console.ReadLine();
        }

        private static void Producer(BlockingCollection<HttpListenerContext> requestQueue, HttpListener listener)
        {
            // listener.GetContext() will wait until there's a request
            while (true)
            {
                var ctx = listener.GetContext();
                if (ctx.Request.QueryString.AllKeys.Contains("stop"))
                    break;
                requestQueue.Add(ctx);
            }

            requestQueue.CompleteAdding();
            Console.WriteLine("Producer stopped");

            // Note: If you specify a capacity to the BlockingCollection,
            // the Add() method will block until it can add the item.
            // You can also use TryAdd(), which will not block but return false
            // if the item couldn't be added
        }

        // See Consumer2 for a more convenient way of consuming
        private static void Consumer(BlockingCollection<HttpListenerContext> requestQueue)
        {
            try
            {
                while (true)
                {
                    // requestQueue.Take() will wait until there's something
                    var ctx = requestQueue.Take();
                    Console.WriteLine(ctx.Request.Url);
                    Thread.Sleep(5000);

                    using (var writer = new StreamWriter(ctx.Response.OutputStream))
                        writer.WriteLine(DateTime.Now);
                }
            }
            // This exception is thrown when .CompleteAdding() is called on BlockingCollection
            catch (InvalidOperationException)
            { }

            Console.WriteLine($"{Task.CurrentId}: Stopped");
        }

        private static void Consumer2(BlockingCollection<HttpListenerContext> requestQueue)
        {
            // The foreach blocks until an item can be taken from the BlockingCollection
            foreach (var ctx in requestQueue.GetConsumingEnumerable())
            {
                Console.WriteLine(ctx.Request.Url);
                Thread.Sleep(5000);

                using (var writer = new StreamWriter(ctx.Response.OutputStream))
                    writer.WriteLine(DateTime.Now);
            }

            Console.WriteLine($"{Task.CurrentId}: Stopped");
        }
    }
}
