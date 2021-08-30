using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHTTPServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
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
                            var sb = new StringBuilder();

                            using var stream = tcpClient.GetStream();
                            using var sr = new StreamReader(stream);
                            using var sw = new StreamWriter(stream);

                            while (tcpClient.Connected)
                            {
                                string line = sr.ReadLine();
                                Console.WriteLine(line);
                                sb.AppendLine(line);

                                if(line == String.Empty)
                                {
                                    sw.Write(ProcessClientInput(sb.ToString()));
                                    sw.Flush();
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static string ProcessClientInput(string v)
        {
            return @"HTTP/1.1 200 OK
Date: Mon, 27 Jul 2009 12:28:53 GMT
Server: Apache
Last - Modified: Wed, 22 Jul 2009 19:15:56 GMT
ETag: ""34aa387-d-1568eb00""
Accept - Ranges: bytes
Content - Length: 51
Vary: Accept - Encoding
Content - Type: text / plain

Hello World! My payload includes a trailing CRLF.";
        }
    }
}
