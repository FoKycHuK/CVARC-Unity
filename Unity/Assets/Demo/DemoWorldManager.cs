using CVARC.V2;
using Demo;
using DemoCompetitions;
using System;
using System.Collections.Generic;
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
            myPlane.transform.position = new Vector3(0, 0, 0);
            myPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
            myPlane.transform.localScale = new Vector3(5, 5, 5);
            
            
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
        }
    }
}
