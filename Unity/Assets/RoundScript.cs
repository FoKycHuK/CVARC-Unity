using UnityEngine;
using Assets;
using System;
using CVARC.V2;
using AIRLab;


public partial class RoundScript : MonoBehaviour
{
    public static RoundScript Behaviour { get; private set; }
    public static Tuple<string, string, int> CollisionInfo { get; set; }
    IWorld world;
    GUIText scoresTextLeft;
    GUIText scoresTextRight;
    GameObject myCamera;
    public GameObject cubePref; // Эти поля -- прототипы, к ним самим обращаться не получится.
    bool worldRunning = true;
    bool worldPrepearedToExit;
    float curWorldTime;
    float timeOnStartSession;
    private long lastStart;


    void Start()
    {
        timeOnStartSession = Time.fixedTime;
        curWorldTime = 0;
        Behaviour = this;
        CameraCreator();
        ScoresFieldsCreator();
        try
        {
            world = Dispatcher.InitializeWorld();
            Debugger.Log(DebuggerMessageType.Unity,"World loaded");
        }
        catch(Exception e)
        {
            Debugger.Log(DebuggerMessageType.Unity,"Fail");
            Debugger.Log(DebuggerMessageType.Unity,e.Message);
        }
        CollisionInfo = new Tuple<string, string, int>(null, null, 0);
        Time.timeScale = 1; // вот почему так?
        //в момент повторного запуска время уже не нулевое
    }

    void Update()
    {
        if (!worldRunning) return;
        
        if (curWorldTime > 30)
        {
            Debugger.Log(DebuggerMessageType.Unity,"Time is Up");
            Dispatcher.SetExpectedExit();
            world.OnExit();
            return;
        }
        Dispatcher.CheckNetworkClient();

        if (CollisionInfo.Item3 == 2)
        {
            ((UEngine)world.Engine).CollisionSender(CollisionInfo.Item1, CollisionInfo.Item2);
            CollisionInfo.Item3 = 0;
        }

        UpdateScores();
    }

    void FixedUpdate() //только физика и строгие расчеты. вызывается строго каждые 20 мс
    {
        curWorldTime = Time.fixedTime - timeOnStartSession;
        world.Clocks.Tick(curWorldTime);
        ((UEngine)world.Engine).UpdateSpeeds();
    }

    void OnDisable()
    {
        Dispatcher.OnDispose();
    }
}