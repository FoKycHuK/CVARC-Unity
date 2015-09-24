using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CVARC.V2;

namespace Assets
{
    public class PercistentTCPServer
    {
        int port;
        bool exitRequest = false;
        public event Action<CvarcClient> ClientConnected;
        public Action<string> Printer;

        public PercistentTCPServer(int port)
        {
            this.port = port;
        }

        void Print(string str)
        {
            if (Printer != null)
                Printer(str);
        }

        public void RequestExit()
        {
            exitRequest = true;
        }

        public void StartThread()
        {
            Print("Server started");
            var listner = new TcpListener(IPAddress.Any, port);
            listner.Start();
            CvarcClient cvarcClient = null;
            while (true)
            {
                while (!listner.Pending())
                {
                    if (exitRequest)
                    {
                        if (cvarcClient != null)
                            cvarcClient.Close();
                        listner.Stop();
                        return;
                    }
                    Thread.Sleep(1);
                }
                var client = listner.AcceptTcpClient();
                Print("Client accepted");
                if (cvarcClient != null)
                    cvarcClient.Close(); // этот метод должен внутри CvarcClient устанавливать флаг, при котором цикл внутри Read заканчивается исключением
                cvarcClient = new CvarcClient(client);
                if (ClientConnected != null)
                    ClientConnected(cvarcClient);
            }
        }
    }
}
