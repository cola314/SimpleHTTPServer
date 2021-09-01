using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHTTPServer.HTTP
{
    public class Response
    {
        public string StatusMessage { get; set; } = "OK";
        public int StatusCode { get; set; } = 200;
        public string ProtocolVersion { get; set; } = "HTTP/1.1";
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public string Body { get; set; }

        public string BuildMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{ProtocolVersion} {StatusCode.ToString()} {StatusMessage}");

            foreach(var header in Headers)
            {
                sb.AppendLine($"{header.Key}: {header.Value}");
            }

            if(Body != null)
            {
                sb.AppendLine($"Content-Length: {Encoding.UTF8.GetByteCount(Body)}")
                    .AppendLine()
                    .AppendLine(Body);
            }

            return sb.ToString();
        }
    }
}
