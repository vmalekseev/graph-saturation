using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GraphLibrary
{
    [Serializable]
    public class Vertex : IEquatable<Vertex>
    {
        [NonSerialized]
        private Ellipse model;
        public List<Edge> EdgesList { get; set; } = new List<Edge>();
        public Ellipse Model { get => model; set => model = value; }
        public double X { get; set; }
        public double Y { get; set; }

        public bool Equals(Vertex other)
        {
            return (X == other.X) && (Y == other.Y); 
        }

        public override string ToString()
        {
            return "Координата X: { " + ((int)X).ToString() + " }\n" +
                "Координата Y: { " + ((int)Y).ToString() + " }\n" + 
                "Степень: { " + EdgesList.Count + " }\n";
        }
    }
}
