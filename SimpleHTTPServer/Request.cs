using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHTTPServer.HTTP
{
    public class Request
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string ProtocolVersion { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string Body { get; set; }
    }
}
