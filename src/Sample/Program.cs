using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using Kurdspell;

namespace Sample
{
    class Program
    {
        class CustomConfig : ManualConfig
        {
            public CustomConfig()
            {
                Add(MemoryDiagnoser.Default);
                Add(BaselineColumn.Default);
            }
        }

        static void Main(string[] args)
        {
            //var spellChecker = new Spellchecker("");
            //var list = spellChecker.Suggest("mamostakanm", 5);

            var runner = BenchmarkRunner.Run<Benchmarks>(CustomConfig.Create(DefaultConfig.Instance));

            //var benchmark = new Benchmarks();
            //benchmark.Suggest();
        }


    }
}
