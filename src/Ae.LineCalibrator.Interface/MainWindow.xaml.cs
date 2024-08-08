using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace Ae.LineCalibrator.Interface
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new LineCalibratorViewModel();
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            if (e.CloseReason == WindowCloseReason.WindowClosing)
            {
                Hide();
                e.Cancel = true;
            }
            else
            {
                ((IDisposable)DataContext).Dispose();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
