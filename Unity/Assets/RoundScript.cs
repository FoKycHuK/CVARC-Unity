using UnityEngine;
using System.Collections;
using Assets;
using System.Threading;
using System;
using CVARC.V2;
using AIRLab;
using UnityEditor;


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

    // Use this for initialization


    
    void Start()
    {
		timeOnStartSession = Time.fixedTime;
		curWorldTime = 0;
        Behaviour = this;
        CameraCreator();
        ScoresFieldsCreator();
        //Debugger.Log(DebuggerMessageType.Unity,"Started");
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
        //world.Scores.ScoresChanged += UpdateScores;
        CollisionInfo = new Tuple<string, string, int>(null, null, 0);
		Time.timeScale = 1; // вот почему так?

		//в момент повторного запуска время уже не нулевое
		

    }

    void Update()
    {
        //Debugger.Log(DebuggerMessageType.Unity,Time.timeScale);
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


        if (!worldRunning) return;
		
        if (curWorldTime > 10)
        {
			Debugger.Log(DebuggerMessageType.Unity,"Time is Up");
			Dispatcher.SetExpectedExit();
			world.OnExit();
            //((UEngine)world.Engine).Stop();
            //((UEngine)world.Engine).UpdateSpeeds();
            return;
        }
		Dispatcher.CheckNetworkClient();

        if (CollisionInfo.Item3 == 2)
        {
            ((UEngine)world.Engine).CollisionSender(CollisionInfo.Item1, CollisionInfo.Item2);
            CollisionInfo.Item3 = 0;
        }
    }

    void FixedUpdate() //только физика и строгие расчеты. вызывается строго каждые 20 мс
    {
		//Debugger.Log(DebuggerMessageType.Unity,Time.fixedTime);
		curWorldTime = Time.fixedTime - timeOnStartSession;
        world.Clocks.Tick(curWorldTime);
        ((UEngine)world.Engine).UpdateSpeeds();
    }

	void OnDisable()
	{
		Dispatcher.OnDispose();
	}
}