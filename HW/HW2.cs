// A utility to analyze text files and provide statistics
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Design;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PRN212_HW_1
{
    class HW2
    {
        public static void Run(string[] args)
        {
            Console.WriteLine("File Analyzer - .NET Core");
            Console.WriteLine("This tool analyzes text files and provides statistics.");
            
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a file path as a command-line argument.");
                Console.WriteLine("Example: dotnet run myfile.txt");
                return;
            }
            
            string filePath = args[0];
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File '{filePath}' does not exist.");
                return;
            }
            
            try
            {
                Console.WriteLine($"Analyzing file: {filePath}");
                
                // Read the file content
                string content = File.ReadAllText(filePath);
                
                // TODO: Implement analysis functionality
                // 1. Count words
                // 2. Count characters (with and without whitespace)
                // 3. Count sentences
                // 4. Identify most common words
                // 5. Average word length
                
                // Example implementation for counting lines:
                int lineCount = File.ReadAllLines(filePath).Length;
                Console.WriteLine($"Number of lines: {lineCount}");
                
                // TODO: Additional analysis to be implemented
                Console.WriteLine($"Number of words: {countWords(content)}");
                Console.WriteLine($"Number of characters (with whitespace, without whitespace): {countCharacters(content)}");
                Console.WriteLine($"Number of sentences: {countSentences(content)}");
                Console.WriteLine($"Most common words: {mostCommon(content)}");
                Console.WriteLine($"Average length: {averageLength(content)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during file analysis: {ex.Message}");
            }
        }

        internal static long countWords(string str)
        {
            // return str.Trim().Split(' ').Length;
            // return Regex.Split(str, "[^\\w]").Length;

            // https://stackoverflow.com/questions/16725848/how-to-split-text-into-words
            var matches = Regex.Matches(str, @"\w+[^\s]*\w+|\w");
            return matches.Count;
        }

        // with white space, without
        internal static (long, long) countCharacters(string str)
        {
            return (str.Length, new string((from c in str
                                            where !char.IsWhiteSpace(c)
                                            select c).ToArray()).Length);
        }

        // https://regex101.com/r/bF3rK7/1
        // short but expensive, creating an array of strings just to get the length
        internal static long countSentences(string str)
        {
            return Regex.Split(str, "(?<=[.?!;])\\s+(?=\\p{Lu})").Length;
        }

        internal static string mostCommon(string str)
        {
            var matches = Regex.Matches(str, @"\w+[^\s]*\w+|\w");

            Dictionary<string, long> dict = new Dictionary<string, long>();

            StringBuilder builder = new StringBuilder();

            long highest = 0;

            foreach (Match match in matches) 
            {
                string lowered = match.Value.ToLower();

                if (!dict.TryGetValue(lowered, out long temp))
                {
                    dict.Add(lowered, ++temp);
                }
                else
                {
                    dict[lowered] = ++temp;
                }

                highest = long.Max(highest, temp);
            }

            foreach (KeyValuePair<string, long> entry in dict)
            {
                if (entry.Value == highest)
                {
                    builder.Append(entry.Key).Append(' ');
                }
            }

            return builder.ToString();
        }

        internal static double averageLength(string str)
        {
            return (double) new string((from c in str
                                        where char.IsLetterOrDigit(c)
                                        select c).ToArray()).Length / countWords(str);
        }
    }
}