using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.IO;
using Akka.Actor;
using System.Net;

namespace Akka.IO.Samples
{
    class Program
    {
        public static IActorRef server;

        static void Main(string[] args)
        {
            var system = ActorSystem.Create("example");
            var manager = system.Tcp();

            server = system.ActorOf(Props.Create<EchoServer>(() => new EchoServer(34000)));

            Console.ReadLine();
        }
    }

    class EchoServer : UntypedActor
    {
        public EchoServer(int port)
        {
            Context.System.Tcp().Tell(new Tcp.Bind(Self, new IPEndPoint(IPAddress.Any, port)));
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Bound)
            {
                var bound = message as Tcp.Bound;
                Console.WriteLine("Listening on {0}", bound.LocalAddress);
            }
            else if (message is Tcp.Connected)
            {
                var connection = Context.ActorOf(Props.Create(() => new EchoConnection(Sender)));
                Sender.Tell(new Tcp.Register(connection));
            }
            else Unhandled(message);
        }
    }

    class EchoConnection : UntypedActor
    {
        private readonly IActorRef _connection;

        public EchoConnection(IActorRef connection)
        {
            _connection = connection;
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Received)
            {
                var received = message as Tcp.Received;
                if (received.Data.Head == 'x')
                    Context.Stop(Self);
                else
                {
                    Console.WriteLine(Encoding.ASCII.GetString(received.Data.ToArray()));
                    _connection.Tell(Tcp.Write.Create(received.Data));
                }
            }
            else Unhandled(message);
        }
    }
}
