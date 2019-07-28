using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Snippets.AsyncBasics.Wpf
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cts;
        private Service _service;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            _service = new Service();

            // Update the progress bar upon progress report
            var progress = new Progress<int>(i => ProgressBar.Value = i);

            try
            {
                _cts = new CancellationTokenSource();
                await _service.ExecuteAsync(progress, _cts.Token);
            }
            catch (TaskCanceledException)
            {
                ProgressBar.Value = 0;
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }
    }

    public class Service
    {
        public async Task ExecuteAsync(IProgress<int> progress, CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100; i++)
            {
                progress?.Report(i);
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
            }
        }
    }
}
