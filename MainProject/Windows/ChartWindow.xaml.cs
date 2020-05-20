using System;
using System.Collections.Generic;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using GraphLibrary;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using LiveCharts.Defaults;


namespace MainProject.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        public ChartValues<ObservablePoint> Values { get; set; } = new ChartValues<ObservablePoint>();
        public List<Tuple<double, double>> Points { get; set; } = new List<Tuple<double, double>>();
        public ChartWindow(string path)
        {
            InitializeComponent();
            string pointsPath = @"..\..\..\SavedCharts\" + path;
            try
            {
                using (FileStream fs = new FileStream(pointsPath, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    Points = (List<Tuple<double, double>>) binaryFormatter.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
                return;
            }
            Values = new ChartValues<ObservablePoint>();
            foreach (var point in Points)
            {
                Values.Add(new ObservablePoint(point.Item1, point.Item2));
            }
            DataContext = this;

        }

        public ChartWindow(Graph graph) 
        {
            InitializeComponent();


            var points = graph.GetFunctionPoints();
            foreach (var point in points)
            {
                Values.Add(new ObservablePoint(point.Item1, point.Item2));
                Points.Add(new Tuple<double, double>(point.Item1, point.Item2));
            }
            DataContext = this;

        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            string path = @"..\..\..\SavedGraphs\graph.bin";
            var dialog = new NamiingChartDialog();
            dialog.Show();
            dialog.Closed += (object send, EventArgs ev) =>
            {
                var dial = send as NamiingChartDialog;
                path = @"..\..\..\SavedCharts\" + dial.ChartName + ".bin";
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(fs, Points);
                    }
                    MessageBox.Show("График сохранён успешно!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
        }
    }
}
