using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleHTTPServer.HTTP
{
    class HTTPServer
    {
        Action<Request, Response> proc_;

        public HTTPServer(Action<Request, Response> proc)
        {
            this.proc_ = proc;
        }

        public async Task Start(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 80);
            listener.Start();

            while (true)
            {
                var tcpClient = await listener.AcceptTcpClientAsync().ConfigureAwait(false);

                _ = Task.Run(() =>
                {
                    try
                    {
                        ProcessTcpClient(tcpClient);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            }
        }

        private void ProcessTcpClient(TcpClient tcpClient)
        {
            var request = new Request();
            var response = new Response();
            var sb = new StringBuilder();

            using var stream = tcpClient.GetStream();
            using var streamReader = new StreamReader(stream);
            using var streamWriter = new StreamWriter(stream);

            var httpParams = streamReader.ReadLine().Split();
            if (httpParams.Length == 3)
            {
                request.Method = httpParams[0];
                request.Path = httpParams[1];
                request.ProtocolVersion = httpParams[2];
            }
            else
            {
                // TODO Send Error Code
            }

            while (tcpClient.Connected)
            {
                string line = streamReader.ReadLine();

                if (line == String.Empty)
                {
                    break;
                }

                int seperatorIndex = line.IndexOf(':');

                if (seperatorIndex == -1)
                {
                    // TODO Send Error Code
                }
                else
                {
                    string fieldName = line.Substring(0, seperatorIndex).Trim();
                    string fieldValue = seperatorIndex == line.Length ?
                        string.Empty : line.Substring(seperatorIndex + 1, line.Length - (seperatorIndex + 1)).Trim();

                    // Duplication of header is UB
                    request.Headers.TryAdd(fieldName, fieldValue);
                }
            }

            if (request.Headers.TryGetValue("Content-Length", out string length) && int.TryParse(length, out int bodyLength))
            {
                char[] buffer = new char[bodyLength];
                int index = 0, remainSize = bodyLength;

                while (remainSize > 0)
                {
                    remainSize -= streamReader.ReadBlock(buffer, index, remainSize);

                    // Sleep for context switching
                    Thread.Sleep(1);
                }

                request.Body = new string(buffer);
            }

            proc_.Invoke(request, response);

            streamWriter.Write(response.BuildMessage());
            streamWriter.Flush();
        }
    }
}
