using UnityEngine;
using System.Collections;
using Assets;
using System.Threading;
using System;
using CVARC.V2;
using AIRLab;
using System.Net.Sockets;
using CVARC.V2.SimpleMovement;
using Demo;

public partial class creater : MonoBehaviour
{
    public static creater Behaviour { get; private set; }

    public static Tuple<string, string, int> CollisionInfo { get; set; }

    GUIText scoresTextLeft;
    GUIText scoresTextRight;
    GameObject myCamera;
    public GameObject cubePref; // Эти поля -- прототипы, к ним самим обращаться не получится.
    bool worldRunning = true;

    // Use this for initialization


    void ClientThread()
    {

        var socket = new TcpClient();
        socket.Connect("127.0.0.1", 14000);
        var proposal = new ConfigurationProposal();
        proposal.LoadingData.AssemblyName = "Demo";
        proposal.LoadingData.Level = "Level1";
        proposal.SettingsProposal = new SettingsProposal();
        proposal.SettingsProposal.Controllers = new System.Collections.Generic.List<ControllerSettings>();
        proposal.SettingsProposal.Controllers.Add(new ControllerSettings { ControllerId = "Left", Type = ControllerType.Client, Name = "This" });
        proposal.SettingsProposal.Controllers.Add(new ControllerSettings { ControllerId = "Right", Type = ControllerType.Bot, Name = "Random" });


        var client = new CvarcTcpClient(socket);
        client.SerializeAndSend(proposal);
        client.ReadObject<SensorsData>();
    }

    IWorld world;

    void ServerThread()
    {
        world = CreateDebug();
        world.Scores.ScoresChanged += UpdateScores;
        
    }

    void Start()
    {
        new Thread(ClientThread).Start();
        ServerThread();

        Behaviour = this;
        CameraCreator();
        ScoresFieldsCreator();
        CollisionInfo = new Tuple<string, string, int>(null, null, 0);
    }

   
    void Update()
    {
        if (world == null) return;

        //Debug.Log(Time.timeScale);
        //timer++;
        //if (timer == 100)
        //{
        //    Time.timeScale = 10;
        //}
        //if (timer == 200)
        //{
        //    Time.timeScale = 0.5f;
        //}
        //ТОЛЬКО В АПДЕЙТЕ (НЕ В ФИКСЕД_АПДЕЙТЕ)
        //напомнить, > чтобы я глянул, что передается в таймер


        if (!worldRunning)
            return;
        if (Time.fixedTime > world.Clocks.TimeLimit)
        {
            worldRunning = false;
            world.OnExit();
            Time.timeScale = 0;
            //((UEngine)world.Engine).Stop();
            //((UEngine)world.Engine).UpdateSpeeds();
            return;
        }


        if (CollisionInfo.Item3 == 2)
        {
            ((UEngine)world.Engine).CollisionSender(CollisionInfo.Item1, CollisionInfo.Item2);
            CollisionInfo.Item3 = 0;
        }
    }

    void FixedUpdate() //только физика и строгие расчеты. вызывается строго каждые 20 мс
    {
        if (world == null) return;

        world.Clocks.Tick(Time.fixedTime);
        ((UEngine)world.Engine).UpdateSpeeds();
    }

}