using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System.Windows.Input;

namespace Ae.LineCalibrator.Interface
{
    public sealed class AppViewModel
    {
        public AppViewModel(IClassicDesktopStyleApplicationLifetime desktop)
        {
            SystemTrayCommand = new SimpleCommand(() =>
            {
                desktop.MainWindow.Show();
            });

            ExitCommand = new SimpleCommand(() =>
            {
                desktop.Shutdown();
            });
        }

        public ICommand SystemTrayCommand { get; }
        public ICommand ExitCommand { get; }
    }
}
