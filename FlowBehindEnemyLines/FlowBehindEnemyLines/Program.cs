using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlowBehindEnemyLines
{
    public class Edge
    {
        public Node From { get; set; }
        public Node To { get; set; }
        public int Capacity { get; set; }
        public int Flow { get; set; }

        public Edge(Node from, Node to, int capacity)
        {
            From = from;
            To = to;
            Capacity = capacity;
        }
    }

    public class Node
    {
        public readonly int Id;
        public readonly string Name;
        public List<Edge> Outgoing { get; set; }

        public Node(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override int GetHashCode()
        {
            return Id + Name.GetHashCode();
        }
    }

    public class Graph
    {
        public List<Node> Nodes { get; set; }
        public int FlowValue { get; set; } // Used as result of main-graph
        public List<Node> MinCut { get; set; }

        public Graph Copy()
        {
            return new Graph
            {
                Nodes = Nodes.Select(
                    n =>
                        new Node(n.Id, n.Name)
                        {
                            Outgoing = n.Outgoing.Select(e => new Edge(e.From, e.To, e.Capacity)).ToList()
                        }).ToList()
            };
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var graph = new Graph();
            var residualGraph = graph.Copy();

            var alg = new Algorithm(graph, residualGraph,
                graph.Nodes.Find(n => n.Name == "ORIGINS"),
                graph.Nodes.Find(n => n.Name == "DESTINATIONS"));

            alg.DoStuff();
        }

        
    }

    public class Path
    {
        public List<Node> Nodes { get; set; }
        public int Bottleneck { get; set; }

        public Path Copy()
        {
            return new Path
            {
                Nodes = new List<Node>(Nodes),
                Bottleneck = this.Bottleneck
            };
        }
    }

    public class Algorithm
    {
        public Graph Original { get; set; }
        public Graph Residual { get; set; }

        public HashSet<Node> MinimumCut { get; set; }

        public Node Source { get; set; }
        public Node Target { get; set; }

        public Algorithm(Graph original, Graph residual, Node source, Node target)
        {
            Original = original;
            Residual = residual;
            Source = source;
            Target = target;
        }

        public void DoStuff()
        {
            // TODO: Find best path from Source to Target (BFS)

            // Reset MinimumCut
            MinimumCut.Clear();
            var path = FindResidualPath(new Path { Nodes = {Source}, Bottleneck = int.MaxValue });

        }

        private Path FindResidualPath(Path path)
        {
            // Check if this path has just reached DESTINATIONS
            if (path.Nodes.Last().Id == Target.Id) return path;

            if (path.Nodes.Last().Outgoing.Count == 0)
            {
                // No path to target from here
                return path;
            }

            var resultPaths = new List<Path>();
            foreach (var edge in path.Nodes.Last().Outgoing)
            {
                // Check that edge doesn't lead to already-visited node
                if (path.Nodes.Exists(n => n.Id == edge.To.Id)) continue;

                // Update recurisvely used path copy
                var copy = path.Copy();
                MinimumCut.Add(edge.To);
                if (edge.Capacity < copy.Bottleneck) copy.Bottleneck = edge.Capacity;

                // Update set of reached Nodes
                copy.Nodes.Add(edge.To);

                // Store result of recursion
                resultPaths.Add(FindResidualPath(copy));
            }

            Path res = null;
            foreach (var resPath in resultPaths)
            {
                if (resPath.Nodes.Last().Id != Target.Id) continue;

                if (res == null) res = resPath;
                else if (resPath.Bottleneck > res.Bottleneck) // Want the highest bottleneck
                {
                    res = resPath;
                }
            }

            return res;
        }
    }
}
