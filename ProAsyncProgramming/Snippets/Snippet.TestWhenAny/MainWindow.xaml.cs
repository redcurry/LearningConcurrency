using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace Snippet.TestWhenAny
{
    public partial class MainWindow : Window
    {
        // To see how exceptions can be caught and displayed, make all websites invalid;
        // to see how the first website is downloaded, make at least one website valid
        private readonly string[] _websites =
        {
            "http://www.carlosjanderson.com", "http://www.microsoft-aoeu.com", "http://www.stackoverflow-aoeu.com"
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var websiteContents = await DownloadFirstWebsiteAsync();
                Contents.Text = websiteContents;
            }
            catch (AggregateException ex)
            {
                // Flatten AggregateException to unwrap all exceptions
                var exceptions = ex.Flatten().InnerExceptions;
                Contents.Text = string.Join("\n", exceptions.Select(x => x.Message));
            }
        }

        private async Task<string> DownloadFirstWebsiteAsync()
        {
            // Create a download task for each website
            var tasks = _websites
                .Select(website => new WebClient().DownloadStringTaskAsync(website))
                .ToList();

            var exceptions = new List<Exception>();

            while (tasks.Count > 0)
            {
                // Get the first task to complete
                var firstCompletedTask = await Task.WhenAny(tasks);

                // If it completed properly, return it -- we're done
                if (firstCompletedTask.Status == TaskStatus.RanToCompletion)
                    return await firstCompletedTask;

                // Otherwise, an exception occurred, so remove it from the
                // task list in order to try again in the next iteration
                tasks.Remove(firstCompletedTask);

                // Keep track of each exception, so that in case all tasks fail,
                // we'll throw an AggregateException with all the exceptions
                exceptions.Add(firstCompletedTask.Exception);
            }

            // All tasks failed
            throw new AggregateException(exceptions);
        }
    }
}
