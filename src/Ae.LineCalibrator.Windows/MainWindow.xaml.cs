using System;
using System.ComponentModel;
using System.Windows;

namespace Ae.Mixer.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            (DataContext as IDisposable)?.Dispose();
            base.OnClosing(e);
        }
    }
}
