using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

using GT.RText.Core;

namespace GT.RText.Comparer
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var first = File.ReadAllBytes(args[0]);
            var second = File.ReadAllBytes(args[1]);

            var parser = new RTextParser();
            parser.Read(first);
            var firstRtext = parser.RText;

            parser.RText = null;
            parser.Read(second);
            var secondRtext = parser.RText;

            Compare(firstRtext, secondRtext);
        }

        private static void Compare(RTextBase first, RTextBase second)
        {
            using var sw = new StreamWriter("compare.txt");

            var firstPages = first.GetPages();
            var secondPages = second.GetPages();

            int max = Math.Max(firstPages.Count, secondPages.Count);

            int rightI = 0;
            int leftI = 0;

            for (var i = 0; i < max; i++)
            {
                KeyValuePair<string, RTextPageBase>? firstPage = null;
                KeyValuePair<string, RTextPageBase>? secondPage = null;

                if (rightI < firstPages.Count)
                    firstPage = firstPages.ElementAt(rightI);

                if (leftI < secondPages.Count)
                    secondPage = secondPages.ElementAt(leftI);

                if (firstPage is not null && secondPage is not null)
                {
                    if (firstPage.Value.Key != secondPage.Value.Key)
                    {
                        int comp = firstPage.Value.Key.CompareTo(secondPage.Value.Key);
                        if (comp > 0)
                        {
                            sw.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            sw.WriteLine($"Page {secondPage.Value.Key} does not exist in Left");
                            sw.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            leftI++;
                        }
                        else if (comp < 0)
                        {
                            sw.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            sw.WriteLine($"Page {firstPage.Value.Key} does not exist in Right");
                            sw.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                            rightI++;
                        }
                        
                    }
                    else
                    {
                        SortedDictionary<string, RTextPairUnit> firstPairUnits = firstPage.Value.Value.PairUnits;
                        SortedDictionary<string, RTextPairUnit> secondPairUnits = secondPage.Value.Value.PairUnits;

                        if (firstPairUnits.Count != secondPairUnits.Count)
                        {
                            sw.WriteLine($"# Page '{firstPage.Value.Key}' (Left: {firstPage.Value.Value.PairUnits.Count}, Right:{secondPage.Value.Value.PairUnits.Count})");
                        }

                        bool different = ComparePairs(sw, firstPairUnits, secondPairUnits);
                        if (different)
                            sw.WriteLine();

                        rightI++;
                        leftI++;
                    }
                }
                else
                {
                    ;
                }
            }
        }

        private static bool ComparePairs(StreamWriter sw, SortedDictionary<string, RTextPairUnit> firstPairUnits, SortedDictionary<string, RTextPairUnit> secondPairUnits)
        {
            bool diff = false;

            int max = Math.Max(firstPairUnits.Count, secondPairUnits.Count);

            int leftI = 0;
            int rightI = 0;

            List<RTextPairUnit> leftList = firstPairUnits.Select(e => e.Value).OrderBy(e => e.ID).ToList();
            List<RTextPairUnit> rightList = secondPairUnits.Select(e => e.Value).OrderBy(e => e.ID).ToList();

            for (var i = 0; i < max; i++)
            {
                RTextPairUnit? firstPair = null;
                RTextPairUnit? secondPair = null;

                if (leftI < firstPairUnits.Count)
                    firstPair = leftList[leftI];

                if (rightI < secondPairUnits.Count)
                    secondPair = rightList[rightI];

                if (firstPair is not null && firstPair.ID < secondPair.ID)
                {
                    sw.WriteLine($"### REMOVED PAIR (ID: {firstPair.ID}, Label: {firstPair.Label})\n - {firstPair.Label}");
                    leftI++;

                    diff = true;
                }
                else if (firstPair is null || secondPair.ID < firstPair.ID)
                {
                    sw.WriteLine($"### NEW PAIR (ID: {secondPair.ID}, Label: {secondPair.Label})\n - {secondPair.Label}");
                    rightI++;

                    diff = true;
                }
                else if (firstPair.ID == secondPair.ID)
                {
                    if (firstPair.Label != secondPair.Label)
                    {
                        sw.WriteLine($"### LABEL CHANGE (ID: {firstPair.ID}) {firstPair.Label} -> {secondPair.Label}");

                        diff = true;
                    }
                    
                    if (firstPair.Value != secondPair.Value)
                    {
                        sw.WriteLine($"### VALUE CHANGE (ID: {firstPair.ID} - {firstPair.Label}) \n - Old: {firstPair.Value}\n - New: {secondPair.Value}");

                        diff = true;
                    }

                    leftI++;
                    rightI++;
                }
            }

            return diff;
        }
    }
}
