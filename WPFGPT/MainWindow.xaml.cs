using System;
using System.Windows.Controls;


namespace WPFGPT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Closed += this.OnClosed;
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}