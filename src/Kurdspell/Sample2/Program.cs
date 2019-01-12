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
            Kurdspell kurdspell = new Kurdspell("ckb-IQ.txt");

            while(true)
            {
                // Ask the user for input
                Console.Write("Word: ");
                string word = Console.ReadLine();
                
                // Check if the word is correct
                if (kurdspell.Check(word))
                {
                    Console.WriteLine("Correct!");
                }
                else
                {
                    // Give the user a list of suggestions
                    List<string> suggestions = kurdspell.Suggest(word, 3);

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
