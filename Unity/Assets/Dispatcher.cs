using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

class Dispatcher
{
	static Loader loader;
	static NetworkServerData loadedNetworkServerData = null;
	public static NetworkServerData WaitingNetworkServer { get; private set; }
    static Func<IWorld> WorldInitializer;

	//Этот метод нужно вызвать ровно один раз навсегда!
	public static void Start()
	{
        
		//создание и заполнение loader-а сюда
        loader = new Loader();
        loader.AddLevel("Demo", "Test", () => new DemoCompetitions.Level1());

		RenewWaitingNetworkServer();
		//создает PercistentServer и подписываемся на его событие
        var server = new PercistentTCPServer(14000);
        server.ClientConnected += ClientConnected;
		server.Printer = str => Debug.Log(str);
		new Thread(server.StartThread) { IsBackground = true }.Start();
	}

	public static void RunThread(Action thread)
	{
		//их в какой-то список складывать
	}

	//Вызывать этот метод при завершении Unity
	public static void Exit()
	{
		//убивать трэды как угодно
	}

	public static void RunAllTests(LoadingData data)
	{
		var competitions = loader.GetCompetitions(data);
		var testsNames = competitions.Logic.Tests.Keys.ToArray();
		//ты умеешь запускать тесты, см. IntroductionScript
		//и ты знаешь, когда очередной тест закончился, тогда вызывается метод Exited
		//должен появится флаг тестового режима, и в этом режиме Exited должен запускать следующий тест. Если кончились, возвращение на Intro.
	}

	static void RenewWaitingNetworkServer()
	{
		WaitingNetworkServer = new NetworkServerData() { Port = 14000 };
		WaitingNetworkServer.ServerLoaded = true;
	}

	static void ClientConnected(CvarcClient client)
	{
		//в отдельном трэде делать!
		new Action(() =>
			{
				WaitingNetworkServer.ClientOnServerSide = client;
				loader.ReceiveConfiguration(WaitingNetworkServer);
				loadedNetworkServerData = WaitingNetworkServer; // сигнал того, что мир готов к созданию.
				RenewWaitingNetworkServer();
				// создавать его прямо здесь нельзя, потому что другой трэд
			}).BeginInvoke(null, null);
	}


	// этот метод означает, что можно создавать мир
	public static void WorldPrepared(Func<IWorld> worldInitializer)
	{
		//устанавливаем инициализатор
		WorldInitializer = worldInitializer;
		//переключаемся на уровень. Этот уровень в старте позовет метод InitializeWorld ...
		Application.LoadLevel("Round");
	}

	// .. и в этом методе мы завершим инициализацию
	public static IWorld InitializeWorld()
	{
		var world = WorldInitializer();
		world.Exit += Exited;
		return world;
	}


	static void Exited()
	{
		Application.LoadLevel("Intro");
		Debug.Log("Exited");
	}

	//Запускать из Intro по типа таймеру
	public static void CheckNetworkClient()
	{
		if (loadedNetworkServerData != null)
		{
			Func<IWorld> worldInitializer = () =>
			{
				loader.InstantiateWorld(loadedNetworkServerData);
				var world = loadedNetworkServerData.World;
				loadedNetworkServerData = null;
				return world;
			};
			WorldPrepared(worldInitializer);
		}
	}


}