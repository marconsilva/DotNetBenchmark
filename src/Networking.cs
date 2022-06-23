using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class Networking
    {

        [GlobalSetup]
        public void Setup()
        {
            SetupEscape();
            SetupSendRecieve(); 
            CreateSocketServer();        }

        public void CreateSocketServer()
        {
            s_listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            s_listener.Listen(int.MaxValue);
            var ep = (IPEndPoint)s_listener.LocalEndPoint;
            s_uri = new Uri($"http://{ep.Address}:{ep.Port}/");
            byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\nDate: Sun, 05 Jul 2020 12:00:00 GMT \r\nServer: Example\r\nContent-Length: 5\r\n\r\nHello");
            byte[] endSequence = new byte[] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' };

            Task.Run(async () =>
            {
                while (true)
                {
                    Socket s = await s_listener.AcceptAsync();
                    _ = Task.Run(() =>
                    {
                        using (var ns = new NetworkStream(s, true))
                        {
                            byte[] buffer = new byte[1024];
                            int totalRead = 0;
                            while (true)
                            {
                                int read = ns.Read(buffer, totalRead, buffer.Length - totalRead);
                                if (read == 0) return;
                                totalRead += read;
                                if (buffer.AsSpan(0, totalRead).IndexOf(endSequence) == -1)
                                {
                                    if (totalRead == buffer.Length) Array.Resize(ref buffer, buffer.Length * 2);
                                    continue;
                                }

                                ns.Write(response, 0, response.Length);

                                totalRead = 0;
                            }
                        }
                    });
                }
            });
        }
        public void SetupSendRecieve()
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            _listener.Listen(1);

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _client.Connect(_listener.LocalEndPoint);

            _server = _listener.Accept();

            for (int i = 0; i < _buffer.Length; i++)
                _buffers.Add(new ArraySegment<byte>(_buffer, i, 1));
        }

        public void SetupEscape()
        {
            _input = ASCII ?
                new string('s', 20_000) :
                string.Concat(Enumerable.Repeat("\xD83D\xDE00", 10_000));
        }


        [Benchmark]
        public Uri Networking_CreateUri() => new Uri("https://github.com/dotnet/runtime/pull/36915");

        private Uri _uri = new Uri("http://github.com/dotnet/runtime");

        [Benchmark]
        public string Networking_PathAndQuery() => _uri.PathAndQuery;

        [Params(false, true)]
        public bool ASCII { get; set; }

        

        private string _input;

        [Benchmark] public string Networking_Escape() => Uri.EscapeDataString(_input);

        private string _value = string.Concat(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 20));

        [Benchmark]
        public string Networking_Unescape() => Uri.UnescapeDataString(_value);

        private Uri[] _uris = Enumerable.Range(0, 1000).Select(i => new Uri($"/some/relative/path?ID={i}", UriKind.Relative)).ToArray();

        [Benchmark]
        public int Networking_SumHashCodes()
        {
            int sum = 0;

            foreach (Uri uri in _uris)
                sum += uri.GetHashCode();

            return sum;
        }

        private Socket _listener, _client, _server;
        private byte[] _buffer = new byte[8];
        private List<ArraySegment<byte>> _buffers = new List<ArraySegment<byte>>();

        

        [Benchmark]
        public async Task Networking_SocketSendReceive()
        {
            await _client.SendAsync(_buffers, SocketFlags.None);
            int total = 0;
            while (total < _buffer.Length)
                total += await _server.ReceiveAsync(_buffers, SocketFlags.None);
        }


        private static readonly Socket s_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly HttpClient s_client = new HttpClient();
        private static Uri s_uri;

        [Benchmark]
        public async Task Networking_HttpGet()
        {
            var m = new HttpRequestMessage(HttpMethod.Get, s_uri);
            m.Headers.TryAddWithoutValidation("Authorization", "ANYTHING SOMEKEY");
            m.Headers.TryAddWithoutValidation("Referer", "http://someuri.com");
            m.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36");
            m.Headers.TryAddWithoutValidation("Host", "www.somehost.com");
            using (HttpResponseMessage r = await s_client.SendAsync(m, HttpCompletionOption.ResponseHeadersRead))
            using (Stream s = await r.Content.ReadAsStreamAsync())
                await s.CopyToAsync(Stream.Null);
        }

        

        [Benchmark]
        public DateTimeOffset? Networking_DatePreferred()
        {
            var m = new HttpResponseMessage();
            m.Headers.TryAddWithoutValidation("Date", "Sun, 06 Nov 1994 08:49:37 GMT");
            return m.Headers.Date;
        }


        //private byte[] _NegotiateStreambuffer = new byte[1];
        //private NegotiateStream _NegotiateStreamclient, _NegotiateStreamserver;

        //[GlobalSetup]
        //public void NegotiateStreamSetup()
        //{
        //    using var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        //    listener.Listen(1);

        //    var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    client.Connect(listener.LocalEndPoint);

        //    Socket server = listener.Accept();

        //    _NegotiateStreamclient = new NegotiateStream(new NetworkStream(client, true));
        //    _NegotiateStreamserver = new NegotiateStream(new NetworkStream(server, true));

        //    Task.WaitAll(
        //        _NegotiateStreamclient.AuthenticateAsClientAsync(),
        //        _NegotiateStreamserver.AuthenticateAsServerAsync());
        //}

        //[Benchmark]
        //public async Task Networking_NegotiateStreamWriteRead()
        //{
        //    for (int i = 0; i < 100; i++)
        //    {
        //        await _NegotiateStreamclient.WriteAsync(_NegotiateStreambuffer);
        //        await _NegotiateStreamserver.ReadAsync(_NegotiateStreambuffer);
        //    }
        //}

        //[Benchmark]
        //public async Task Networking_NegotiateStreamReadWrite()
        //{
        //    for (int i = 0; i < 100; i++)
        //    {
        //        var r = _NegotiateStreamserver.ReadAsync(_buffer);
        //        await _NegotiateStreamclient.WriteAsync(_buffer);
        //        await r;
        //    }
        //}
    }
}
