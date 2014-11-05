using CVARC.V2;
using Demo;
using DemoCompetitions;
using RepairTheStarship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class CameraWorldManager : WorldManager<MovementWorld>
    {
        public override void CreateWorld(IdGenerator generator)
        {
            var colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };
            var count = 10;
            double a = 0;
            for (int i = 0; i < count; i++)
            {
                //Root.Add(new Cylinder
                //{
                //    Location = new AIRLab.Mathematics.Frame3D(100 * Math.Sin(a), 100 * Math.Cos(a), 0),
                //    Height = 10,
                //    RTop = 10,
                //    RBottom = 10,
                //    DefaultColor = colors[i % colors.Length]
                //});
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                obj.AddComponent<Rigidbody>();
                obj.transform.position = new Vector3((float)(100 * Math.Sin(a)), 0, 100 * (float)(Math.Cos(a)));
                obj.transform.localScale = new Vector3(10, 10, 10);
                obj.renderer.material.color = colors[i % colors.Length];
                a += Math.PI * 2.0 / count;
            }
        }
    }
}
