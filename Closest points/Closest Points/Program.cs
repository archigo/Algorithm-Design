using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Closest_Points
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Name { get; set; }
    }
    public class Program
    {
        private static string inputFile = "data\\";

        public static List<Point> Input { get; set; }
        //public static List<Point> SortedByX { get; set; }
        //public static List<Point> SortedByY { get; set; }
        public static void Main(string[] args)
        {
            // TODO: Parsing
            Console.WriteLine("write the name of a file in the data subfoler. FX: \"a280 - tsp.txt\"");
            inputFile += Console.ReadLine();

            Parse();
            var input = Input.OrderBy(p => p.X).ToList();
            double result = DoWork(input);
        }

        private static void Parse()
        {
            System.IO.StreamReader file =
                new System.IO.StreamReader(inputFile);
            
            string line;
            while ((line = file.ReadLine()) != null)
            {



                System.Console.WriteLine(line);
                
            }

            file.Close();
        }

        private static void Overall()
        {
        }

        private static double DoWork(List<Point> input)
        {
            var dist = 0.0;
            var delta = 0.0;
            if (input.Count <= 3) //we assume there can never be less than 2 points in input
            {
                
                for (int i = 0; i < input.Count; i++)
                {
                    for (int j = 0; j < input.Count; j++)
                    {
                        if (i == j) continue; //don't compare the point with itself
                        dist =
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
            var halfSize = input.Count/2;
            var part1 = input.GetRange(0, halfSize);
            var part2 = input.GetRange(halfSize, input.Count - halfSize);
            var res1 = DoWork(part1);
            var res2 = DoWork(part2);
            delta = res1 <= res2 ? res1 : res2;
            var midElem = input[halfSize];
            var ySortedWithinDelta = input.Where(p => (midElem.X - p.X <= delta && midElem.X - p.X >= 0)
                                                      || (p.X - midElem.X <= delta && p.X - midElem.X >= 0)).OrderBy(p => p.Y).ToList();
            var part1SortY = part1.Where(p => midElem.X - p.X <= delta).OrderBy(p => p.Y);
            var part2SortY = part2.Where(p => p.X - midElem.X <= delta).OrderBy(p => p.Y);
            var result = delta;
            for (int i = 0; i < ySortedWithinDelta.Count; i++)
            {
                var pt = ySortedWithinDelta[i];
                var comparedTo = ySortedWithinDelta.GetRange(i - 10 < 0 ? 0 : i - 10,
                    i + 10 > ySortedWithinDelta.Count - 1 ? ySortedWithinDelta.Count - 1 : i + 10);
                    //20 - i >= ySortedWithinDelta.Count ? ySortedWithinDelta.Count - 1 : 20 - i);
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


    }
}
