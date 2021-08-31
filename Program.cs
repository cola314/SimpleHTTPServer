using SimpleHTTPServer.HTTP;
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
        static void Main(string[] args)
        {
            new HTTPServer((req, res) =>
            {
                if(req.Path == "/" && req.Method == "GET")
                {
                    res.Headers.TryAdd("Content-Type", "text/plain");
                    res.Body = "Hello World";
                }
                else if(req.Path == "/" && req.Method == "POST")
                {
                    res.Headers.TryAdd("Content-Type", "text/json");
                    res.Body = req.Body;
                }
                else
                {
                    res.StatusCode = 404;
                    res.StatusMessage = "Not Found";
                    res.Headers.TryAdd("Content-Type", "text/html");
                    res.Body = "<h1>404 Not Found</h1>";
                }
            }).Start(80).Wait();
        }
    }
}
