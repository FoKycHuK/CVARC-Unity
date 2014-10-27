using CVARC.V2;
using DemoCompetitions;
using RepairTheStarship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class RTSActorManager : ActorManager<IRTSRobot>, IRTSActorManager
    {



        public override void CreateActorBody()
        {
            GameObject robot = null;
            if (Actor.ControllerId == "Left")
            {
                robot = GameObject.Instantiate(creater.Behaviour.cubePref, new Vector3(0, 5, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
                robot.renderer.material.color = Color.green;
            }
            else
            {
                robot = GameObject.Instantiate(creater.Behaviour.cubePref, new Vector3(5, 5, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
                robot.renderer.material.color = Color.red;
            }
//эксепшн
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.parent = robot.transform;
            plane.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            plane.renderer.material.color = Color.black;
            plane.transform.localPosition = new Vector3(0.6f, 1f, 0f);
//эксепшн
            //robot.rigidbody.drag = 1;
            robot.rigidbody.angularDrag = 0;
            robot.rigidbody.useGravity = false;
            robot.AddComponent("OnCollisionScript");
            robot.name = Actor.ObjectId;

        }



        public void Capture(string detailId)
        {
            
        }

        public bool IsDetailFree(string detailId)
        {
            return true;
        }

        public void Release(string detailId)
        {
            
        }
    }
}
