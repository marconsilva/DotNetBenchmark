using BenchmarkDotNet.Attributes;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class Colletions
    {
        private Dictionary<int, int> _dictionary = Enumerable.Range(0, 10_000).ToDictionary(i => i);

        [Benchmark]
        public int Colletions_SumDictionary()
        {
            Dictionary<int, int> dictionary = _dictionary;
            int sum = 0;

            for (int i = 0; i < 10_000; i++)
                if (dictionary.TryGetValue(i, out int value))
                    sum += value;

            return sum;
        }
        private HashSet<int> _set = Enumerable.Range(0, 10_000).ToHashSet();

        [Benchmark]
        public int Colletions_SumHashSet()
        {
            HashSet<int> set = _set;
            int sum = 0;

            for (int i = 0; i < 10_000; i++)
                if (set.Contains(i))
                    sum += i;

            return sum;
        }

        private ConcurrentDictionary<int, int> _concurrentdictionary = new ConcurrentDictionary<int, int>(Enumerable.Range(0, 10_000).Select(i => new KeyValuePair<int, int>(i, i)));

        [Benchmark]
        public int Colletions_SumConcurrentDictionary()
        {
            ConcurrentDictionary<int, int> dictionary = _concurrentdictionary;
            int sum = 0;

            for (int i = 0; i < 10_000; i++)
                if (dictionary.TryGetValue(i, out int value))
                    sum += value;

            return sum;
        }

        private ImmutableArray<int> _array = ImmutableArray.Create(Enumerable.Range(0, 100_000).ToArray());

        [Benchmark]
        public int Colletions_SumImmutableArray()
        {
            int sum = 0;

            foreach (int i in _array)
                sum += i;

            return sum;
        }

        private ImmutableList<int> _list = ImmutableList.Create(Enumerable.Range(0, 1_000).ToArray());

        [Benchmark]
        public int Colletions_SumImmutableList()
        {
            int sum = 0;

            for (int i = 0; i < 1_000; i++)
                if (_list.Contains(i))
                    sum += i;

            return sum;
        }

        private bool[] _bitarray;

        [GlobalSetup]
        public void Setup()
        {
            var r = new Random(42);
            _bitarray = Enumerable.Range(0, 1000).Select(_ => r.Next(0, 2) == 0).ToArray();
        }

        [Benchmark]
        public BitArray Colletions_CreateBitArray() => new BitArray(_bitarray);
    }
}
