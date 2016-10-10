using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FlowBehindEnemyLines
{
	public class Edge
	{
		public int FromId { get; set; }
		public int ToId { get; set; }
		public int Capacity { get; set; }
		public int Flow { get; set; }

		public Edge(int fromId, int toId, int capacity)
		{
			FromId = fromId;
			ToId = toId;
			Capacity = capacity;
		}
	}

    public class Node
    {
        public readonly int Id;
        public readonly string Name;
        public List<Edge> Outgoing { get; set; } = new List<Edge>();

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
						new Node(n.Id, n.Name)
						{
							Outgoing = n.Outgoing.Select(e => new Edge(e.FromId, e.ToId, e.Capacity)).ToList()
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
					t += Nodes.Find(n => n.Id == edge.FromId).Name + " -> " + Nodes.Find(n => n.Id == edge.ToId).Name + Environment.NewLine;
				}
			}
			return t;
		}
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var directoryPath = "../../rail.txt";

			var graph = new Parser(directoryPath).Parse();
			var residualGraph = graph.Copy();

			var alg = new Algorithm(graph, residualGraph,
				graph.Nodes.Find(n => n.Name == "ORIGINS").Id,
				graph.Nodes.Find(n => n.Name == "DESTINATIONS").Id);

			alg.DoStuff();

		    Console.Read();
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

        public HashSet<Node> MinimumCut { get; set; } = new HashSet<Node>();

        public int SourceId { get; set; }
        public int TargetId { get; set; }

		public Algorithm(Graph original, Graph residual, int sourceId, int targetId)
		{
			Original = original;
			Residual = residual;
			SourceId = sourceId;
			TargetId = targetId;
		}

        public void DoStuff()
        {
            // TODO: Find best path from Source to Target (BFS)

            // Reset MinimumCut
            MinimumCut.Clear();
            var sourceNode = Original.Nodes.Find(n => n.Id == SourceId);
            var sourceNodeList = new List<Node>();
            sourceNodeList.Add(sourceNode);
            var pathStartingPoint = new Path { Nodes = sourceNodeList, Bottleneck = int.MaxValue };
            var path = FindResidualPath(pathStartingPoint);

#if DEBUG
            if (path != null)
            {
                Console.WriteLine("Path found:");
                foreach (var node in path.Nodes)
                {
                    if (node == path.Nodes.First()) Console.Write(node.Name);
                    else Console.Write(" --> {0}", node.Name);
                }
                Console.WriteLine();
                Console.WriteLine("Bottleneck: {0}", path.Bottleneck);
            }
#endif

        }

        private Path FindResidualPath(Path path)
        {
            // Check if this path has just reached DESTINATIONS
            if (path.Nodes.Last().Id == TargetId) return path;

            if (path.Nodes.Last().Outgoing.Count == 0)
            {
                // No path to target from here
                return path;
            }

            var resultPaths = new List<Path>();
            foreach (var edge in path.Nodes.Last().Outgoing)
            {
                // Check that edge doesn't lead to already-visited node
                if (path.Nodes.Exists(n => n.Id == edge.ToId)) continue;

                var node = Original.Nodes.Find(n => n.Id == edge.ToId);

                // Update recurisvely used path copy
                var copy = path.Copy();
                copy.Nodes.Add(node);
                if (edge.Capacity == -1)
                {
                    // TODO: Handle infinity...
                }
                if (edge.Capacity < copy.Bottleneck) copy.Bottleneck = edge.Capacity;

                // Update set of reached Nodes
                MinimumCut.Add(node);

                // Store result of recursion
                var resPath = FindResidualPath(copy);
                if (resPath != null) resultPaths.Add(resPath);
            }

            Path res = null;
            foreach (var resPath in resultPaths)
            {
                if (resPath.Nodes.Last().Id != TargetId) continue;

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
				graph.Nodes.Add(new Node(i - 1, contentArray[i]));

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
				var edge = new Edge(from.Id, to.Id, c);
				// Add edge as outgoing on the from node
				from.Outgoing.Add(edge);

				// Test
				//Console.WriteLine("Edge: " + lineArray[0] + " " + lineArray[1] + " " + lineArray[2]);
			}

			// Test
			//Console.Write(graph.ToString());

			return graph;
		}
	}
}
