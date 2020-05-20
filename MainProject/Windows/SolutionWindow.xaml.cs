using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GraphLibrary;
using MainProject.Windows;

namespace MainProject
{
    /// <summary>
    /// Логика взаимодействия для SolutionWindow.xaml
    /// </summary>
    public partial class SolutionWindow : Window
    {
        // Сдвиг для расположения объекта на полотне (10 - 10 * sqrt(2))
        private const double shift = -4.14213562373;

        /// <summary>
        /// Описывает граф, с которым идёт работа
        /// </summary>
        private Graph Graph { get; set; }

        // Вершина с которой в данный момент идёт взаимодействие
        Vertex curVertex;
        // Ребро с которым в данный момент идёт взаимодействие
        Edge curEdge;
        // Словарь эллипс-вершина для нахождения вершины на полотне по эллипсу
        Dictionary<Ellipse, Vertex> verticisDictionary = new Dictionary<Ellipse, Vertex>();
        // Словарь линия-ребро для нахождения ребра на полотне по линии
        Dictionary<Line, Edge> edgesDictionary = new Dictionary<Line, Edge>();
       
        /// <summary>
        /// Конструктор инициализирует окно и загружает граф для работы с ним
        /// </summary>
        /// <param name="path"> Имя файла сохранённого графа </param>
        public SolutionWindow(string path)
        {
            InitializeComponent();

            string graphPath = @"..\..\..\SavedGraphs\" + path;
            try
            {
                using (FileStream fs = new FileStream(graphPath, FileMode.Open, FileAccess.Read))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    Graph = (Graph)binaryFormatter.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
                return;
            }
            

            ShowGraph();
        }
        /// <summary>
        /// Рисует граф на полотне
        /// </summary>
        private void ShowGraph()
        {
            foreach (var vertex in Graph.Vertices)
                ShowVertex(vertex);
            foreach (var edge in Graph.Edges)
                ShowEdge(edge);
        }
        /// <summary>
        /// Рисует вершину на полотне
        /// </summary>
        /// <param name="edge"> вершина </param>
        private void ShowEdge(Edge edge)
        {
            Line line = new Line()
            {
                Stroke = Brushes.Black,
                X1 = edge.Vertex1.X + shift,
                Y1 = edge.Vertex1.Y + shift,
                X2 = edge.Vertex2.X + shift,
                Y2 = edge.Vertex2.Y + shift,
                StrokeThickness = 2,
                Cursor = Cursors.Hand,
                
            };

            line.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) => 
            { 
                var line1 = sender as Line;
                ShowEdgeInfo(line1);
            };
            line.SetValue(Canvas.ZIndexProperty, 1);

            TextBlock lengthTextBlock = new TextBlock()
            {
                Text = Math.Round(edge.Length, 3).ToString(),
                Foreground = Brushes.WhiteSmoke
            };
            lengthTextBlock.SetValue(Canvas.ZIndexProperty, 3);
            Canvas.SetLeft(lengthTextBlock, shift + (edge.Vertex1.X + edge.Vertex2.X) / 2);
            Canvas.SetTop(lengthTextBlock, shift + (edge.Vertex1.Y + edge.Vertex2.Y) / 2);
            canvas.Children.Add(lengthTextBlock);

            edgesDictionary[line] = edge;
            canvas.Children.Add(line);
        }
        /// <summary>
        /// Рисует ребро на полотне
        /// </summary>
        /// <param name="vertex"> ребро </param>
        private void ShowVertex(Vertex vertex)
        {
            Ellipse ellipse = new Ellipse()
            {
                Fill = new SolidColorBrush(Color.FromRgb(103, 55, 184)),
                Width = 20,
                Height = 20,
                Cursor = Cursors.Hand,
                StrokeThickness = 1,
                Stroke = Brushes.Black
            };

            ellipse.SetValue(Canvas.LeftProperty, vertex.X - 10 * Math.Sqrt(2));
            ellipse.SetValue(Canvas.TopProperty, vertex.Y - 10 * Math.Sqrt(2));
            ellipse.SetValue(Canvas.ZIndexProperty, 2);

            verticisDictionary[ellipse] = vertex;
            canvas.Children.Add(ellipse);
            vertex.Model = ellipse;

            ellipse.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                if (Graph.StartVertex != null)
                    Graph.StartVertex.Model.Fill = new SolidColorBrush(Color.FromRgb(103, 55, 184));
                Graph.StartVertex = vertex;
                Graph.StartVertex.Model.Fill = Brushes.LightGreen;
            };
            ellipse.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                var ellipse1 = sender as Ellipse;
                ShowVertexInfo(ellipse1);
            };
        }

        /// <summary>
        /// Показывает информацию о данном ребре
        /// </summary>
        /// <param name="line"> Линия представляющая ребро на полотне </param>
        private void ShowEdgeInfo(Line line)
        {
            curEdge = edgesDictionary[line];
            infoBlock.Children.Clear();
            var label = new Label()
            {
                Content = edgesDictionary[line].ToString(),
                Foreground = Brushes.WhiteSmoke,
                FontSize = 18
            };
            infoBlock.Children.Add(label);
        }
        /// <summary>
        /// Показывает информацию о данной вершине
        /// </summary>
        /// <param name="ellipse"> Эллипс представляющий вершину на полотне </param>
        private void ShowVertexInfo(Ellipse ellipse)
        {
            curVertex = verticisDictionary[ellipse];
            infoBlock.Children.Clear();

            var label = new Label()
            {
                Content = verticisDictionary[ellipse].ToString(),
                Foreground = Brushes.WhiteSmoke,
                FontSize = 18
            };
            infoBlock.Children.Add(label);
        }

        
        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            var window = new LoaderWindow();
            window.Show();
            Close();
        }
        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            if (Graph.StartVertex == null)
            {
                MessageBox.Show("Выберите стартовую вершину!");
                return;
            }
            if (!(double.TryParse(minEpsBox.Text, out double minEps) && 
                double.TryParse(maxEpsBox.Text, out double maxEps) &&
                minEps > 0 && maxEps > 0 &&  minEps < maxEps))
            {
                MessageBox.Show("Введите корректно минимальную и максимальную длину окрестности, а также скорость насыщения!");
                return;
            }
            Graph.MinEps = minEps;
            Graph.MaxEps = maxEps;
            Graph.SaturationSpeed = speedSlider.Value;
            var window = new ChartWindow(Graph);
            window.Show();
        }
    }
}
