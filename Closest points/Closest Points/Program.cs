using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;

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
        public static List<Point> SortedByY { get; set; }

        private static bool fullExection = true;
        public static void Main(string[] args)
        {
            if (fullExection)
            {
                var directoryPath = Path.Combine(Path.Combine(Environment.CurrentDirectory, @"../../../"), @"closest-points/data/");
                var files = Directory.GetFiles(directoryPath);
                var watch = new Stopwatch();
                watch.Start();
                foreach (var file in files)
                {
                    if (Path.GetFileName(file).StartsWith("wc")) continue;
                    CalculateResultForFile(Path.GetFileName(file), file);
                    Input.Clear();
                }
                watch.Stop();
                Console.WriteLine(string.Format("FINISHED in {0} ms", watch.ElapsedMilliseconds));
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
                var fullPath = Path.Combine(Path.Combine(Environment.CurrentDirectory, @"../../../"),
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
            SortedByY = Input.OrderBy(p => p.Y).ToList();
            var result = DoWork(input, SortedByY);

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

        private static double DoWork(List<Point> input, IEnumerable<Point> ySorted)
        {
            double dist;
            double delta;
            if (input.Count < 4)
            {
                return GetResultForLessThanFourPoints(input);
            }
            var halfSize = input.Count/2;
            var midElem = input[halfSize];
            var yLeft = new List<Point>();
            var yRight = new List<Point>();
            //foreach (var p in ySorted) // This is slow as hell! (heightens execution time from 15s to 65s)
            //{
            //    if (p.X <= midElem.X)
            //        yLeft.Add(p);
            //    else
            //        yRight.Add(p);
            //}

            var res1 = DoWork(input.GetRange(0, halfSize), yLeft);
            var res2 = DoWork(input.GetRange(halfSize, input.Count - halfSize), yRight);
            delta = Math.Min(res1, res2);
            var ySortedWithinDelta = input.Where(p => (midElem.X - p.X <= delta && midElem.X - p.X >= 0)
                                                      || (p.X - midElem.X <= delta && p.X - midElem.X >= 0)).OrderBy(p => p.Y).ToList();

            if (ySortedWithinDelta.Count == 1) return delta;

            for (int i = 0; i < ySortedWithinDelta.Count; i++)
            {
                var pt = ySortedWithinDelta[i];

                var ySortedWithoutPt = new List<Point>(ySortedWithinDelta);
                ySortedWithoutPt.RemoveAt(i);

                var startIndex = Math.Max(0, i - 8);
                var endIndex = Math.Min(ySortedWithoutPt.Count - 1, i + 8);
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
            double delta = double.MaxValue;
            for (int i = 0; i < input.Count; i++)
            {
                for (int j = 0; j < input.Count; j++)
                {
                    if (i == j) continue; //don't compare the point with itself
                    var dist = Math.Sqrt(Math.Pow(input[i].X - input[j].X, 2) +
                                  Math.Pow(input[i].Y - input[j].Y, 2));
                    if (dist < delta)
                    {
                        delta = dist;
                    }
                }
            }
            return delta;
        }
    }
}
