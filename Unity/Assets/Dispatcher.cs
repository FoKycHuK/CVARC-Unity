﻿using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;
using RoboMovies;

class Dispatcher
{
	public static Loader loader { get; private set; }

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
        Debugger.DisableByDefault = true;
        Debugger.EnabledTypes.Add(DebuggerMessageType.Unity);
        Debugger.EnabledTypes.Add(DebuggerMessageType.UnityTest);
        Debugger.EnabledTypes.Add(RMDebugMessage.WorldCreation);
        Debugger.EnabledTypes.Add(RMDebugMessage.Logic);
        Debugger.EnabledTypes.Add(RMDebugMessage.Workflow);
        Debugger.EnabledTypes.Add(DebuggerMessageType.Workflow);
        // Debugger.EnabledTypes.Add(DebuggerMessageType.Initialization);
		Debugger.Logger = s => Debug.Log(s);
		//создание и заполнение loader-а сюда
		loader = new Loader();
		loader.AddLevel("Demo", "Test", () => new DemoCompetitions.Level1());
        loader.AddLevel("RoboMovies", "Test", () => new RMCompetitions.Level1());

		RenewWaitingNetworkServer();
		//создает PercistentServer и подписываемся на его событие
		server = new PercistentTCPServer(14000);
		server.ClientConnected += ClientConnected;
		server.Printer = str => Debugger.Log(DebuggerMessageType.Unity,"FROM SERVER: " + str);
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
				Debugger.Log(DebuggerMessageType.Unity, "THREAD WARN: thread " + thread.Name + " not closed yet. I'll kill it");
				thread.Abort();
			}
		}
		//убивать трэды как угодно
	}

    public static void RunOneTest(LoadingData data, string testName)
    {
        Debugger.Log( DebuggerMessageType.Unity, "Starting test "+testName);
        var competitionsInstance = loader.GetCompetitions(data);
        var test = loader.GetTest(data, testName);
        var asserter = new UnityAsserter(testName);
        Dispatcher.WaitingNetworkServer.LoadingData = data;
        Action action = () =>
        {
            ExecuteTest(testName, test, asserter);
        };
        Dispatcher.RunThread(action, "test thread");
    }

    static void ExecuteTest(string testName, ICvarcTest test, UnityAsserter asserter)
    {
        try
        {
            test.Run(WaitingNetworkServer, asserter);
        }
        catch (Exception e)
        {
            asserter.Fail(e.GetType().Name + " " + e.Message);
        }
        asserter.DebugOkMessage();
        lock (LastTestExecution)
        {
            LastTestExecution[testName] = !asserter.Failed;
        }
    }

    public static readonly Dictionary<string, bool> LastTestExecution = new Dictionary<string, bool>();

	public static void RunAllTests(LoadingData data)
	{
		var competitions = loader.GetCompetitions(data);
		var testsNames = competitions.Logic.Tests.Keys.ToArray();
		
		Action runOneTest = () => 
			{
                Debugger.Log(DebuggerMessageType.Unity, "staring tests");
				foreach(var testName in testsNames)
				{
                    var asserter = new UnityAsserter(testName);
					Debugger.Log(DebuggerMessageType.Unity,"Test is ready");
					Dispatcher.WaitingNetworkServer.LoadingData = data;
					var test = loader.GetTest(data, testName);
                    ExecuteTest(testName, test, asserter);
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
		Debugger.Log(DebuggerMessageType.Unity,"local exit");
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
			Debugger.Log(DebuggerMessageType.Unity,"GLOBAL exit");
		}
		else
			ExpectedExit = false;
	}

	public static void SetExpectedExit()
	{
		ExpectedExit = true;
	}

}