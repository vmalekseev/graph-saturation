using GraphLibrary;
using System;
using System.Windows;

namespace MainProject.DialogsWindows
{
    /// <summary>
    /// Логика взаимодействия для ChangeEdgeLengthDialog.xaml
    /// </summary>
    public partial class ChangeEdgeLengthDialog : Window
    {
        public double Length { get; private set; }
        public ChangeEdgeLengthDialog(Edge edge)
        {
            InitializeComponent();
            Length = edge.Length;
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(double.TryParse(aBox.Text, out double a) &&
                    double.TryParse(powerBox.Text, out double pow) &&
                    a > 0 && pow > 0))
                    throw new Exception("Основание и степень введены некорректно!");
                Length = Math.Pow(a, 1 / pow);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неправильный ввод данных!\nОшибка:" + ex.Message);
            }

        }
    }
}
