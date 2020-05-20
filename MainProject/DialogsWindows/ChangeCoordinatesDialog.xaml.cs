using GraphLibrary;
using System;
using System.Windows;


namespace MainProject
{
    /// <summary>
    /// Логика взаимодействия для ChangeCoordinatesDialog.xaml
    /// </summary>
    public partial class ChangeCoordinatesDialog : Window
    {
        public double X { get; set; }
        public double Y { get; set; }

        public ChangeCoordinatesDialog(Vertex vertex)
        {
            InitializeComponent();
            X = vertex.X;
            Y = vertex.Y;
            xBox.Text = X.ToString();
            yBox.Text = Y.ToString();
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = int.Parse(xBox.Text);
                int y = int.Parse(yBox.Text);
                if (x < 0 || x > 1000 || y < 0 || y > 665)
                    throw new Exception("Числа не входят в интервал!");
                X = x;
                Y = y;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неправильный ввод координат!\nОшибка:" + ex.Message);
            }
            
        }
        
    }
}
