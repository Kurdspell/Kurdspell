using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

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
