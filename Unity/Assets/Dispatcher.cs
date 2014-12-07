using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TechnicalProject;
using UnityEngine;

class Dispatcher
{
	static Loader loader;
	static NetworkServerData loadedNetworkServerData = null;
    static Func<IWorld> WorldInitializer;

	//Этот метод нужно вызвать ровно один раз навсегда!
	public static void Start()
	{
        
		//создание и заполнение loader-а сюда
        loader = new Loader();
        loader.AddLevel("Demo", "Test", () => new DemoCompetitions.Level1());
        loader.AddLevel("RepairTheStarship", "Level1", () => new RepairTheStarship.Level1());

		//создает PercistentServer и подписываемся на его событие
        var server = new PercistentTCPServer(14000);
        server.ClientConnected += ClientConnected;
        new Thread(server.StartThread).Start();
	}

	static void ClientConnected(CvarcClient client)
	{
		//в отдельном трэде делать!
		var networkServerData = new NetworkServerData();
		networkServerData.ClientOnServerSide = client;
		loader.ReceiveConfiguration(networkServerData);
		loadedNetworkServerData = networkServerData; // сигнал того, что мир готов к созданию.
													 // создавать его прямо здесь нельзя, потому что другой трэд
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