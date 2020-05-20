using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GraphLibrary
{
    [Serializable]
    public class Edge : IEquatable<Edge>
    {
        [NonSerialized]
        private Line model;
        public Line Model { get => model; set => model = value; }
        public Vertex Vertex1 { get; private set; }
        public Vertex Vertex2 { get; private set; }
        public double Length { get => GetLength(); }
        public List<SPoint> SPoints { get; set; } = new List<SPoint>();

        public Edge(Vertex v1, Vertex v2)
        {
            Vertex1 = v1;
            Vertex2 = v2;
        }

        private double GetLength()
        {
            double length = Math.Sqrt((Vertex1.X - Vertex2.X) * (Vertex1.X - Vertex2.X)
                + (Vertex1.Y - Vertex2.Y) * (Vertex1.Y - Vertex2.Y));
            if (Math.Abs(length - Math.Round(length, 3)) < 0.0005)
                return Math.Round(length, 3);
            return length;
        }

        public bool Equals(Edge other)
        {
            return Vertex1.Equals(other.Vertex1) && Vertex2.Equals(other.Vertex2) ||
                Vertex1.Equals(other.Vertex2) && Vertex2.Equals(other.Vertex1);
        }

        public override string ToString()
        {
            return "Первая вершина: \n" + Vertex1.ToString() + "\n" +
                "Вторая вершина: \n" + Vertex2.ToString() + "\n" +
                "Длина: { " + Math.Round(Length, 3).ToString() + " }\n"; 
        }
    }
}
