using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using CVARC.V2;
using System.Threading;
using System.Net.Sockets;
namespace TechnicalProject
{
    [TestFixture]
    public class Tests
    {
        byte[] message = new byte[] { 1, 2, 3 };

        [Test]
        public void Some()
        {
            //в тестах ексепшены и фейлы в тредах не влияют на результат (тесты зеленые)
            //треды продолжают работу после завершения теста (жму кэнсл)
            //что-то делаю не так?
            var server = new PercistentTCPServer(14000);
            server.ClientConnected += ClientConnected;
            var exceptionHandled = false;
            Exception exception = null;
            var serverThread = new Thread(() => server.StartThread());
            var clientThread = new Thread(() =>
                {
                    try
                    {
                        var client = new TcpClient();
                        client.Connect("127.0.0.1", 14000);
                        Thread.Sleep(50);
                        //client.ReceiveTimeout = 15;
                        //client.Client.ReceiveTimeout = 15;
                        client.Client.Send(message);
                        client.Client.Receive(message);
                        //exceptionHandled = true; //оно доходит до сюда, значит не виснет и значит сервер не закрывает клиент?
                        //попозже разберусь
                    }
                    catch (Exception e)
                    {
                        exceptionHandled = true;
                        exception = e;
                    }
                });
            serverThread.Start();
            Thread.Sleep(10);

            clientThread.Start();
            Thread.Sleep(10);

            if (exceptionHandled)
                throw exception;

            clientThread = new Thread(() => new TcpClient().Connect("127.0.0.1", 14000));
            clientThread.Start();

            Thread.Sleep(100);
            if (!exceptionHandled)
                Assert.Fail();
        }

        public void ClientConnected(CvarcClient client)
        {
            client.WriteLine(message);
            client.ReadLine();
        }
    }
}
