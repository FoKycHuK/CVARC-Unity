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
    //public GUIText scoresTextLeftPref;
    //public GUIText scoresTextRightPref;
    GameObject myCamera;
    public GameObject cubePref; // Эти поля -- прототипы, к ним самим обращаться не получится.
    //public GameObject planePref; // Для этого, нужно найти объект в мире каким-либо образом.
    //public GameObject cameraPref; // Например: GameObject.Find(name); написал, чтоб не забыть.
    // А вот так можно получить все объекты в мире, а потом уже выбирать:
    // var allGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];


    // Use this for initialization


    
    void Start()
    {
        Behaviour = this;
        CameraCreator();
        ScoresFieldsCreator();
        //Instantiate(cameraPref, new Vector3(0, 20, 0), Quaternion.Euler(90, 0, 0));
        //Instantiate(planePref, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

        world = CreateDemoWorld();
        //world = CreateDemoWorld();
        world.Scores.ScoresChanged += UpdateScores;
        CollisionInfo = new Tuple<string, string, int>(null, null, 0);
        //scoresTextLeft = Instantiate(scoresTextLeftPref) as GUIText;
        //scoresTextRight = Instantiate(scoresTextRightPref) as GUIText;
        //scoresTextLeft.text = "Left Scores: 0";
        //scoresTextRight.text = "Right Scores: 0";
        //world.Scores.ScoresChanged += UpdateScores;
    }

    void Update()
    {
        if (CollisionInfo.Item3 == 2)
        {
            ((UEngine)world.Engine).CollisionSender(CollisionInfo.Item1, CollisionInfo.Item2);
            CollisionInfo.Item3 = 0;
        }
        //UKeyboard a = new UKeyboard();
        //foreach (string i in a.PressedKeys)
        //    Debug.Log(i);

    }

    bool worldRunning = true;

    void FixedUpdate()
    {
        if (!worldRunning) return;
        if (Time.fixedTime > world.Clocks.TimeLimit)
        {
            worldRunning = false;
            world.OnExit();
            ((UEngine)world.Engine).Stop();
            ((UEngine)world.Engine).UpdateSpeeds();
            return;
        }
        world.Clocks.Tick(Time.fixedTime);
        ((UEngine)world.Engine).UpdateSpeeds();
    }
    
}