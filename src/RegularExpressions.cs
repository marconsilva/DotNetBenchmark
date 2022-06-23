using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class RegularExpressions
    {
        private readonly string _input = new HttpClient().GetStringAsync("http://www.gutenberg.org/cache/epub/1112/pg1112.txt").Result;
        private Regex _regex;

        [Params(false, true)]
        public bool Compiled { get; set; }

        [GlobalSetup]
        public void Setup() => _regex = new Regex(@"^.*\blove\b.*$", RegexOptions.Multiline | (Compiled ? RegexOptions.Compiled : RegexOptions.None));

        [Benchmark] public int RegularExpressions_Count() => _regex.Matches(_input).Count;

        private readonly Regex _regex2 = new Regex("hello.*world", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly string _input2 = "abcdHELLO" + new string('a', 128) + "WORLD123";

        [Benchmark] public bool RegularExpressions_IsMatch() => _regex2.IsMatch(_input2);


        private Regex _email = new Regex(@"[\w\.+-]+@[\w\.-]+\.[\w\.-]+", RegexOptions.Compiled);
        private Regex _uri = new Regex(@"[\w]+://[^/\s?#]+[^\s?#]+(?:\?[^\s#]*)?(?:#[^\s]*)?", RegexOptions.Compiled);
        private Regex _ip = new Regex(@"(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9])\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9])", RegexOptions.Compiled);

        private string _input3 = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/mariomka/regex-benchmark/652d55810691ad88e1c2292a2646d301d3928903/input-text.txt").Result;

        [Benchmark] public int RegularExpressions_Email() => _email.Matches(_input3).Count;
        [Benchmark] public int RegularExpressions_Uri() => _uri.Matches(_input3).Count;
        [Benchmark] public int RegularExpressions_IP() => _ip.Matches(_input3).Count;

    }
}
