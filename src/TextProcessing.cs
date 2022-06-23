using BenchmarkDotNet.Attributes;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class TextProcessing
    {
        [Benchmark]
        public int TextProcessing_Trim() => " test ".AsSpan().Trim().Length;
        [Benchmark]
        public int TextProcessing_TrimNoSpan() => " test ".Trim().Length;

        [Benchmark]
        [Arguments("It's exciting to see great performance!")]
        public int TextProcessing_ToUpperInvariant(string s)
        {
            int sum = 0;

            for (int i = 0; i < s.Length; i++)
                sum += char.ToUpperInvariant(s[i]);

            return sum;
        }

        [Benchmark] public string TextProcessing_ToString12345() => 12345.ToString();
        [Benchmark] public string TextProcessing_ToString123() => ((byte)123).ToString();

        private byte[] _bytes = new byte[100];
        private char[] _chars = new char[100];
        private DateTime _dt = DateTime.Now;

        //[Benchmark] public bool TextProcessing_FormatChars() => _dt.TryFormat(_chars, out _, "o");
        [Benchmark] public bool TextProcessing_FormatBytes() => Utf8Formatter.TryFormat(_dt, _bytes, out _, 'O');


        [Benchmark]
        public string TextProcessing_Roundtrip()
        {
            byte[] bytes = Encoding.UTF8.GetBytes("this is a test");
            return Encoding.UTF8.GetString(bytes);
        }

        private static readonly Encoding s_latin1 = Encoding.GetEncoding("iso-8859-1");

        [Benchmark]
        public string TextProcessing_RoundtripLatin1()
        {
            byte[] bytes = s_latin1.GetBytes("this is a test. this is only a test. did it work?");
            return s_latin1.GetString(bytes);
        }

        private char[] _dest = new char[1000];

        //[Benchmark]
        //public void TextProcessing_Encode() => JavaScriptEncoder.Default.Encode("This is a test to see how fast we can encode something that does not actually need encoding", _dest, out _, out _);
    }
}
