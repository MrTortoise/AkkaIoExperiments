using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Akka.IO.ConsoleTextClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 34000);

            Socket sender = new Socket(AddressFamily.InterNetwork,
               SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(remoteEP);

            Console.WriteLine("Socket connected to {0}",
                sender.RemoteEndPoint.ToString());

            while(true)
            {
                var input = Console.ReadLine();
                var encodedInput = Encoding.UTF8.GetBytes(input);

                int bytesSent = sender.Send(encodedInput);
                var inputString = new ArraySegment<byte>(encodedInput, 0, encodedInput.Length);  
   
          
                //var receiveTasl = client.ReceiveAsync(received,new CancellationToken());

                //receiveTasl.ContinueWith(res =>
                //{
                //    if (res.IsCompleted)
                //    {
                //        Console.WriteLine(Encoding.UTF8.GetChars(received.Array));
                //    }
                    

                //    if (res.IsFaulted)
                //    {
                //        Console.WriteLine("Bang");
                //        Console.WriteLine(res.Exception.Flatten().ToString());
                //    }
                //});
            }
            

        }
    }
}
