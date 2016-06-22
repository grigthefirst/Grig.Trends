using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grig.Trends.Services
{
    public class FuzzySearcher
    {

        public List<FuzzyGroup> RankWords(string text, double fuzzyness)
        {
            return RankWords(GetWords(text), fuzzyness);
        }

        public List<FuzzyGroup> RankWords(IEnumerable<string> words, double fuzzyness)
        {
            List<FuzzyGroup> fuzzyGroups = new List<FuzzyGroup>();
            while(words.Count() > 0)
            {
                List<string> foundWords = this.FuzzySearch(words.First(), words, fuzzyness);
                fuzzyGroups.Add(new FuzzyGroup() { Forms = foundWords.Distinct().ToList(), Count = foundWords.Count });
                words = words.Except(foundWords);
            }
            return fuzzyGroups;
        }

        public IEnumerable<string> GetWords(string text)
        {
            string cleanStr = Regex.Replace(text.ToLower(), @"[.'"",]", "");
            return cleanStr.Split(' ');
        }

        public List<string> FuzzySearch(string searchWord, IEnumerable<string> words, double fuzzyness)
        {
            List<string> foundWords = new List<string>();

            foreach (string s in words)
            {
                // Calculate the Levenshtein-distance:
                int levenshteinDistance =
                    CountLevenshteinDistance(searchWord, s);

                // Length of the longer string:
                int length = Math.Max(searchWord.Length, s.Length);

                // Calculate the score:
                double score = 1.0 - (double)levenshteinDistance / length;

                // Match?
                if (score > fuzzyness)
                    foundWords.Add(s);
            }
            return foundWords;
        }

        private int CountLevenshteinDistance(string source, string destination)
        {
            int[,] d = new int[source.Length + 1, destination.Length + 1];
            int i, j, cost;
            char[] str1 = source.ToCharArray();
            char[] str2 = destination.ToCharArray();

            for (i = 0; i <= str1.Length; i++)
            {
                d[i, 0] = i;
            }
            for (j = 0; j <= str2.Length; j++)
            {
                d[0, j] = j;
            }
            for (i = 1; i <= str1.Length; i++)
            {
                for (j = 1; j <= str2.Length; j++)
                {

                    if (str1[i - 1] == str2[j - 1])
                        cost = 0;
                    else
                        cost = 1;

                    d[i, j] =
                        Math.Min(
                            d[i - 1, j] + 1,              // Deletion
                            Math.Min(
                                d[i, j - 1] + 1,          // Insertion
                                d[i - 1, j - 1] + cost)); // Substitution

                    if ((i > 1) && (j > 1) && (str1[i - 1] ==
                        str2[j - 2]) && (str1[i - 2] == str2[j - 1]))
                    {
                        d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                    }
                }
            }

            return d[str1.Length, str2.Length];
        }
    }
}
