using System;
using System.Collections.Generic;
using System.IO;
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
		public List<Edge> Outgoing = new List<Edge>();
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
					t += edge.From.Name + " -> " + edge.To.Name + Environment.NewLine;
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
			//Console.Write(graph.ToString());

			return graph;
		}
	}
}
