using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalProject
{
    public class PercistentTCPServer
	{
        volatile bool shouldStop = false;
		int port;
		public PercistentTCPServer(int port)
		{
			this.port = port;
		}

		public event Action<CvarcClient> ClientConnected;

		public void StartThread()
		{
			var listner=new TcpListener(IPAddress.Any, port);
            listner.Start();
			CvarcClient cvarcClient = null;
			while(!shouldStop)
			{
				var client = listner.AcceptTcpClient();
				if (cvarcClient != null)
					cvarcClient.Close(); // этот метод должен внутри CvarcClient устанавливать флаг, при котором цикл внутри Read заканчивается исключением
				cvarcClient = new CvarcClient(client);
				if (ClientConnected != null)
					ClientConnected(cvarcClient);
			}
            if (cvarcClient != null)
            {
                cvarcClient.Close();
            }
		}
        public void RequestStop() // КОСТЫЛЬ на закрытие треда.
        {
            shouldStop = true;
        }

		//Тесты:
		//поднимаешь сервер, делаешь обработчик события, который инициирует трэд чтения и читает-пишет
		//пытаешься работать с этим сервером, в том числе, двумя клиентами - проверить, что первый перестает работать, когда подключается второй.
		//проверить, что второй может подключиться после того, как корректно или некорректно отключился первый

	}
}
