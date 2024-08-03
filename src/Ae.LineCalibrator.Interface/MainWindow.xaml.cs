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
            Hide();
            e.Cancel = true;
            ((IDisposable)DataContext).Dispose();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
