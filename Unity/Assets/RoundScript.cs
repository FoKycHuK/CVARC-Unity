using UnityEngine;
using System.Collections;
using Assets;
using System.Threading;
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

    // Use this for initialization


    
    void Start()
    {
        Behaviour = this;
        CameraCreator();
        ScoresFieldsCreator();
        Debug.Log("Started");
        try
        {
            world = Dispatcher.InitializeWorld();
            Debug.Log("Loaded");
        }
        catch(Exception e)
        {
            Debug.Log("Fail");
            Debug.Log(e.Message);
        }
        world.Scores.ScoresChanged += UpdateScores;
        CollisionInfo = new Tuple<string, string, int>(null, null, 0);
		Time.timeScale = 1; // вот почему так?

		//в момент повторного запуска время уже не нулевое
		

    }

    void Update()
    {
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


        if (!worldRunning) return;
		
        if (Time.fixedTime > 10)
        {
			Debug.Log("Time is Up");
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
		//Debug.Log(Time.fixedTime);
        world.Clocks.Tick(Time.fixedTime);
        ((UEngine)world.Engine).UpdateSpeeds();
    }
    
}