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
    public class RTSActorManager : ActorManager<DemoRobot>
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
                robot = GameObject.Instantiate(creater.Behaviour.cubePref, new Vector3(0, 5, 5), Quaternion.Euler(0, 0, 0)) as GameObject;
                robot.renderer.material.color = Color.red;
            }
            robot.name = Actor.World.IdGenerator.CreateNewId(Actor);
        }

      
    }
}
