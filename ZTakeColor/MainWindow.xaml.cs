using System.Runtime.CompilerServices;
using System.Windows;
using ZTakeColor.ViewModel;

namespace ZTakeColor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            
            
        }
    }
}