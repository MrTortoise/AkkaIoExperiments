using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Akka.IO.ConsoleAkkaClient
{
    class Program
    {
        public static IActorRef client;
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("example");
            var manager = system.Tcp();

            client = system.ActorOf(Props.Create(() => new TelnetClient("localhost", 34000)));


            while(true)
            {
                Thread.Sleep(100);
            }
        }

        class TelnetClient : UntypedActor
        {
            public TelnetClient(string host, int port)
            {
                var endpoint = new DnsEndPoint(host, port);
                Context.System.Tcp().Tell(new Tcp.Connect(endpoint));
            }

            protected override void OnReceive(object message)
            {
                if (message is Tcp.Connected)
                {
                    var connected = message as Tcp.Connected;
                    Console.WriteLine("Connected to {0}", connected.RemoteAddress);

                    // Register self as connection handler
                    Sender.Tell(new Tcp.Register(Self));
                    ReadConsoleAsync();
                    Become(Connected(Sender));
                }
                else if (message is Tcp.CommandFailed)
                {
                    Console.WriteLine("Connection failed");
                }
                else Unhandled(message);
            }

            private UntypedReceive Connected(IActorRef connection)
            {
                return message =>
                {
                    if (message is Tcp.Received)  // data received from network
                    {
                        var received = message as Tcp.Received;
                        Console.WriteLine("Received from network:" + Encoding.ASCII.GetString(received.Data.ToArray()));
                    }
                    else if (message is string)   // data received from console
                    {
                        connection.Tell(Tcp.Write.Create(ByteString.FromString((string)message + "\n")));
                        ReadConsoleAsync();
                    }
                    else if (message is Tcp.PeerClosed)
                    {
                        Console.WriteLine("Connection closed");
                    }
                    else Unhandled(message);
                };
            }

            private void ReadConsoleAsync()
            {
                Task.Factory.StartNew(self => Console.In.ReadLineAsync().PipeTo((ICanTell)self), Self);
            }
        }
    }
}
