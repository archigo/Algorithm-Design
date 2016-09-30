using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Gorilla
{
    public static class Program
    {
        public static Dictionary<string, int> IdToIndex { get; set; } = new Dictionary<string, int>();
        public static int[][] CostMatrix { get; set; }


        public static string First { get; set; }
        public static string Second { get; set; }
        public static Dictionary<Tuple<int, int>, Tuple<int, string>> Cache { get; set; } = new Dictionary<Tuple<int, int>, Tuple<int, string>>();

        public static int CountCaches { get; set; }
        static void Main(string[] args)
        {
            Second = "MVHLTPEEKSAVTALWGKVNVDEVGGEALGRLLVVYPWTQRFFESFGDLSTPDAVMGNPKVKAHGKKVLGAFSDGLAHLDNLKGTFATLSELHCDKLHVDPENFRLLGNVLVCVLAHHFGKEFTPPVQAAYQKVVAGVANALAHKYH";
            First = "PIVDTGSVAPLSAAEKTKIRSAWAPVYSTYETSGVDILVKFFTSTPAAQEFFPKFKGLTTADELKKSADVRWHAERIINAVDDAVASMDDTEKMSMKLRNLSGKHAKSFQVDPEYFKVLAAVIADTVAAGDAGFEKLMSMICILLRSAY";
            if (args.Length == 2)
            {
                First = args[0];
                Second = args[1];


            }
            var stars = Math.Abs(First.Length - Second.Length);

            if (First.Length < Second.Length)
                First = new StringBuilder().Append('*', stars).Append(First).ToString();
            else
                Second = new StringBuilder().Append('*', stars).Append(Second).ToString();

            // Parse Cost-Matrix
            var contents = File.ReadAllLines(Path.Combine(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\"),
                    string.Format(@"data\{0}", "BLOSUM62.txt")));
            ParseCostMatrix(contents);

            // Get some input

            // Result: Should be "K-AK" but is "KAK*"
            // Do work given last valid indexes
            var res = DoWork(First.Length - 1, Second.Length - 1);
            Console.WriteLine("Based on: ");
            Console.WriteLine("Cost: {0}\n{1}", res.Item1, res.Item2);

             Console.ReadLine();
        }
        
        // "OPT"
        private static Tuple<int, string> DoWork(int idx1, int idx2)
        {
            if (idx1 == 0 && idx2 == 0)
            {
            }
                Tuple<int, string> result;
            if (Cache.TryGetValue(Tuple.Create(idx1, idx2), out result))
            {
                CountCaches++;
                return result;
            }
            // Check out of bounds
            if (idx1 == -1 || idx2 == -1) return Tuple.Create(0, ""); // "****"

            var char1 = First[idx1];
            var char2 = Second[idx2];


            int costIdx1;
            if (!IdToIndex.TryGetValue(char1.ToString(), out costIdx1)) throw new Exception("Oh no, we failed!");
            int costIdx2;
            if (!IdToIndex.TryGetValue(char2.ToString(), out costIdx2)) throw new Exception("Oh no, we failed!");

            var minusCost = -4; // Could look up in matrix under "*" and char2
            var exchangeCost = CostMatrix[costIdx1][costIdx2];
             if (idx1 <= 5 && idx2 <= 5)
            {

            }
            if (idx1 == 0 && idx2 == 0)
            {
                var res = exchangeCost >= minusCost
                    ? Tuple.Create(exchangeCost, "" + char2)
                    : Tuple.Create(minusCost, "*");
                if(Cache.ContainsKey(Tuple.Create(idx1,idx2)))
                {
                    Tuple<int,String> tempRes;
                    Cache.TryGetValue(Tuple.Create(idx1, idx2), out tempRes);
                    if (tempRes != null && (tempRes.Item1 != res.Item1 || !tempRes.Item2.Equals(res.Item2)))
                        Console.WriteLine(res.Item1 + " vs cached " + tempRes);
                }
                else
                    Cache.Add(Tuple.Create(idx1, idx2), res);
                //Console.WriteLine("Cached: ({0}, {1}) --> {2}, {3}", idx1, idx2, res.Item1, res.Item2);
                return res;
            }




            //var minus1 = idx2 -1 >0  && idx1 > 0 ? DoWork(idx1, idx2 - 1) : new Tuple<int, string>(-10000, "*");
            //var replace = idx2 > 0 && idx1 > 0 ? DoWork(idx1 - 1, idx2 - 1) : new Tuple<int, string>(-10000, "*");
            //var minus2 = idx1 - 1 > 0 && idx2 > 0 ? DoWork(idx1 - 1, idx2) : new Tuple<int, string>(-10000, "*");

            var minus1 = DoWork(idx1, idx2 - 1);
            var replace = DoWork(idx1 - 1, idx2 - 1);
            var minus2 = DoWork(idx1 - 1, idx2);


            //if (minus1.Item2.Equals("out")) minus1 = Tuple.Create(-999, "out");
            //if (minus2.Item2.Equals("out")) minus2 = Tuple.Create(-999, "out");
            //if (replace.Item2.Equals("out")) replace = Tuple.Create(-999, "out");

            //if (idx1 == 0 && idx2 == 1)
            //Console.Write("debug");


            var best = Math.Max(minus1.Item1 + minusCost, Math.Max(replace.Item1 + exchangeCost, minus2.Item1 + minusCost));
            string part;
             if (replace.Item1 + exchangeCost == best)
            {
                part = replace.Item2 + char2;
                //Console.WriteLine("Exchanging {0} with {1}", char1, char2);
            }
            else if(minus1.Item1 + minusCost == best)
            {
                part = minus1.Item2 + "*";
                //Console.WriteLine("minus 1! (Iterating string 2)");
            }
            else if (minus2.Item1 + minusCost == best)
            {
                part = minus2.Item2 + "*";
                //Console.WriteLine("minus 2! (Iterating string 1)");
            }
            else throw new Exception("Oh no, we failed!");

            if (idx1 <= 15 && idx2 <= 5)
            {
                // for breakpoints to inspect first few chars
            }

            // Cache idx + "part"
            var costAndPart = Tuple.Create(best, part);
            if (Cache.ContainsKey(Tuple.Create(idx1, idx2)))
            {
                Tuple<int, String> tempRes;
                Cache.TryGetValue(Tuple.Create(idx1, idx2), out tempRes);
                if (tempRes != null && (tempRes.Item1 != costAndPart.Item1 || !tempRes.Item2.Equals(costAndPart.Item2)))
                    Console.WriteLine(costAndPart.Item1 + " vs cached " + tempRes);
            }
            else
                Cache.Add(Tuple.Create(idx1, idx2), costAndPart);
            //Console.WriteLine("Cached: ({0}, {1}) --> {2}, {3}", idx1, idx2, best, part);
            return costAndPart;
        }

        private static void ParseCostMatrix(string[] contents)
        {
            foreach (var line in contents)
            {
                if (line.StartsWith("#") || line.Length == 0) continue;

                var betterLine = Regex.Replace(line, @"\s+", " ");
                var splitted = betterLine.TrimStart().TrimEnd().Split(null);
                if (line.StartsWith(" ")) // Column identifiers
                {
                    for (int j = 0; j < splitted.Length; j++)
                    {
                        IdToIndex.Add(splitted[j], j);
                    }
                    CostMatrix = new int[IdToIndex.Count][];
                    continue;
                }

                // Actual matrix rows
                var row = new int[IdToIndex.Count];
                for (int colIdx = 1; colIdx < splitted.Length; colIdx++)
                {
                    int cost;
                    if (!int.TryParse(splitted[colIdx], out cost)) throw new Exception("Oh no, we failed!");

                    row[colIdx - 1] = cost;
                }

                int rowIndex;
                if (!IdToIndex.TryGetValue(splitted[0], out rowIndex)) throw new Exception("Oh no, we failed!");
                CostMatrix[rowIndex] = row;
            }
        }
    }
}
