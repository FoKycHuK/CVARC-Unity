﻿using CVARC.V2;
using CVARC.V2.SimpleMovement;
using Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BullShi : ICvarcTest
{
	bool RobotIsRectangular;
	bool ObjectsOnField;

	public SettingsProposal GetSettings()
	{
		return new SettingsProposal
		{
			TimeLimit = 10,
			Controllers = new List<ControllerSettings> 
                    {
                        new ControllerSettings  { ControllerId=MovementLogicPart.ControllerId, Name="This", Type= ControllerType.Client}
                    }
		};
	}
	public MovementWorldState GetWorldState()
	{
		return new MovementWorldState()
		{
			RectangularRobot = RobotIsRectangular,
			objects = ObjectsOnField
		};
	}


	public void Run(NetworkServerData holder, IAsserter asserter)
	{
		var client = new CvarcClient<SensorsData, SimpleMovementCommand>();
		Debug.Log("1");
		var configurationProposal = new ConfigurationProposal
		{
			LoadingData = holder.LoadingData,
			SettingsProposal = GetSettings()
		};
		client.Configurate(holder.Port, configurationProposal, GetWorldState());
		Debug.Log("2");
		var iworld = holder.WaitForWorld();
		var world = Compatibility.Check<MovementWorld>(this, iworld);
		Debug.Log("3");
		//Test(client, world, asserter);
		var test = LocationTest(10, 0, 0, SimpleMovementCommand.Move(10, 1));
		Debug.Log("4");
		try
		{
			test(client, world, asserter);
		}
		catch (Exception e) { Debug.Log(e.Message); }
		client.Exit();
	}

	MovementTestEntry LocationTest(double X, double Y, double angleInGrad, params SimpleMovementCommand[] command)
	{
		return (client, world, asserter) =>
		{
			SensorsData data = null;
			Debug.Log("5");
			foreach (var c in command)
				data = client.Act(c);
			Debug.Log("5.5");
			asserter.IsEqual(X, data.Locations[0].X, 1e-3);
			Debug.Log("6");
			asserter.IsEqual(Y, data.Locations[0].Y, 1e-3);
			Debug.Log("6.5");
			asserter.IsEqual(angleInGrad, data.Locations[0].Angle % 360, 5e-3);
			Debug.Log((data.Locations[0].Angle % 360) > 5e-3);
		};
	}
}