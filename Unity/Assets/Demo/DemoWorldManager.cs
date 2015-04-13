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
        public override void CreateWorld(IdGenerator generator)
        {
            var myPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            myPlane.transform.position = new Vector3(0, -1, 0);
            myPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
            myPlane.transform.localScale = new Vector3(20, 1, 20);
            myPlane.renderer.material.color = Color.red;

            //this.generator = generator;
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
            //if (data == null)
            //{
            //    Debugger.Log(DebuggerMessageType.Drawing, "data is null!");
            //    return;
            //}
            //Debugger.Log(DebuggerMessageType.Drawing, "Begin Drawing");
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
            //Debugger.Log(DebuggerMessageType.Drawing, "End Drawing");

        }
    }
}
