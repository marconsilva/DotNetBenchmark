using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class Linq
    {
        [GlobalSetup]
        public void Setup()
        {
            var r = new Random(42);
            _array = Enumerable.Range(0, 1_000).Select(_ => r.Next()).ToArray();
        }

        private int[] _array;

        [Benchmark]
        public void Linq_Sort()
        {
            foreach (int i in _array.OrderBy(i => i)) { }
        }

        //private IEnumerable<int> data = Enumerable.Range(0, 100).ToList();

        //[Benchmark]
        //public int Linq_SkipLast() => data.SkipLast(5).Sum();
    }
}
