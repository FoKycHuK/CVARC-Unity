using UnityEngine;
using System.Collections;
using Assets;

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
        var cubeObj = Instantiate(cubePref, new Vector3(0, 10, 0), new Quaternion()) as GameObject;
        Instantiate(planePref, new Vector3(0, 0, 0), Quaternion.Euler(15, 0, 0));
        cubeObj.AddComponent(typeof(Rigidbody));
        cubeObj.name = "my cube:Cube:CVARC_obj";
        //var engine = new UEngine();
        //foreach (var value in engine.GetAllObjects())
        //    Debug.Log(value.Id + ' ' + value.Type);
	}
	
	// Update is called once per frame
	void Update () 
    {
	}
}
