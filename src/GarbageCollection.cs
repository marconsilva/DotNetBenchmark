using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class GarbageCollection
    {

        public class DoubleSorting : Sorting<double> { protected override double GetNext() => _random.Next(); }
        public class Int32Sorting : Sorting<int> { protected override int GetNext() => _random.Next(); }
        public class StringSorting : Sorting<string>
        {
            protected override string GetNext()
            {
                var dest = new char[_random.Next(1, 5)];
                for (int i = 0; i < dest.Length; i++) dest[i] = (char)('a' + _random.Next(26));
                return new string(dest);
            }
        }

        public abstract class Sorting<T>
        {
            protected Random _random;
            private T[] _orig, _array;

            [Params(10)]
            public int Size { get; set; }

            protected abstract T GetNext();

            [GlobalSetup]
            public void Setup()
            {
                _random = new Random(42);
                _orig = Enumerable.Range(0, Size).Select(_ => GetNext()).ToArray();
                _array = (T[])_orig.Clone();
                Array.Sort(_array);
            }

            [Benchmark]
            public void GarbageCollection_RandomSorting()
            {
                _orig.AsSpan().CopyTo(_array);
                Array.Sort(_array);
            }
        }
    }
}
