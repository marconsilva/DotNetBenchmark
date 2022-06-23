using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class Json
    {
        private MemoryStream _stream = new MemoryStream();
        private DateTime[] _array = Enumerable.Range(0, 1000).Select(_ => DateTime.UtcNow).ToArray();

        [Benchmark]
        public Task Json_LargeArraySerialize()
        {
            _stream.Position = 0;
            return JsonSerializer.SerializeAsync(_stream, _array);
        }

        private MemoryStream _Dictionarystream = new MemoryStream();
        private JsonSerializerOptions _options = new JsonSerializerOptions();
        private Dictionary<string, int> _instance = new Dictionary<string, int>()
        {
            { "One", 1 }, { "Two", 2 }, { "Three", 3 }, { "Four", 4 }, { "Five", 5 },
            { "Six", 6 }, { "Seven", 7 }, { "Eight", 8 }, { "Nine", 9 }, { "Ten", 10 },
        };

        [Benchmark]
        public async Task Json_DictionarySerialize()
        {
            _stream.Position = 0;
            await JsonSerializer.SerializeAsync(_Dictionarystream, _instance, _options);
        }


        private MemoryStream _SimpleTypestream = new MemoryStream();
        private MyAwesomeType _SimpleTypeinstance = new MyAwesomeType() { SomeString = "Hello", SomeInt = 42, SomeByte = 1, SomeDouble = 1.234 };

        [Benchmark]
        public Task Json_SimpleTypeSerialize()
        {
            _stream.Position = 0;
            return JsonSerializer.SerializeAsync(_SimpleTypestream, _SimpleTypeinstance);
        }

        public struct MyAwesomeType
        {
            public string SomeString { get; set; }
            public int SomeInt { get; set; }
            public double SomeDouble { get; set; }
            public byte SomeByte { get; set; }
        }
    }
}
