using System.Windows;

namespace MainProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void OnOpenLoaderCLick(object sender, RoutedEventArgs e)
        {
            var loaderWindow = new LoaderWindow();
            loaderWindow.Show();
            Close();
        }

        private void OnOpenCreatorCLick(object sender, RoutedEventArgs e)
        {
            var creatorWindow = new CreatorWindow();
            creatorWindow.Show();
            Close();
        }

        private void OnOpenChartLoaderClick(object sender, RoutedEventArgs e)
        {
            var loaderWindow = new ChartLoaderWindow();
            loaderWindow.Show();
            Close();
        }
    }
}
