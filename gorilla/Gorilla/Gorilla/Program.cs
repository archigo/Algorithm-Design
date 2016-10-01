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
            Second = "MVHLTPEEKSAVTALWGKVNVDEVGGEALGRLLVVYPWTQRFFESFGDLSTPDAVMGNPKVKAHGKKVLGAFSDGLAHLDNLKGTFATLSELHCDKLHVDPENFKLLGNVLVCVLAHHFGKEFTPPVQAAYQKVVAGVANALAHKYH";
            First = "XGGTLAIQAQGDLTLAQKKIVRKTWHQLMRNKTSFVTDVFIRIFAYDPSAQNKFPQMAGMSASQLRSSRQMQAHAIRVSSIMSEYVEELDSDILPELLATLARTHDLNKVGADHYNLFAKVLMEALQAELGSDFNEKTRDAWAKAFSVVQAVLLVKHG";
            if (args.Length == 2)
            {
                First = args[0];
                Second = args[1];


            }

            Console.WriteLine("\n\nINPUT\n1: {0}\n2: {1}",First,Second);


            var stars = Math.Abs(First.Length - Second.Length);

            if (First.Length < Second.Length)
                First = new StringBuilder().Append('*', stars).Append(First).ToString();
            else
                Second = new StringBuilder().Append('*', stars).Append(Second).ToString();

            // Parse Cost-
            try
            {
                // works from IDE
                var contents = File.ReadAllLines(Path.Combine(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\"),string.Format(@"data\{0}", "BLOSUM62.txt")));
                ParseCostMatrix(contents);
            }
            catch(Exception e)
            {
                // wworks from CMD
                var contents = File.ReadAllLines(Path.Combine(Path.Combine(Environment.CurrentDirectory, @"..\..\"),string.Format(@"data\{0}", "BLOSUM62.txt")));
                ParseCostMatrix(contents);
            }

            Console.WriteLine("\n\nOUTPUT");

            var res = DoWork(First.Length - 1, Second.Length - 1);
            Console.WriteLine("Ordered, cost: {0}\n{1}", res.Item1, res.Item2);

            var temp = First;
            First = Second;
            Second = temp;

            res = DoWork(First.Length - 1, Second.Length - 1);
            Console.WriteLine("Reveresed order, cost: {0}\n{1}", res.Item1, res.Item2);

            Console.ReadLine();
        }
        
        // "OPT"
        private static Tuple<int, string> DoWork(int idx1, int idx2)
        {
            // Try to get from cache
            Tuple<int, string> result;
            if (Cache.TryGetValue(Tuple.Create(idx1, idx2), out result))
                return result;

            // Check out of bounds
            if (idx1 == -1 || idx2 == -1)
                return Tuple.Create(0, ""); // 

            var char1 = First[idx1];
            var char2 = Second[idx2];

            int costIdx1;
            if (!IdToIndex.TryGetValue(char1.ToString(), out costIdx1)) throw new Exception("Oh no, we failed!");
            int costIdx2;
            if (!IdToIndex.TryGetValue(char2.ToString(), out costIdx2)) throw new Exception("Oh no, we failed!");

            var minusCost = -4; // Could look up in matrix under "*" and char2
            var exchangeCost = CostMatrix[costIdx1][costIdx2];

            
            if (idx1 == 0 && idx2 == 0)
            {
                var res = exchangeCost >= minusCost
                    ? Tuple.Create(exchangeCost, "" + char2)
                    : Tuple.Create(minusCost, "*");

                Cache.Add(Tuple.Create(idx1, idx2), res);
                return res;
            }

            var minus1 = DoWork(idx1, idx2 - 1);
            var replace = DoWork(idx1 - 1, idx2 - 1);
            var minus2 = DoWork(idx1 - 1, idx2);

            // best match for the current indexes
            var best = Math.Max(minus1.Item1 + minusCost, Math.Max(replace.Item1 + exchangeCost, minus2.Item1 + minusCost));
            string part;

            //append new character based on best result
            if (replace.Item1 + exchangeCost == best)           
                part = replace.Item2 + char2;
            else if(minus1.Item1 + minusCost == best)
                part = minus1.Item2 + "*";
            else if (minus2.Item1 + minusCost == best)
                part = minus2.Item2 + "*";
            // or throw exception if somehow none of the values matches the best one
            else throw new Exception("Oh no, we failed!");

            // Cache idx1 and idx2 + "part"
            var costAndPart = Tuple.Create(best, part);
            Cache.Add(Tuple.Create(idx1, idx2), costAndPart);

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
