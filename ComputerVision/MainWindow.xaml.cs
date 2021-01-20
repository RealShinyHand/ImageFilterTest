using ComputerVision.model;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ComputerVision
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {


            InitializeComponent();
            this.DataContext = new MainWindowModel();
        }
    }
}
