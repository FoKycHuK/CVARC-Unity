using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using CVARC.V2;
using Demo;
using UnityEngine;

namespace Assets
{
    public class CameraActorManager : ActorManager<CameraRobot>
    {


        //private Stream GetResourceStream(string resourceName)
        //{
        //    var assembly = GetType().Assembly;
        //    var names = assembly.GetManifestResourceNames();
        //    return assembly.GetManifestResourceStream("Demo.KroR.Resourses." + resourceName);
        //}

        public override void CreateActorBody()
        {
            var robot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            if (Actor.ControllerId == "Left")
            {
                robot.renderer.material.color = Color.blue;
                robot.transform.position = new Vector3(-50, 3, 50);
            }
            else
            {
                robot.renderer.material.color = Color.yellow;
                robot.transform.position = new Vector3(50, 3, 50);
            }
            robot.AddComponent<Rigidbody>();
            robot.transform.rotation = Quaternion.Euler(new Vector3(0, -(float)Math.PI / 2, 0));
            robot.transform.localScale = new Vector3(20, 10, 10);
            robot.name = Actor.ObjectId;
            robot.rigidbody.drag = 0; // трение
            robot.rigidbody.angularDrag = 0;
            robot.rigidbody.useGravity = false;

            var plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plane.transform.parent = robot.transform;
            plane.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            plane.renderer.material.color = Color.white;
            plane.transform.localPosition = new Vector3(0.3f, 0.90f, 0f);
        }
    }
}
