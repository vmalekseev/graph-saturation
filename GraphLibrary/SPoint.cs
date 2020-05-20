using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary
{
    [Serializable]
    public class SPoint
    {
        public Edge Edge { get; set; }
        public Vertex NextVertex { get; set; }
        public double Distance { get; set; }

        public SPoint(Edge edge, Vertex next, double distance)
        {
            Edge = edge;
            NextVertex = next;
            Distance = distance;
        }
    } 
}
