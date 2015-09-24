using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets;
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
    static public bool TestMode = false;
    static readonly List<Thread> Threads = new List<Thread>();
    static PercistentTCPServer server;
    public static readonly Dictionary<string, bool> LastTestExecution = new Dictionary<string, bool>();

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
        Debugger.Logger = s => Debug.Log(s);
        loader = new Loader();
        loader.AddLevel("Demo", "Test", () => new DemoCompetitions.Level1());
        loader.AddLevel("RoboMovies", "Test", () => new RMCompetitions.Level1());

        RenewWaitingNetworkServer();
        //создает PercistentServer и подписываемся на его событие
        server = new PercistentTCPServer(14000);
        server.ClientConnected += ClientConnected;
        server.Printer = str => Debugger.Log(DebuggerMessageType.Unity,"FROM SERVER: " + str);
        RunThread(server.StartThread, "Server");
    }

    //Запускать трэды надо не руками, а через этот метод! Это касается тестов в первую очередь.
    public static void RunThread(Action action, string name)
    {
        var thread = new Thread(new ThreadStart(action)) { IsBackground = true, Name = name };
        Threads.Add(thread);
        thread.Start();
    }

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
    }

    public static void RunOneTest(LoadingData data, string testName)
    {
        Debugger.Log( DebuggerMessageType.Unity, "Starting test "+testName);
        var test = loader.GetTest(data, testName);
        var asserter = new UnityAsserter(testName);
        WaitingNetworkServer.LoadingData = data;
        Action action = () =>
        {
            ExecuteTest(testName, test, asserter);
        };
        RunThread(action, "test thread");
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
                    WaitingNetworkServer.LoadingData = data;
                    var test = loader.GetTest(data, testName);
                    ExecuteTest(testName, test, asserter);
                }
            };
        
        RunThread(runOneTest, "test runner");
    }

    //Подготавливает диспетчер к приему нового клиента.
    static void RenewWaitingNetworkServer()
    {
        WaitingNetworkServer = new NetworkServerData { Port = 14000 };
        WaitingNetworkServer.ServerLoaded = true;
    }

    static void ClientConnected(CvarcClient client)
    {
        RunThread(() =>
            {
                WaitingNetworkServer.ClientOnServerSide = client;
                loader.ReceiveConfiguration(WaitingNetworkServer);
                loadedNetworkServerData = WaitingNetworkServer; // сигнал того, что мир готов к созданию.
                RenewWaitingNetworkServer(); // а это мы делаем, чтобы следующее подключение удалось.
                // создавать его прямо здесь нельзя, потому что другой трэд
            }, "Connection");
    }


    
    // этот метод означает, что можно создавать мир
    public static void WorldPrepared(Func<IWorld> worldInitializer)
    {
        //устанавливаем инициализатор
        WorldInitializer = worldInitializer;
        SetExpectedExit();
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
        Debugger.Log(DebuggerMessageType.Unity,"local exit");
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