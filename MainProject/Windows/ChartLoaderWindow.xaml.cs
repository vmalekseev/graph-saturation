using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MainProject.Windows;

namespace MainProject
{
    /// <summary>
    /// Логика взаимодействия для LoaderWindow.xaml
    /// </summary>
    public partial class ChartLoaderWindow : Window
    {
        string Path { get; set; }

        public ChartLoaderWindow()
        {
            InitializeComponent();
            string path = @"..\..\..\SavedCharts";
            foreach (var filePath in Directory.EnumerateFiles(path, "*.bin"))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                Button button = new Button()
                {
                    Content = directoryInfo.Name.Substring(0, directoryInfo.Name.Length - 4),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5),
                    Foreground = Brushes.WhiteSmoke,
                    FontSize = 18
                };
                button.Click += OnInfoButtonClick;
                panel.Children.Add(button);

            }
        }

        private void OnInfoButtonClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            textBlock.Text = "Имя: " + btn.Content.ToString() + "\n";
            Path = btn.Content.ToString() + ".bin";

            DirectoryInfo directoryInfo = new DirectoryInfo(@"..\..\..\SavedCharts\" + Path);
            textBlock.Text += "Дата создания: " + directoryInfo.CreationTime;
            Button button = new Button()
            {
                Content = "Начать",
                Margin = new Thickness(5),
                BorderThickness = new Thickness(0)
            };
            button.Click += OnGoButtonClick;

            Button button2 = new Button()
            {
                Content = "Удалить",
                Background = Brushes.Red,
                Margin = new Thickness(5),
                Foreground = Brushes.Yellow,
                BorderThickness = new Thickness(0)
            };
            button2.Click += OnDeleteButtonClick;

            StackPanel stack = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0,55,0,55)
            };
            Grid.SetColumn(stack, 1);
            Grid.SetRow(stack, 1);
            stack.Children.Add(button);

            
            Grid.SetColumn(button2, 1);
            Grid.SetRow(button2, 1);
            stack.Children.Add(button2);

            grid.Children.Add(stack);
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
            Close();
        }

        private void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                File.Delete(@"..\..\..\SavedCharts\" + Path);
                var window = new ChartLoaderWindow();
                window.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Невозможно удалить файл: " + ex.Message);
            }
        }

        private void OnGoButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new ChartWindow(Path);
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки файла: " + ex.Message);
                return;
            }
        }
    }
}
