using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class RuntimeHelpers
    {
        private List<string> _list = new List<string>();

        // IReadOnlyCollection<out T> is covariant
        [Benchmark] public bool RuntimeHelpers_IsIReadOnlyCollection() => IsIReadOnlyCollection(_list);
        [MethodImpl(MethodImplOptions.NoInlining)] private static bool IsIReadOnlyCollection(object o) => o is IReadOnlyCollection<int>;

        [Benchmark]
        public void RuntimeHelpers_GenericDictionaries()
        {
            for (int i = 0; i < 14; i++)
                GenericMethod<string>(i);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static object GenericMethod<T>(int level)
        {
            switch (level)
            {
                case 0: return typeof(T);
                case 1: return typeof(List<T>);
                case 2: return typeof(List<List<T>>);
                case 3: return typeof(List<List<List<T>>>);
                case 4: return typeof(List<List<List<List<T>>>>);
                case 5: return typeof(List<List<List<List<List<T>>>>>);
                case 6: return typeof(List<List<List<List<List<List<T>>>>>>);
                case 7: return typeof(List<List<List<List<List<List<List<T>>>>>>>);
                case 8: return typeof(List<List<List<List<List<List<List<List<T>>>>>>>>);
                case 9: return typeof(List<List<List<List<List<List<List<List<List<T>>>>>>>>>);
                case 10: return typeof(List<List<List<List<List<List<List<List<List<List<T>>>>>>>>>>);
                case 11: return typeof(List<List<List<List<List<List<List<List<List<List<List<T>>>>>>>>>>>);
                case 12: return typeof(List<List<List<List<List<List<List<List<List<List<List<List<T>>>>>>>>>>>>);
                default: return typeof(List<List<List<List<List<List<List<List<List<List<List<List<List<T>>>>>>>>>>>>>);
            }
        }
    }
}
