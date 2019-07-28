using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Snippets.AsyncBasics.ConfigureAwait
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Test that using ConfigureAwait, the context after running an async method
        // does not return to the original thread but continues on the new thread

        // Recommendation is to return to the original thread (typically the UI thread)
        // only when necessary; otherwise, it may block the UI thread too much

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var originalThreadId = Thread.CurrentThread.ManagedThreadId;
            Debug.WriteLine($"Original Thread = {originalThreadId}");

            // Set ConfigureAwait to true to return to the original thread
            await DoSomethingAsync().ConfigureAwait(false);

            var continuedThreadId = Thread.CurrentThread.ManagedThreadId;
            Debug.WriteLine($"Continued Thread = {continuedThreadId}");
        }

        private Task DoSomethingAsync()
        {
            return Task.Run(() =>
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                Debug.WriteLine($"Current Thread = {currentThreadId}");
            });
        }
    }
}
