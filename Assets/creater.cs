using UnityEngine;
using System.Collections;
using Assets;
using System.Threading;
using System;
using CVARC.V2;
public class creater : MonoBehaviour 
{
    public static creater Behaviour { get; private set; }
    public double myTime;
    IWorld world;
    public GameObject cubePref; // Эти поля -- прототипы, к ним самим обращаться не получится.
    public GameObject planePref; // Для этого, нужно найти объект в мире каким-либо образом.
    public GameObject cameraPref; // Например: GameObject.Find(name); написал, чтоб не забыть.
    // А вот так можно получить все объекты в мире, а потом уже выбирать:
    // var allGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
    // т.к. юнити сама создает кучу непонятных лишних объектов в мире, пусть будет так:
    // Имя (gameObject.name) любого объекта, который будет нужен в методе GetAllObjects будет иметь такой формат:
    // %id%:%type%:CVARC_obj

	// Use this for initialization
	void Start () 
    {
        Behaviour = this;
        Instantiate(cameraPref, new Vector3(0, 100, 0), Quaternion.Euler(90, 0, 0));
        Instantiate(planePref, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        
       

        #region

        var enginePart = new EnginePart(new UEngine(), new UKeyboard());
        var managerPart = new RTSManagerPart();
        var logicPart = new DemoCompetitions.DemoLogicPart(); //Заменить только эту строчку для перехода на корабль
        var competitions = new Competitions(logicPart, enginePart, managerPart);
        var runMode = RunModes.Available["BotDemo"];
        var cmdArguments = new RunModeArguments();
        cmdArguments.ControllersInfo["Left"] = "Square";
        cmdArguments.ControllersInfo["Right"] = "Random";
        world=competitions.Create(cmdArguments, runMode());

        #endregion

        watch=  new System.Diagnostics.Stopwatch();
        watch.Start();
        //foreach (var value in engine.GetAllObjects())
        //    Debug.Log(value.Id + ' ' + value.Type);
        myTime = 0;
	}

    public ConcurrentQueue<ITask> tasks = new ConcurrentQueue<ITask>();
    System.Diagnostics.Stopwatch watch ;



    int frameCount = 0;
    double dt = 0.0;
    double fps = 0.0;
    double updateRate = 1.0;  // 4 updates per sec.
	void Update () 
    {
        //var time =  watch.ElapsedMilliseconds/1000.0; // в этом месте надо использовать что-то другое, какие-то точные часы
        //world.Clocks.Tick(Time.realtimeSinceStartup);
        //Debug.Log(Time.deltaTime);
        //frameCount++;
        //dt += Time.deltaTime;
        //if (dt > 1.0 / updateRate)
        //{
        //    fps = frameCount / dt;
        //    frameCount = 0;
        //    dt -= 1.0 / updateRate;
        //    world.Clocks.Tick(myTime / updateRate);
        //    myTime += 1;
        //}
	}
    void FixedUpdate()
    {

        //var time =  watch.ElapsedMilliseconds/1000.0; // в этом месте надо использовать что-то другое, какие-то точные часы
        //world.Clocks.Tick(Time.fixedTime);
        Debug.Log(Time.fixedTime);
        //Debug.Log(Time.fixedDeltaTime);
    }
}
