using CVARC.V2;
using Demo;
using UnityEngine;


namespace Assets
{
    public class DemoWorldManager : WorldManager<DemoWorld>, IDemoWorldManager
    {
        public IdGenerator Generator { get; private set; }
        public override void CreateWorld(IdGenerator generator)
        {
            var myPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            myPlane.transform.position = new Vector3(0, 0, 0);
            myPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
            myPlane.transform.localScale = new Vector3(20, 1, 20);
            myPlane.GetComponent<Renderer>().material.color = Color.red;
            myPlane.name = "floor";
            this.Generator = generator;
        }

        public void CreateObject(DemoObjectData data)
        {
            var gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var width = data.XSize;
            var height = data.YSize;
            var weight = data.ZSize;
            gameObj.AddComponent<Rigidbody>();
            gameObj.transform.position = new Vector3(data.X, weight / 2f, data.Y);
            gameObj.transform.rotation = Quaternion.Euler(0, 0, 0);
            gameObj.transform.localScale = new Vector3(width, weight, height);
            if (data.IsStatic)
            {
                gameObj.GetComponent<Rigidbody>().drag = 0.001F;
                gameObj.GetComponent<Rigidbody>().mass = 999;
                gameObj.GetComponent<Rigidbody>().isKinematic = data.IsStatic;
                gameObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                gameObj.GetComponent<Rigidbody>().drag = 40F;
                gameObj.GetComponent<Rigidbody>().angularDrag = 40F;
                gameObj.GetComponent<Rigidbody>().mass = 100;
                gameObj.GetComponent<Rigidbody>().isKinematic = data.IsStatic;
                gameObj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX |
                                                RigidbodyConstraints.FreezeRotationZ |
                                                RigidbodyConstraints.FreezePositionY;
            }
            gameObj.name = Generator.CreateNewId(data);
        }
    }
}
