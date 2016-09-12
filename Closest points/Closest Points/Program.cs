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
    }
    public class Program
    {
        private const string inputFile = "";

        public static List<Point> Input { get; set; }
        public static List<Point> SortedByX { get; set; }
        public static List<Point> SortedByY { get; set; }
        public static void Main(string[] args)
        {
            // TODO: Parsing


            var input = Input.OrderBy(p => p.X).ToList();
            double result = DoWork(input);
        }

        private static void Parse()
        {
            
        }

        private static void Overall()
        {
            
        }

        private static double DoWork(List<Point> input)
        {
            var halfSize = input.Count/2;
            var part1 = input.GetRange(0, halfSize);
            var part2 = input.GetRange(halfSize, input.Count - halfSize);
            var res1 = DoWork(part1);
            var res2 = DoWork(part2);
            var delta = res1 <= res2 ? res1 : res2;
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
                    20 - i >= ySortedWithinDelta.Count ? ySortedWithinDelta.Count - 1 : 20 - i);
                foreach (var cmpPt in comparedTo)
                {
                    if (Math.Pow(pt.X, 2) + Math.Pow(pt.Y - , 2))
                }

            }
        }


    }
}
