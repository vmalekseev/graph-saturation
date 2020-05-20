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
using MainProject.DialogsWindows;

namespace MainProject
{
    /// <summary>
    /// Логика взаимодействия для CreatorWindow.xaml
    /// </summary>
    public partial class CreatorWindow : Window
    {
        // Сдвиг для расположения объекта на полотне (10 - 10 * sqrt(2))
        private const double shift = -4.14213562373;
        // Точка хранящая координаты расположения курсора мыши на полотне
        Point point;

        // Вершина с которой в данный момент идёт взаимодействие
        Vertex curVertex;
        // Ребро с которым в данный момент идёт взаимодействие
        Edge curEdge;
        // Словарь эллипс-вершина для нахождения вершины на полотне по эллипсу
        Dictionary<Ellipse, Vertex> verticisDictionary = new Dictionary<Ellipse, Vertex>();
        // Словарь линия-ребро для нахождения ребра на полотне по линии
        Dictionary<Line, Edge> edgesDictionary = new Dictionary<Line, Edge>();

        // Первая вершина для создания ребра
        Vertex firstVertex;
        // Вторая вершина для создания ребра
        Vertex secondVertex;

        /// <summary>
        /// Описывает создающийся граф
        /// </summary>
        Graph Graph { get; set; } = new Graph();

