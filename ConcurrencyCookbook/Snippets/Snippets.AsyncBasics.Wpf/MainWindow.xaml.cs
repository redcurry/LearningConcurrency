using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Snippets.AsyncBasics.Wpf
{
    public partial class MainWindow : Window
    {
        public const int TaskCount = 10;

        public readonly int[] TaskRange = Enumerable.Range(0, TaskCount).ToArray();

        private readonly ProgressBar[] _progressBars;

        private CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();

            _progressBars = TaskRange.Select(_ => new ProgressBar
            {
                Minimum = 0, Maximum = 99,
                Height = 24
            }).ToArray();

            ProgressBars.ItemsSource = _progressBars;
        }

        private async void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            _cts = new CancellationTokenSource();

            // Generate random delays in milliseconds
            var random = new Random();
            var delaysInMs = TaskRange.Select(_ => random.Next(10, 50)).ToArray();

            // Create services that take as long as each generated delay
            var services = TaskRange.Select(i => new Service(TimeSpan.FromMilliseconds(delaysInMs[i]))).ToArray();

            // Create tasks that execute each service
            var tasks = TaskRange.Select(i =>
            {
                var progress = new Progress<int>(p => _progressBars[i].Value = p);
                return services[i].ExecuteAsync(progress, _cts.Token);
            }).ToArray();


            // Create tasks that do something as soon as each completes
            var processingTasks = tasks.Select(async (t, i) =>
            {
                await t;
                _progressBars[i].Foreground = Brushes.Blue;
            }).ToArray();

            try
            {
                // Wait for all the tasks to complete
                await Task.WhenAll(processingTasks);
            }
            catch (TaskCanceledException)
            {
                // If any task was cancelled, reset all progress bars
                // Note: all tasks are cancelled because they share the cancellation token
                foreach (var progressBar in _progressBars)
                {
                    progressBar.Value = 0;
                    progressBar.Foreground = Brushes.Green;
                }
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Cancel all the tasks (because they all use the same token)
            _cts.Cancel();
        }
    }

    public class Service
    {
        private readonly TimeSpan _delay;

        public Service(TimeSpan delay)
        {
            _delay = delay;
        }

        public async Task ExecuteAsync(IProgress<int> progress, CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100; i++)
            {
                progress?.Report(i);
                await Task.Delay(_delay, cancellationToken);
            }
        }
    }
}
