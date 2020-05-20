using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary
{
    [Serializable]
    public class Graph
    {
        public List<Vertex> Vertices { get; } = new List<Vertex>();
        public List<Edge> Edges { get; } = new List<Edge>();
        public Vertex StartVertex { get; set; }

        public double MinEps { get; set; }
        public double MaxEps { get; set; }
        public double SaturationSpeed { get; set; }

        public void AddEdge(Edge edge)
        {
            if (!ContainsEdge(edge))
            {
                Edges.Add(edge);
                edge.Vertex1.EdgesList.Add(edge);
                edge.Vertex2.EdgesList.Add(edge);
            }
        }
        public void AddVertex(Vertex vertex)
        {
            if (!ContainsVertex(vertex))
                Vertices.Add(vertex);
        }
        public bool ContainsEdge(Edge edge) => Edges.Contains(edge);
        public bool ContainsVertex(Vertex vertex) => Vertices.Contains(vertex);
        public void RemoveEdge(Edge edge)
        {
            if (!ContainsEdge(edge)) return;
            edge.Vertex1.EdgesList.Remove(edge);
            edge.Vertex2.EdgesList.Remove(edge);
            Edges.Remove(edge);
        }
        public void RemoveVertex(Vertex vertex)
        {
            if (!ContainsVertex(vertex)) return;
            foreach (var edge in vertex.EdgesList)
                RemoveEdge(edge);
            Vertices.Remove(vertex);
        }
        

        public List<Tuple<double, double>> GetFunctionPoints()
        {
            Solution solution = new Solution(this);
            return solution.Solve();
        }

    }
}
