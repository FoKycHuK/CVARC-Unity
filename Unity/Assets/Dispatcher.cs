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

	static bool ExpectedExit;

	static bool IsRoundScene;

	static public bool TestMode;

	static List<Thread> Threads = new List<Thread>();

	static PercistentTCPServer server;
	//Этот метод нужно вызвать ровно один раз навсегда! для этого завести флаг.
	public static void Start()
	{
		Debugger.Logger = s => Debug.Log("CVARC:" + s);
		//создание и заполнение loader-а сюда
		loader = new Loader();
		loader.AddLevel("Demo", "Test", () => new DemoCompetitions.Level1());

		RenewWaitingNetworkServer();
		//создает PercistentServer и подписываемся на его событие
		server = new PercistentTCPServer(14000);
		server.ClientConnected += ClientConnected;
		server.Printer = str => Debug.Log("FROM SERVER: " + str);
		//new Thread(server.StartThread) { IsBackground = true }.Start();
		RunThread(server.StartThread, "Server");
	}

	//Запускать трэды надо не руками, а через этот метод! Это касается тестов в первую очередь.
	public static void RunThread(Action action, string name)
	{
		var thread = new Thread(new ThreadStart(action)) { IsBackground = true, Name = name };
		Threads.Add(thread);
		thread.Start();
		//их в какой-то список складывать
	}

	//Вызывать этот метод при завершении Unity. Найти метод, который вызывается в Unity перед завершением. 
	public static void KillThreads()
	{
		foreach (var thread in Threads)
		{
			if (thread.IsAlive)
			{
				Debug.Log("THREAD WARN: thread " + thread.Name + " not closed yet. I'll kill it");
				thread.Abort();
			}
		}
		//убивать трэды как угодно
	}

	public static void RunAllTests(LoadingData data)
	{
		var competitions = loader.GetCompetitions(data);
		var testsNames = competitions.Logic.Tests.Keys.ToArray();
		var asserter = new UnityAsserter();
		Action runOneTest = () => 
			{
				foreach(var testName in testsNames)
				{
					Debug.Log("Test is ready");
					Thread.Sleep(500);
					Dispatcher.WaitingNetworkServer.LoadingData = data;
					var test = IntroductionStript.loader.GetTest(data, testName);
					test.Run(WaitingNetworkServer, asserter);
					//while (IsRoundScene	)					Thread.Sleep(1);
				}
			};
		
		RunThread(runOneTest, "test runner");
		//ты умеешь запускать тесты, см. IntroductionScript
		//и ты знаешь, когда очередной тест закончился, тогда вызывается метод Exited
		//должен появится флаг тестового режима, и в этом режиме Exited должен запускать следующий тест. Если кончились, возвращение на Intro.

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
			}), "Connection");//.BeginInvoke(null, null);
	}


	
	// этот метод означает, что можно создавать мир
	public static void WorldPrepared(Func<IWorld> worldInitializer)
	{
		//устанавливаем инициализатор
		WorldInitializer = worldInitializer;
		Dispatcher.SetExpectedExit();
		//переключаемся на уровень. Этот уровень в старте позовет метод InitializeWorld ...
		IsRoundScene = true; 
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
			//EditorApplication.isPlaying = false; // так мы "отжимаем" кнопку play. при этом скрипты продолжают выполняться, но юнити сцены закрываются и вызывается метод OnDisable
		IsRoundScene = false;
		Application.LoadLevel("Intro"); //может не надо этого?
		Debug.Log("local exit");
		//KillThreads(); // я не уверен, что это должно быть тут, но походу этого больше нигде нет.
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

	public static void OnDispose()
	{
		if (!ExpectedExit)
		{
			server.RequestExit();
			Thread.Sleep(100);
			KillThreads();
			Debug.Log("GLOBAL exit");
		}
		else
			ExpectedExit = false;
	}

	public static void SetExpectedExit()
	{
		ExpectedExit = true;
	}

}