using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Edge> Outgoing { get; set; }
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
                        new Node
                        {
                            Id = n.Id,
                            Name = n.Name,
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

    public class Algorithm
    {
        public Graph Original { get; set; }
        public Graph Residual { get; set; }

        public Node Source { get; set; }
        public Node Target { get; set; }

        public Algorithm(Graph original, Graph residual, Node source, Node target)
        {
            Original = original;
            Residual = residual;
            Source = source;
            Target = target;
        }

        public List<Node> ComputeMinCut()
        {
            return new List<Node>(); // TODO:
        }

        public void DoStuff()
        {
            // TODO: Find best path from Source to Target (BFS)

        }
    }
}