        /// <summary>
        /// Конструктор окна создания графа
        /// </summary>
        public CreatorWindow()
        {
            InitializeComponent();
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Show();
            Close();
        }
        private void OnCreateVertexClick(object sender, RoutedEventArgs e)
        {
            canvas.MouseLeftButtonDown += OnCanvasMouseDownCreateVertex;
            canvas.MouseLeftButtonUp += OnCanvasMouseUpCreateVertex;
        }
        private void OnCreateEdgeClick(object sender, RoutedEventArgs e)
        {
            if (firstVertex != null && secondVertex != null && firstVertex != secondVertex)
                CreateEdge();
        }
        private void OnDeleteEdgeClick(object sender, RoutedEventArgs e)
        {
            if (curEdge == null) return;
            canvas.Children.Remove(curEdge.Model);
            Graph.RemoveEdge(curEdge);
            infoBlock.Children.Clear();
        }
        private void OnDeleteVertexClick(object sender, RoutedEventArgs e)
        {
            if (curVertex == null) return;
            var edgesListCopy = new Edge[curVertex.EdgesList.Count];
            curVertex.EdgesList.CopyTo(edgesListCopy);
            foreach (var edge in edgesListCopy)
            {
                canvas.Children.Remove(edge.Model);
                Graph.RemoveEdge(edge);
            }
            canvas.Children.Remove(curVertex.Model);
            Graph.RemoveVertex(curVertex);
            infoBlock.Children.Clear();
        }
        private void OnChangeVertexCoordinatesClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ChangeCoordinatesDialog(curVertex);
            dialog.Show();
            dialog.Closed += (object send, EventArgs ev) => 
            {
                var dial = send as ChangeCoordinatesDialog;
                MoveVertex(dial.X, dial.Y, false);
                ShowVertexInfo(curVertex.Model);
            };
        }
        private void OnChangeEdgeLengthClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ChangeEdgeLengthDialog(curEdge);
            dialog.Show();
            dialog.Closed += (object send, EventArgs ev) =>
            {
                var dial = send as ChangeEdgeLengthDialog;
                var length = dial.Length;
                Vertex v1, v2;
                if (curEdge.Vertex1.EdgesList.Count >= curEdge.Vertex2.EdgesList.Count)
                {
                    curVertex = curEdge.Vertex2;
                    v1 = curEdge.Vertex1;
                    v2 = curEdge.Vertex2;
                }
                else
                {
                    curVertex = curEdge.Vertex1;
                    v1 = curEdge.Vertex2;
                    v2 = curEdge.Vertex1;
                }
                var vectorX = (v1.X - v2.X) * length / curEdge.Length;
                var vectorY = (v1.Y - v2.Y) * length / curEdge.Length;
                MoveVertex(v1.X - vectorX, v1.Y - vectorY, false);
                ShowEdgeInfo(curEdge.Model);
            };
        }
        private void SaveGraphClick(object sender, RoutedEventArgs e)
        {
            string path = @"..\..\..\SavedGraphs\graph.bin";
            var dialog = new NamiingGraphDialog();
            dialog.Show();
            dialog.Closed += (object send, EventArgs ev) =>
            {
                var dial = send as NamiingGraphDialog;
                path = @"..\..\..\SavedGraphs\" + dial.GraphName + ".bin";
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        BinaryFormatter binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(fs, Graph);
                    }
                    MessageBox.Show("Граф сохранён успешно!");
                    var window = new MainWindow();
                    window.Show();
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };

        }

        /// <summary>
        /// Создаёт новое ребро в графе
        /// </summary>
        private void CreateEdge()
        {

            Line line = new Line()
            {
                Stroke = Brushes.Black,
                X1 = firstVertex.X + shift,
                Y1 = firstVertex.Y + shift,
                X2 = secondVertex.X + shift,
                Y2 = secondVertex.Y + shift,
                StrokeThickness = 2,
                Cursor = Cursors.Hand
            };
            line.MouseLeftButtonDown += OnCanvasMouseUpCreateEdge;
            line.SetValue(Canvas.ZIndexProperty, 1);
            Edge edge = new Edge(firstVertex, secondVertex) { Model = line };
            

            if (!Graph.ContainsEdge(edge))
            {
                Graph.AddEdge(edge);
                edgesDictionary[line] = edge;
                canvas.Children.Add(line);
            }

            firstVertex.Model.Fill = new SolidColorBrush(Color.FromRgb(103, 55, 184));
            secondVertex.Model.Fill = new SolidColorBrush(Color.FromRgb(103, 55, 184));
            firstVertex = null;
            secondVertex = null;

        }
        /// <summary>
        /// Создаёт новую вершину в графе
        /// </summary>
        private void CreateVertex()
        {
            Ellipse ellipse = new Ellipse()
            {
                Fill = new SolidColorBrush(Color.FromRgb(103,55,184)),
                Width = 20,
                Height = 20,
                Cursor = Cursors.Hand,
                StrokeThickness = 1,
                Stroke = Brushes.Black
            };

            ellipse.MouseRightButtonUp += OnVertexMouseRightButtonUp;
            ellipse.MouseLeftButtonDown += OnVertexMouseLeftButtonDown;
            
            ellipse.SetValue(Canvas.LeftProperty, point.X - 10 * Math.Sqrt(2));
            ellipse.SetValue(Canvas.TopProperty, point.Y - 10 * Math.Sqrt(2));
            ellipse.SetValue(Canvas.ZIndexProperty, 2);

            Vertex vertex = new Vertex()
            {
                X = point.X,
                Y = point.Y,
                Model = ellipse 
            };

            if (!Graph.ContainsVertex(vertex))
            {
                verticisDictionary.Add(ellipse, vertex);
                Graph.AddVertex(vertex);
                canvas.Children.Add(ellipse);
            }

            canvas.MouseLeftButtonDown -= OnCanvasMouseDownCreateVertex;
            canvas.MouseLeftButtonUp -= OnCanvasMouseUpCreateVertex;
        }
        /// <summary>
        /// Передвигает вершину на заданные координаты
        /// </summary>
        /// <param name="x"> координата x </param>
        /// <param name="y"> координата y </param>
        /// <param name="isMouseMove">  в движении ли находится мышь </param>
        private void MoveVertex(double x, double y, bool isMouseMove)
        {
            double canvasLeft = double.Parse(curVertex.Model.GetValue(Canvas.LeftProperty).ToString());
            double canvasTop = double.Parse(curVertex.Model.GetValue(Canvas.TopProperty).ToString());
            if (!isMouseMove || !(x <= 20 - shift + canvasLeft && x >= canvasLeft &&
                            y <= 20 - shift + canvasTop && y >= canvasTop))
            {
                curVertex.Model.SetValue(Canvas.LeftProperty, x + shift - 10);
                curVertex.Model.SetValue(Canvas.TopProperty, y + shift - 10);
                curVertex.X = x;
                curVertex.Y = y;
                foreach (var edge in curVertex.EdgesList)
                {
                    if (curVertex == edge.Vertex1)
                    {
                        edge.Model.X1 = curVertex.X + shift;
                        edge.Model.Y1 = curVertex.Y + shift;
                    }
                    else
                    {
                        edge.Model.X2 = curVertex.X + shift;
                        edge.Model.Y2 = curVertex.Y + shift;
                    }
                }
            }
        }
        /// <summary>
        /// Показывает информацию о данном ребре
        /// </summary>
        /// <param name="line"> Линия представляющая ребро на полотне </param>
        private void ShowEdgeInfo(Line line)
        {
            curEdge = edgesDictionary[line];
            infoBlock.Children.Clear();

            // Добавление информации о ребре
            var label = new Label()
            {
                Content = edgesDictionary[line].ToString(),
                Foreground = Brushes.WhiteSmoke,
                FontSize = 18

            };
            infoBlock.Children.Add(label);

            // Добавление кнопки удаления ребра
            var changeEdgeButton = new Button()
            {
                Content = "Изменить",
                FontSize = 18,
                Background = Brushes.Yellow,
                BorderThickness = new Thickness(0),
                Foreground = Brushes.Red
            };
            changeEdgeButton.Click += OnChangeEdgeLengthClick;
            infoBlock.Children.Add(changeEdgeButton);

            // Добавление кнопки удаления ребра
            var deleteEdgeButton = new Button() 
            {
                Content = "Удалить", 
                FontSize = 18,
                Background = Brushes.Red,
                BorderThickness = new Thickness(0),
                Foreground = Brushes.Yellow
            };
            deleteEdgeButton.Click += OnDeleteEdgeClick;
            infoBlock.Children.Add(deleteEdgeButton);
        }
        /// <summary>
        /// Показывает информацию о данной вершине
        /// </summary>
        /// <param name="ellipse"> Эллипс представляющий вершину на полотне </param>
        private void ShowVertexInfo(Ellipse ellipse)
        {
            curVertex = verticisDictionary[ellipse];
            infoBlock.Children.Clear();

            // Добавление информации о вершине
            var label = new Label()
            {
                Content = verticisDictionary[ellipse].ToString(),
                Foreground = Brushes.WhiteSmoke,
                FontSize = 18
            };
            infoBlock.Children.Add(label);

            // Добавление кнопки изменения вершины
            var changeVertexCoordinatesButton = new Button()
            { 
                Content = "Изменить",
                FontSize = 18,
                Background = Brushes.Yellow,
                Foreground = Brushes.Red,
                BorderThickness = new Thickness(0)
            };
            changeVertexCoordinatesButton.Click += OnChangeVertexCoordinatesClick;
            infoBlock.Children.Add(changeVertexCoordinatesButton);

            infoBlock.Children.Add(new Label());
            
            // Добавление кнопки удаления вершины
            var deleteVertexButton = new Button()
            {
                Content = "Удалить",
                FontSize = 18,
                Background = Brushes.Red,
                Foreground = Brushes.Yellow,
                BorderThickness = new Thickness(0),
                
            };
            deleteVertexButton.Click += OnDeleteVertexClick;
            infoBlock.Children.Add(deleteVertexButton);
        }

        private void OnCanvasMouseLeftButtonUpMoveVertex(object sender, MouseButtonEventArgs e)
        {
            MoveVertex(point.X, point.Y, true);
            canvas.MouseMove -= OnCanvasMouseMoveMoveVertex;
            canvas.MouseLeftButtonUp -= OnCanvasMouseLeftButtonUpMoveVertex;
            ShowVertexInfo(curVertex.Model);
        }
        private void OnCanvasMouseMoveMoveVertex(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                point = e.GetPosition(this);

        }
        private void OnVertexMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            curVertex = verticisDictionary[sender as Ellipse];
            point = e.GetPosition(this);
            canvas.MouseMove += OnCanvasMouseMoveMoveVertex;
            canvas.MouseLeftButtonUp += OnCanvasMouseLeftButtonUpMoveVertex;
            ShowVertexInfo(curVertex.Model);
        }
        private void OnVertexMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = sender as Ellipse;

            if (firstVertex == null)
            {
                ellipse.Fill = Brushes.Pink;
                firstVertex = verticisDictionary[ellipse];
                return;
            }

            if (secondVertex == null)
            {
                ellipse.Fill = Brushes.Pink;
                secondVertex = verticisDictionary[ellipse];
                return;
            }

            firstVertex.Model.Fill = new SolidColorBrush(Color.FromRgb(103, 55, 184));
            firstVertex = secondVertex;
            ellipse.Fill = Brushes.Pink;
            secondVertex = verticisDictionary[ellipse];
        }
        private void OnCanvasMouseUpCreateVertex(object sender, MouseButtonEventArgs e) => CreateVertex();
        private void OnCanvasMouseDownCreateVertex(object sender, MouseButtonEventArgs e) => point = e.GetPosition(this);
        private void OnCanvasMouseUpCreateEdge(object sender, MouseButtonEventArgs e)
        {
            var line = sender as Line;
            ShowEdgeInfo(line);
        }

    }
}
