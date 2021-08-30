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
                            const int BUFFER_SIZE = 1024;
                            byte[] buffer = new byte[BUFFER_SIZE];

                            using var stream = tcpClient.GetStream();
                            using var sr = new StreamReader(stream);
                            using var sw = new StreamWriter(stream);

                            while (tcpClient.Connected)
                            {
                                string line = sr.ReadLine();
                                Console.WriteLine(line);
                                sw.WriteLine(line);
                                sw.Flush();
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
    }
}
