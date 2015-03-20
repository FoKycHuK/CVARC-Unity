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
    public class DemoActorManager : ActorManager<DemoRobot>
    {

        public override void CreateActorBody()
        {
            GameObject robot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            if (Actor.ControllerId == "Left")
            {
                robot.transform.position = new Vector3(0, 5, 0);
                robot.renderer.material.color = Color.green;
            }
            else
            {
                robot.transform.position = new Vector3(45, 5, 0);
                //robot = GameObject.Instantiate(creater.Behaviour.cubePref, new Vector3(45, 5, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
                robot.renderer.material.color = Color.red;
            }
            robot.AddComponent<Rigidbody>();
            robot.transform.rotation = Quaternion.Euler(0, 0, 0);
            var plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plane.transform.parent = robot.transform;
            plane.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            plane.renderer.material.color = Color.white;
            plane.transform.localPosition = new Vector3(0.3f, 0.90f, 0f);
            robot.rigidbody.drag = 0; // трение
            robot.rigidbody.angularDrag = 0;
            robot.rigidbody.useGravity = false;
            robot.AddComponent("OnCollisionScript");
            robot.name = Actor.ObjectId;



            //GameObject robot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            //if (Actor.ControllerId == "Left")
            //{
            //    //robot = GameObject.Instantiate(creater.Behaviour.cubePref, new Vector3(5, 5000, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
            //    robot.transform.position = new Vector3(-150 + 25 - 10, 10, 100 - 25 + 10);
            //    robot.transform.rotation = Quaternion.Euler(0, 0, 0);
            //    robot.transform.localScale = new Vector3(20, 10, 20);
            //    robot.renderer.material.color = Color.green;
            //}
            //else
            //{
            //    //robot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            //    //GameObject.Instantiate(creater.Behaviour.cubePref, new Vector3(150 - 25 + 10, 5, 100 - 25 + 10), Quaternion.Euler(0, 0, 0)) as GameObject;

            //    //robot.AddComponent<Rigidbody>();
            //    //robot.rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;

            //    robot.transform.position = new Vector3(150 - 25 + 10, 10, 100 - 25 + 10);
            //    robot.transform.rotation = Quaternion.Euler(0, 0, 0);
            //    robot.transform.localScale = new Vector3(20, 10, 20);
            //    robot.renderer.material.color = Color.red;
            //}
            //robot.AddComponent<Rigidbody>();
            //robot.transform.rotation = Quaternion.Euler(0, 0, 0);
            //var plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //plane.transform.parent = robot.transform;
            //plane.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            //plane.renderer.material.color = Color.white;
            //plane.transform.localPosition = new Vector3(0.3f, 0.90f, 0f);
            //robot.rigidbody.drag = 0; // трение
            //robot.rigidbody.angularDrag = 0;
            //robot.rigidbody.useGravity = false;
            //robot.AddComponent("OnCollisionScript");
            //robot.name = Actor.ObjectId;



            Actor.World.Engine.DefineCamera(Actor.ObjectId + ".Camera", Actor.ObjectId, new CVARC.V2.RobotCameraSettings());
        
        }
    }
}
