using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

class Dispatcher
{
	static Loader loader;
	
	//Данные уже установленного соединения
	static NetworkServerData loadedNetworkServerData = null;

	//Данные соединения, которое еще не установлено. 
	public static NetworkServerData WaitingNetworkServer { get; private set; }

	//Делегат, который запустит мир, определенный очередным клиентом.
    static Func<IWorld> WorldInitializer;

	static public bool TestMode;

	static List<Thread> Threads = new List<Thread>();
	//Этот метод нужно вызвать ровно один раз навсегда! для этого завести флаг.
	public static void Start()
	{
        
		//создание и заполнение loader-а сюда
        loader = new Loader();
        loader.AddLevel("Demo", "Test", () => new DemoCompetitions.Level1());

		RenewWaitingNetworkServer();
		//создает PercistentServer и подписываемся на его событие
        var server = new PercistentTCPServer(14000);
        server.ClientConnected += ClientConnected;
		server.Printer = str => Debug.Log("FROM SERVER: " + str);
		//new Thread(server.StartThread) { IsBackground = true }.Start();
		RunThread(server.StartThread);
	}

	//Запускать трэды надо не руками, а через этот метод! Это касается тестов в первую очередь.
	public static void RunThread(Action action)
	{
		var thread = new Thread(new ThreadStart(action)) { IsBackground = true };
		Threads.Add(thread);
		thread.Start();
		//их в какой-то список складывать
	}

	//Вызывать этот метод при завершении Unity. Найти метод, который вызывается в Unity перед завершением. 
	public static void KillThreads()
	{
		foreach (var thread in Threads)
		{
			Debug.Log(thread.IsAlive);
			thread.Abort();
			Debug.Log("one of thread kill success");
		}
		Debug.Log("all threads killed");
		//убивать трэды как угодно
	}

	public static void RunAllTests(LoadingData data)
	{
		var competitions = loader.GetCompetitions(data);
		var testsNames = competitions.Logic.Tests.Keys.ToArray();
		//ты умеешь запускать тесты, см. IntroductionScript
		//и ты знаешь, когда очередной тест закончился, тогда вызывается метод Exited
		//должен появится флаг тестового режима, и в этом режиме Exited должен запускать следующий тест. Если кончились, возвращение на Intro.

		var asserter = new UnityAsserter();
	}

	//Подготавливает диспетчер к приему нового клиента.
	static void RenewWaitingNetworkServer()
	{
		WaitingNetworkServer = new NetworkServerData() { Port = 14000 };
		WaitingNetworkServer.ServerLoaded = true;
	}

	static void ClientConnected(CvarcClient client)
	{
		//в отдельном трэде делать!
		RunThread(
		new Action(() =>
			{
				WaitingNetworkServer.ClientOnServerSide = client;
				loader.ReceiveConfiguration(WaitingNetworkServer);
				loadedNetworkServerData = WaitingNetworkServer; // сигнал того, что мир готов к созданию.
				RenewWaitingNetworkServer(); // а это мы делаем, чтобы следующее подключение удалось.
				// создавать его прямо здесь нельзя, потому что другой трэд
			}));//.BeginInvoke(null, null);
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
		if (!TestMode)
		{
			EditorApplication.isPlaying = false; // так мы "отжимаем" кнопку play. при этом скрипты продолжают выполняться, но юнити сцены закрываются и вызывается метод OnDisable
			//Application.LoadLevel("Intro"); //может не надо этого?
		}
		Debug.Log("Exited");
		KillThreads(); // я не уверен, что это должно быть тут, но походу этого больше нигде нет.
	}

	//Запускать из Intro по типа таймеру
	public static void CheckNetworkClient()
	{
		
		if (loadedNetworkServerData != null)
		{
			Debug.Log("WOW! Someone connected?");
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