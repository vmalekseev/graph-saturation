using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary
{ 
    class Solution
    {
        Graph graph;
        double minTime = 0;
        public Solution(Graph graph)
        {
            this.graph = graph;
        }

        public List<Tuple<double, double>> Solve()
        {
            var result = new List<Tuple<double, double>>();
            double step = (graph.MaxEps - graph.MinEps) / 100;
            for (double eps = graph.MaxEps; eps >= graph.MinEps; eps -= step)
            {
                result.Add(new Tuple<double, double>(eps, GetSaturationTime(eps)));
            }
            return result;
        }

        private double GetSaturationTime(double eps)
        {
            foreach (var edge in graph.Edges)
                edge.SPoints = new List<SPoint>();
            
            List<SPoint> queue = new List<SPoint>();
            foreach (var edge in graph.StartVertex.EdgesList)
            {
                var p = new SPoint(edge, edge.Vertex1 == graph.StartVertex ?
                    edge.Vertex2 : edge.Vertex1, edge.Length);
                edge.SPoints.Add(p);
                queue.Add(p);
            }
            queue.Sort((p1, p2) => p1.Distance.CompareTo(p2.Distance));

            double time = 0;
            while (time <= minTime || !IsGraphSaturated(eps))
            {
                var current = queue[0];
                queue.RemoveAt(0);
                current.Edge.SPoints.Remove(current);

                double dist = current.Distance;
                time += current.Distance / graph.SaturationSpeed;
                foreach (var point in queue)
                    point.Distance -= dist;
                foreach (var edge in current.NextVertex.EdgesList)
                {
                    var p = new SPoint(edge, edge.Vertex1 == current.NextVertex ?
                    edge.Vertex2 : edge.Vertex1, edge.Length);
                    int left = 0;
                    int right = queue.Count;
                    while (right - left > 1)
                    {
                        int m = (right + left) / 2;
                        if (queue[m].Distance > p.Distance)
                            right = m;
                        else
                            left = m;
                    }
                    if (queue.Count == 0)
                        queue.Add(p);
                    else if (queue[left].Distance > p.Distance)
                        queue.Insert(left, p);
                    else
                        queue.Insert(right, p);
                    edge.SPoints.Add(p);
                }
            }

            
            minTime = time;
            return time;
        }

        private bool IsGraphSaturated(double eps)
        {
            foreach (var edge in graph.Edges)
            {
                var coords = new List<double>();
                foreach (var point in edge.SPoints)
                    coords.Add(point.NextVertex == edge.Vertex1 ? point.Distance
                        : edge.Length - point.Distance);
                coords.Sort();
                if (eps < coords[0])
                    return false;
                for (int i = 0; i < coords.Count - 1; i++)
                {
                    if (coords[i + 1] - coords[i] > 2 * eps)
                        return false;
                }
                if (eps < edge.Length - coords[coords.Count - 1])
                    return false;
            }
            return true;
        }
    }
}
