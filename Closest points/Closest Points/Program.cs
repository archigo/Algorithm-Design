using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Closest_Points
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; }

        public Point(double x, double y, string name)
        {
            X = x;
            Y = y;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, X: {1}, Y: {2}", Name, X, Y);
        }
    }
    public class Program
    {
        public static List<Point> Input { get; set; } = new List<Point>();
        //public static List<Point> SortedByX { get; set; }
        //public static List<Point> SortedByY { get; set; }

        private static bool fullExection = true;
        public static void Main(string[] args)
        {
            if (fullExection)
            {
                var directoryPath = Path.Combine(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"), @"closest-points\data\");
                var files = Directory.GetFiles(directoryPath);
                foreach (var file in files)
                {
                    CalculateResultForFile(Path.GetFileName(file), file);
                    Input.Clear();
                }
            }
            else
            {
                string fileName;
                if (args.Length > 0 && args[0].Length > 0)
                {
                    fileName = args[0];
                }
                else
                {
                    Console.WriteLine("Write the name of a file in the data subfolder. FX: \"a280 - tsp.txt\"");
                    fileName = Console.ReadLine();
                }
                var fullPath = Path.Combine(Path.Combine(Environment.CurrentDirectory, @"..\..\..\"),
                    string.Format(@"closest-points\data\{0}", fileName));

                CalculateResultForFile(fileName, fullPath);
            }
            
            Console.ReadLine();
        }

        private static void CalculateResultForFile(string fileName, string fullPath)
        {
            var contentArray = File.ReadAllLines(fullPath);

            Parse(contentArray);
            
            var input = Input.OrderBy(p => p.X).ToList();
            var result = DoWork(input);

            Console.WriteLine(fileName + ": " + Input.Count + ", " + result);
        }

        private static void Parse(string[] contents)
        {
            foreach (string line in contents)
            {
                var trimmedLine = line.TrimStart().TrimEnd().Split(null);
                var values = trimmedLine.ToList().Where(r => r.Length != 0).ToList();
                double dummy;
                if (values.Count != 3 || !double.TryParse(values[1], out dummy)) { continue; }
                var x = (double) decimal.Parse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture);
                var y = (double) decimal.Parse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture);
                var point = new Point(x, y, values[0]);
                Input.Add(point);
            }
            if (fullExection) return;
            // Print if just one file
            foreach (var point in Input)
            {
                Console.WriteLine(point);
            }
        }

        private static double DoWork(List<Point> input)
        {
            double dist;
            double delta;
            if (input.Count < 4) //we assume there can never be less than 2 points in input
            {
                return GetResultForLessThanFourPoints(input);
            }
            var halfSize = input.Count/2;
            var res1 = DoWork(input.GetRange(0, halfSize));
            var res2 = DoWork(input.GetRange(halfSize, input.Count - halfSize));
            delta = Math.Min(res1, res2);
            var midElem = input[halfSize];
            var ySortedWithinDelta = input.Where(p => (midElem.X - p.X <= delta && midElem.X - p.X >= 0)
                                                      || (p.X - midElem.X <= delta && p.X - midElem.X >= 0)).OrderBy(p => p.Y).ToList();

            if (ySortedWithinDelta.Count == 1) return delta;

            for (int i = 0; i < ySortedWithinDelta.Count; i++)
            {
                var pt = ySortedWithinDelta[i];
                var ySortedWithoutPt = new List<Point>(ySortedWithinDelta);
                ySortedWithoutPt.RemoveAt(i);

                var startIndex = Math.Max(0, i - 10);
                var endIndex = Math.Min(ySortedWithoutPt.Count - 1, i + 10);
                var length = endIndex - startIndex;
                var comparedTo = ySortedWithoutPt.GetRange(startIndex, length);
                foreach (var cmpPt in comparedTo)
                {
                    dist = Math.Sqrt(Math.Pow(cmpPt.X - pt.X, 2) + Math.Pow(cmpPt.Y - pt.Y, 2));
                    if (dist < delta)
                    {
                        delta = dist;
                    }
                }
            }
            return delta;
        }

        private static double GetResultForLessThanFourPoints(List<Point> input)
        {
            double delta = 0;
            for (int i = 0; i < input.Count; i++)
            {
                for (int j = 0; j < input.Count; j++)
                {
                    if (i == j) continue; //don't compare the point with itself
                    var dist =
                        Math.Sqrt(Math.Pow(input[i].X - input[j].X, 2) +
                                  Math.Pow(input[i].Y - input[j].Y, 2));
                    if (i == 0 && j == 1) delta = dist; //set the first delta so it is not 0

                    else if (dist < delta)
                    {
                        delta = dist;
                    }
                }
            }
            return delta;
        }
    }
}
