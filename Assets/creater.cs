using UnityEngine;
using System.Collections;
using Assets;
using System.Threading;
using System;
public class creater : MonoBehaviour 
{

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
        Instantiate(cameraPref, new Vector3(0, 3, -5), Quaternion.Euler(15, 0, 0));
        //var cubeObj = Instantiate(cubePref, new Vector3(0, 10, 0), new Quaternion()) as GameObject;
        Instantiate(planePref, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
        //cubeObj.AddComponent(typeof(Rigidbody));
        //cubeObj.name = "my cube:Cube:CVARC_obj";
        
       

        #region

        var Competitions = new Gems.Levels.Level1();
        Competitions.HelloPackage = new CVARC.Network.HelloPackage { MapSeed = -1 };

        var engine = new UEngine(this);

        Competitions.Initialize(
               engine,
               new[] { 
                    new CVARC.Basic.RobotSettings(0, true), 
                    new CVARC.Basic.RobotSettings(1, true) 
                });

        var bots = new[] 
            { 
                new CVARC.Basic.SquareMovingBot(), 
                new CVARC.Basic.SquareMovingBot()
            };


        new Thread(() => Competitions.ProcessParticipants(true, int.MaxValue, bots)) { IsBackground = true }
            .Start();

        #endregion

        watch=  new System.Diagnostics.Stopwatch();
        watch.Start();
        //foreach (var value in engine.GetAllObjects())
        //    Debug.Log(value.Id + ' ' + value.Type);

	}

    public ConcurrentQueue<ITask> tasks = new ConcurrentQueue<ITask>();
    System.Diagnostics.Stopwatch watch ;
    long oldTime = 0;
	// Update is called once per frame

    public long Clockdown;

	void Update () 
    {
        var time = watch.ElapsedMilliseconds;
        var delta = time - oldTime;
        oldTime = time;
        Clockdown -= delta;
       

        while (!tasks.IsEmpty)
        {
            tasks.Dequeue().Run();
        }
	}
}
