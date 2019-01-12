using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Kurdspell;

namespace Sample2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load the language dictionary into the Spellchecker
            SpellChecker spellChecker = new SpellChecker("ckb-IQ.txt");

            while(true)
            {
                // Ask the user for input
                Console.Write("Word: ");
                string word = Console.ReadLine();
                
                // Check if the word is correct
                if (spellChecker.Check(word))
                {
                    Console.WriteLine("Correct!");
                }
                else
                {
                    // Give the user at most 3 suggestions
                    List<string> suggestions = spellChecker.Suggest(word, 3);

                    Console.WriteLine("Incorrect, Suggestions: ");
                    foreach(string suggestion in suggestions)
                    {
                        Console.WriteLine(suggestion);
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
