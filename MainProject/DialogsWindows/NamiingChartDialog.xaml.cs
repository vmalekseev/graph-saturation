using System.Windows;

namespace MainProject
{
    /// <summary>
    /// Логика взаимодействия для NamiingGraphDialog.xaml
    /// </summary>
    public partial class NamiingChartDialog : Window
    {
        public string ChartName { get; set; } = "chart";
        public NamiingChartDialog()
        {
            InitializeComponent();
            textBox.Text = ChartName;
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            ChartName = textBox.Text;
            Close();
        }
    }
}
