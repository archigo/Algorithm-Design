using System;
using System.Collections.Generic;
using System.IO;
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
		public List<Node> Nodes = new List<Node>();
		public int FlowValue { get; set; } // Used as result of main-graph
		public List<Node> MinCut = new List<Node>();

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

		public override string ToString()
		{
			var t = "";
			foreach (Node node in Nodes)
			{
				foreach (Edge edge in node.Outgoing)
				{
					t += edge.From.Name + " ---[" + (edge.Capacity > -1 && edge.Capacity < 10 ? "0" + edge.Capacity : "" + edge.Capacity) + "]--> " + edge.To.Name + Environment.NewLine;
				}
			}
			return t;
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var directoryPath = "../../network.txt";

			var graph = new Parser(directoryPath).Parse();
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

	public class Parser
	{
		public string FilePath { get; set; }

		public Parser(string path)
		{
			FilePath = path;
		}

		public Graph Parse()
		{
			var contentArray = File.ReadAllLines(FilePath);

			var graph = new Graph();

			// Parse the first line to get the number of nodes
			int n = int.Parse(contentArray[0]);
			for (int i = 1; i <= n; i++)
			{ // Parse each node, and place them in the graph
				graph.Nodes.Add(new Node()
				{
					Name = contentArray[i],
					Id = i - 1
				});

				// Test
				//Console.WriteLine("Node: " + contentArray[i] + ", " + (i - 1));
			}

			// Parse the next line after nodes to get the number of edges
			int m = int.Parse(contentArray[n + 1]);
			for (int i = n + 2; i <= n + 1 + m; i++)
			{ // Parse each edge
			  // Get u, v and c
				var lineArray = contentArray[i].Split(' ');
				// u is the index of the first node
				int u = int.Parse(lineArray[0]);
				// v is the index of the second node
				int v = int.Parse(lineArray[1]);
				// c is the capacity of the edge
				int c = int.Parse(lineArray[2]);

				// Get nodes for from and to
				Node from = graph.Nodes.Find(node => node.Id == u);
				Node to = graph.Nodes.Find(node => node.Id == v);

				// Create the edge
				var edge = new Edge(from, to, c);
				// Add edge as outgoing on the from node
				from.Outgoing.Add(edge);

				// Test
				//Console.WriteLine("Edge: " + lineArray[0] + " " + lineArray[1] + " " + lineArray[2]);
			}

			// Test
			// Console.Write(graph.ToString());

			return graph;
		}
	}
}
