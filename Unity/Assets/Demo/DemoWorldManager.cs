using CVARC.V2;
using Demo;
using DemoCompetitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
            myPlane.renderer.material.color = Color.red;
            myPlane.name = "floor";
            this.Generator = generator;
            //Settings = World.SceneSettings;

            //var myPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            //myPlane.renderer.material.color = Color.yellow;
            //var widthPlane = 30;
            //var heightPlane = 20;
            //var weightPlane = 5;
            //myPlane.transform.position = new Vector3(0, 0, 0);
            //myPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
            //myPlane.transform.localScale = new Vector3(widthPlane, weightPlane, heightPlane);

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
                gameObj.rigidbody.drag = 0.001F;
                gameObj.rigidbody.mass = 999;
                gameObj.rigidbody.isKinematic = data.IsStatic;
                //gameObj.rigidbody.useGravity = true;
                gameObj.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                gameObj.rigidbody.drag = 40F;
                gameObj.rigidbody.angularDrag = 40F;
                gameObj.rigidbody.mass = 100;
                gameObj.rigidbody.isKinematic = data.IsStatic;
                gameObj.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
                                                RigidbodyConstraints.FreezeRotationZ |
                                                RigidbodyConstraints.FreezePositionY;
            }
            gameObj.name = Generator.CreateNewId(data);
            //Debugger.Log(DebuggerMessageType.Drawing, "End Drawing");

        }
    }
}
