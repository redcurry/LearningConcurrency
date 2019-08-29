using System;
using System.Collections.Concurrent;
using System.IO;
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
            Console.ReadLine();
        }

        private static void Producer(BlockingCollection<HttpListenerContext> requestQueue, HttpListener listener)
        {
            // listener.GetContext() will wait until there's a request
            while (true)
                requestQueue.Add(listener.GetContext());

            // Note: If you specify a capacity to the BlockingCollection,
            // the Add() method will block until it can add the item.
            // You can also use TryAdd(), which will not block but return false
            // if the item couldn't be added
        }

        private static void Consumer(BlockingCollection<HttpListenerContext> requestQueue)
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
    }
}
