using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.ComponentModel;

namespace Ae.LineCalibrator.Interface
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new LineCalibratorViewModel();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ((IDisposable)DataContext).Dispose();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
