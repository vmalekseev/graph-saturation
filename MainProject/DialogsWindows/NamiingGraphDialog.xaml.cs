using System.Windows;

namespace MainProject
{
    /// <summary>
    /// Логика взаимодействия для NamiingGraphDialog.xaml
    /// </summary>
    public partial class NamiingGraphDialog : Window
    {
        public string GraphName { get; set; } = "graph";

        public NamiingGraphDialog()
        {
            InitializeComponent();
            textBox.Text = GraphName;
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            GraphName = textBox.Text;
            Close();
        }
    }
}
