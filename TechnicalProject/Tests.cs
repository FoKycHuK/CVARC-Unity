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
    //в тестах ексепшены и фейлы в тредах не влияют на результат (тесты зеленые)
    //треды продолжают работу после завершения теста (жму кэнсл)
    //что-то делаю не так?
    //Два теста не хотят запускаться одновременно. Хотя по очереди работают хорошо.
    [TestFixture]
    public class Tests
    {
        

        [Test]
        public void First_client_throws_exception_when_second_connected()
        {
            byte[] message = new byte[] { 1, 2, 3 };
            var server = new PercistentTCPServer(14000);
            server.ClientConnected += ClientConnected;
            var exceptionHandled = false;
            Exception exception = null;
            Exception secondClientException = null;
            var serverThread = new Thread(() => server.StartThread());
            var clientThread = new Thread(() =>
                {
                    try
                    {
                        var client = new TcpClient();
                        client.Connect("127.0.0.1", 14000);
                        Thread.Sleep(30); //ждем, пока другой клиент подсоединится. 
                        client.Client.Send(message); //где-то тут сервер должен разорвать соединение.
                        client.Client.Receive(message);
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
                throw exception; //на этот момент не должно быть эксепшна, первый должен к этому моменту успешно подключиться и ждать в слипе.

            clientThread = new Thread(() => 
                {
                    try
                    {
                        new TcpClient().Connect("127.0.0.1", 14000);
                    }
                    catch (Exception e)
                    {
                        secondClientException = e;
                    }
                });
            clientThread.Start();

            Thread.Sleep(50);

            server.RequestStop(); //lol
            new TcpClient().Connect("127.0.0.1", 14000); // КОСТЫЛЬ на закрытие треда.

            if (secondClientException != null)
                throw secondClientException; //второй не должен кидать эксепшн, а должен успешно подключиться.

            if (!exceptionHandled) // первый должен кинуть эксепшн после подключения второго
                Assert.Fail();

        }

        [Test]
        public void Second_can_connect_when_first_correct_close()
        {
            byte[] message = new byte[] { 1, 2, 3 };
            var exceptionHandled = false;
            Exception exception = null;
            var server = new PercistentTCPServer(14000);
            server.ClientConnected += ClientConnected;

            var serverThread = new Thread(() => server.StartThread());
            serverThread.Start();

            var clientThread = new Thread(() =>
            {
                try
                {
                    var client = new TcpClient();
                    client.Connect("127.0.0.1", 14000);
                    client.Client.Close();
                }
                catch (Exception e)
                {
                    exceptionHandled = true;
                    exception = e;
                }
            });
            clientThread.Start();
            clientThread.Join();

            if (exceptionHandled)
                throw exception;

            clientThread = new Thread(() =>
            {
                try
                {
                    var client = new TcpClient();
                    client.Connect("127.0.0.1", 14000);
                    //client.Client.Send(message); //с этими строчками работает по отдельности, но не работает когда жмешь Ран Олл
                    //client.Client.Receive(message);
                }
                catch (Exception e)
                {
                    exceptionHandled = true;
                    exception = e;
                }
            });
            clientThread.Start();
            clientThread.Join();

            if (exceptionHandled)
                throw exception;

            server.RequestStop();
            new TcpClient().Connect("127.0.0.1", 14000); // КОСТЫЛЬ на закрытие треда.
        }

        [Test]
        public void Second_can_connect_when_first_not_close()
        {
            byte[] message = new byte[] { 1, 2, 3 };
            var exceptionHandled = false;
            Exception exception = null;
            var server = new PercistentTCPServer(14000);
            server.ClientConnected += ClientConnected;

            var serverThread = new Thread(() => server.StartThread());
            serverThread.Start();

            var clientThread = new Thread(() =>
            {
                try
                {
                    var client = new TcpClient();
                    client.Connect("127.0.0.1", 14000);
                }
                catch (Exception e)
                {
                    exceptionHandled = true;
                    exception = e;
                }
            });
            clientThread.Start();
            clientThread.Join();

            if (exceptionHandled)
                throw exception;

            clientThread = new Thread(() =>
            {
                try
                {
                    var client = new TcpClient();
                    client.Connect("127.0.0.1", 14000);
                    //client.Client.Send(message); //с этими строчками работает по отдельности, но не работает когда жмешь Ран Олл
                    //client.Client.Receive(message);
                }
                catch (Exception e)
                {
                    exceptionHandled = true;
                    exception = e;
                }
            });
            clientThread.Start();
            clientThread.Join();

            if (exceptionHandled)
                throw exception;

            server.RequestStop();
            new TcpClient().Connect("127.0.0.1", 14000); // КОСТЫЛЬ на закрытие треда.
        }

        void ClientConnected(CvarcClient client)
        {
            new Thread(() =>
            {
                client.WriteLine(new byte[] { 1, 2, 3 });
                client.ReadLine();
            }).Start();
        }
    }
}
