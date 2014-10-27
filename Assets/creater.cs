using UnityEngine;
using System.Collections;
using Assets;
using System.Threading;
using System;
using CVARC.V2;
using AIRLab;


public partial class creater : MonoBehaviour
{
    public static creater Behaviour { get; private set; }
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
        world = CreateDemoWorld();
        world.Scores.ScoresChanged += UpdateScores;
        CollisionInfo = new Tuple<string, string, int>(null, null, 0);

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
        world.Clocks.Tick(Time.fixedTime);
        ((UEngine)world.Engine).UpdateSpeeds();
    }
    
}