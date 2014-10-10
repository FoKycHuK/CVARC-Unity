using UnityEngine;
using System.Collections;
using Assets;
using System.Threading;
using System;
using CVARC.V2;
using AIRLab;


public class creater : MonoBehaviour
{
    public static creater Behaviour { get; private set; }
    public static Tuple<string, string, int> CollisionInfo { get; set; }
    IWorld world;
    GUIText scoresTextLeft;
    GUIText scoresTextRight;
    public GUIText scoresTextLeftPref;
    public GUIText scoresTextRightPref;
    public GameObject cubePref; // Эти поля -- прототипы, к ним самим обращаться не получится.
    public GameObject planePref; // Для этого, нужно найти объект в мире каким-либо образом.
    public GameObject cameraPref; // Например: GameObject.Find(name); написал, чтоб не забыть.
    // А вот так можно получить все объекты в мире, а потом уже выбирать:
    // var allGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];


    // Use this for initialization


    IWorld CreateDemoWorld()
    {
        var enginePart = new EnginePart(new UEngine(), new UKeyboard());
        var managerPart = new DemoManagerPart();
        var logicPart = new DemoCompetitions.DemoLogicPart(); //Заменить только эту строчку для перехода на корабль
        var competitions = new Competitions(logicPart, enginePart, managerPart);
        var runMode = RunModes.Available["BotDemo"];
        var cmdArguments = new RunModeArguments();
        cmdArguments.ControllersInfo["Left"] = "Square";
        cmdArguments.ControllersInfo["Right"] = "Random";
        return competitions.Create(cmdArguments, runMode());

    }

    IWorld CreateRTSWorld()
    {
        var enginePart = new EnginePart(new UEngine(), new UKeyboard());
        var managerPart = new RTSManagerPart();
        var logicPart = new RepairTheStarship.RTSLogicPart(); //Заменить только эту строчку для перехода на корабль
        var competitions = new Competitions(logicPart, enginePart, managerPart);
        var runMode = RunModes.Available["BotDemo"];
        var cmdArguments = new RunModeArguments();
        cmdArguments.ControllersInfo["Left"] = "Azura";
        cmdArguments.ControllersInfo["Right"] = "Sanguine";
        return competitions.Create(cmdArguments, runMode());

    }

    void Start()
    {
        Behaviour = this;

        Instantiate(cameraPref, new Vector3(0, 100, 0), Quaternion.Euler(90, 0, 0));
        Instantiate(planePref, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

        world = CreateDemoWorld();
        CollisionInfo = new Tuple<string, string, int>(null, null, 0);
        scoresTextLeft = Instantiate(scoresTextLeftPref) as GUIText;
        scoresTextRight = Instantiate(scoresTextRightPref) as GUIText;
        scoresTextLeft.text = "Left Scores: 0";
        scoresTextRight.text = "Right Scores: 0";
        world.Scores.ScoresChanged += UpdateScores;
        //Instantiate(new GUIText());
    }

    void Update()
    {
        if (CollisionInfo.Item3 == 2)
        {
            ((UEngine)world.Engine).CollisionSender(CollisionInfo.Item1, CollisionInfo.Item2);
            CollisionInfo.Item3 = 0;
        }
        UKeyboard a = new UKeyboard();
        foreach (string i in a.PressedKeys)
            Debug.Log(i);

    }
    void FixedUpdate()
    {
        world.Clocks.Tick(Time.fixedTime);
        ((UEngine)world.Engine).UpdateSpeeds();
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