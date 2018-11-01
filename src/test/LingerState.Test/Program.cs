using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LingerState.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var ipAddress = IPAddress.Parse("192.168.0.24");
            var port = 8102;

            var tcpClient = new TcpClient
            {
                LingerState = new LingerOption(true, 0)
            };

            await tcpClient.ConnectAsync(ipAddress, port);

            Console.WriteLine("Sending something");
            await SendSomething(tcpClient);

            Console.WriteLine("Closing connection");
            tcpClient.Close();
            Console.WriteLine("Connection closed");

            await Task.Delay(TimeSpan.FromSeconds(360));

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }

        private static async Task SendSomething(TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();

            var wakeup = Encoding.UTF8.GetBytes("\r");

            var command = Encoding.UTF8.GetBytes("\r?P\r");
            
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            
            if (stream?.CanWrite ?? false)
            {
                await stream.WriteAsync(wakeup);
                await Task.Delay(TimeSpan.FromMilliseconds(200));
                await stream.WriteAsync(command);
                await stream.FlushAsync();
            }
        }
    }
}
