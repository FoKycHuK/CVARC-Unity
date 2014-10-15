using Assets;
using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public partial class creater : MonoBehaviour
{
    IWorld CreateDemoWorld()
    {
        var enginePart = new EnginePart(new UEngine(), new UKeyboard());
        var managerPart = new DemoManagerPart();
        var logicPart = new DemoCompetitions.DemoLogicPart(); //Заменить только эту строчку для перехода на корабль
        var competitions = new Competitions(logicPart, enginePart, managerPart);

        //var runMode = RunModeFactory.Create(RunModes.BotDemo);
        //var cmdArguments = new Configuration();
        //cmdArguments.Controllers.Add(new ControllerConfiguration { ControllerId = "Left", Name = "Square", Type = ControllerType.Bot });
        //cmdArguments.Controllers.Add(new ControllerConfiguration { ControllerId = "Right", Name = "Square", Type = ControllerType.Bot });
        //cmdArguments.EnableLog = true;
        //cmdArguments.TimeLimit = 10;

        IRunMode runMode = null;
        runMode = Competitions.CreateMode("Demo", "Level1", "BotDemo", "-TimeLimit", "10", "-EnableLog", "-Controller.Left", "Bot.Square", "-Controller.Right", "Bot.Square");
        //runMode = Competitions.CreateMode("log0.cvarclog");

        //var runMode = RunModeFactory.Create(RunModes.Play);
        //var cmdArguments = new Configuration();
        //cmdArguments.LogFile = "log0.cvarclog";
        return competitions.Create(runMode.Configuration, runMode);

    }

    IWorld CreateRTSWorld()
    {
        var enginePart = new EnginePart(new UEngine(), new UKeyboard());
        var managerPart = new RTSManagerPart();
        var logicPart = new RepairTheStarship.RTSLogicPart(); //Заменить только эту строчку для перехода на корабль
        var competitions = new Competitions(logicPart, enginePart, managerPart);
        var runMode = RunModeFactory.Create(RunModes.BotDemo);
        var cmdArguments = new Configuration();
        cmdArguments.Controllers.Add(new ControllerConfiguration { ControllerId = "Left", Name = "Azura", Type = ControllerType.Bot });
        cmdArguments.Controllers.Add(new ControllerConfiguration { ControllerId = "Right", Name = "Sanguine", Type = ControllerType.Bot });
        runMode.CheckArguments(cmdArguments);
        return competitions.Create(cmdArguments, runMode);

    }

    IWorld CreateTutorialWorld()
    {
        var enginePart = new EnginePart(new UEngine(), new UKeyboard());
        var managerPart = new RTSManagerPart();
        var logicPart = new RepairTheStarship.RTSLogicPart(); //Заменить только эту строчку для перехода на корабль
        var competitions = new Competitions(logicPart, enginePart, managerPart);
        var runMode = RunModeFactory.Create(RunModes.Tutorial);
        var cmdArguments = new Configuration();
        runMode.CheckArguments(cmdArguments);
        return competitions.Create(cmdArguments, runMode);


    }

    void CameraCreator()
    {
        myCamera = new GameObject("myCamera");
        myCamera.AddComponent<Camera>();
        myCamera.AddComponent<GUILayer>();
        myCamera.AddComponent<AudioListener>();
        myCamera.transform.position = new Vector3(0, 100, 0);
        myCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    void ScoresFieldsCreator()
    {
        scoresTextLeft = new GameObject("LeftScoreText").AddComponent<GUIText>() as GUIText;
        scoresTextLeft.pixelOffset = new Vector2(1, 1);
        scoresTextLeft.text = "Left Scores: 0";
        scoresTextLeft.transform.position = new Vector3(0, 1, 0);
        scoresTextRight = new GameObject("RightScoreText").AddComponent<GUIText>() as GUIText;
        scoresTextRight.pixelOffset = new Vector2(2, 2);
        scoresTextRight.text = "Right Scores: 0";
        scoresTextRight.transform.position = new Vector3(0.88f, 1, 0);
    }
    void UpdateScores()
    {
        foreach (var player in world.Scores.GetAllScores())
        {
            if (player.Item1 == "Left")
                scoresTextLeft.text = "Left Scores: " + player.Item2;
            if (player.Item1 == "Right")
                scoresTextRight.text = "Right Scores: " + player.Item2;
        }
    }
}

