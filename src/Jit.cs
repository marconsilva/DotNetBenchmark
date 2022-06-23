using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class Jit
    {
        /*[Benchmark]
        public int Zeroing()
        {
            ReadOnlySpan<char> s1 = "hello world";
            ReadOnlySpan<char> s2 = Nop(s1);
            ReadOnlySpan<char> s3 = Nop(s2);
            ReadOnlySpan<char> s4 = Nop(s3);
            ReadOnlySpan<char> s5 = Nop(s4);
            ReadOnlySpan<char> s6 = Nop(s5);
            ReadOnlySpan<char> s7 = Nop(s6);
            ReadOnlySpan<char> s8 = Nop(s7);
            ReadOnlySpan<char> s9 = Nop(s8);
            ReadOnlySpan<char> s10 = Nop(s9);
            return s1.Length + s2.Length + s3.Length + s4.Length + s5.Length + s6.Length + s7.Length + s8.Length + s9.Length + s10.Length;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ReadOnlySpan<char> Nop(ReadOnlySpan<char> span) => default;
*/

        private static bool TryToHex(int value, Span<char> span)
        {
            if ((uint)span.Length <= 7)
                return false;

            ReadOnlySpan<byte> map = new byte[] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9', (byte)'A', (byte)'B', (byte)'C', (byte)'D', (byte)'E', (byte)'F' }; ;
            span[0] = (char)map[(value >> 28) & 0xF];
            span[1] = (char)map[(value >> 24) & 0xF];
            span[2] = (char)map[(value >> 20) & 0xF];
            span[3] = (char)map[(value >> 16) & 0xF];
            span[4] = (char)map[(value >> 12) & 0xF];
            span[5] = (char)map[(value >> 8) & 0xF];
            span[6] = (char)map[(value >> 4) & 0xF];
            span[7] = (char)map[value & 0xF];
            return true;
        }

        private char[] _buffer = new char[100];

        [Benchmark]
        public bool Jit_BoundsChecking() => TryToHex(int.MaxValue, _buffer);

        class A { }
        sealed class B : A { }

        private B[] _array = new B[42];

        [Benchmark]
        public int Jit_Ctor() => new Span<B>(_array).Length;

        private C c1 = new C() { Value = 1 }, c2 = new C() { Value = 2 }, c3 = new C() { Value = 3 };

        [Benchmark]
        public int Jit_Compare() => Comparer<C>.Smallest(c1, c2, c3);

        class Comparer<T> where T : IComparable<T>
        {
            public static int Smallest(T t1, T t2, T t3) =>
                Compare(t1, t2) <= 0 ?
                    (Compare(t1, t3) <= 0 ? 0 : 2) :
                    (Compare(t2, t3) <= 0 ? 1 : 2);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int Compare(T t1, T t2) => t1.CompareTo(t2);
        }

        class C : IComparable<C>
        {
            public int Value;
            public int CompareTo(C other) => other is null ? 1 : Value.CompareTo(other.Value);
        }

        private int[] _array2 = Enumerable.Range(0, 1000).ToArray();

        [Benchmark]
        public bool Jit_IsSorted() => IsSorted(_array2);

        private static bool IsSorted(ReadOnlySpan<int> span)
        {
            for (int i = 0; i < span.Length - 1; i++)
                if (span[i] > span[i + 1])
                    return false;

            return true;
        }

        private int _offset = 0;

        [Benchmark]
        public int Jit_ThrowHelpers()
        {
            var arr = new int[10];
            var s0 = new Span<int>(arr, _offset, 1);
            var s1 = new Span<int>(arr, _offset + 1, 1);
            var s2 = new Span<int>(arr, _offset + 2, 1);
            var s3 = new Span<int>(arr, _offset + 3, 1);
            var s4 = new Span<int>(arr, _offset + 4, 1);
            var s5 = new Span<int>(arr, _offset + 5, 1);
            return s0[0] + s1[0] + s2[0] + s3[0] + s4[0] + s5[0];
        }
    }
}
