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
            var state = Actor.World.WorldState;
            var description = state.Robots.FirstOrDefault(z => z.RobotName == Actor.ControllerId);
            if (description == null) throw new Exception("Robot " + Actor.ControllerId + " is not defined in WorldState");
            Debugger.Log(DebuggerMessageType.Always, Actor.ControllerId);
         

            //var location = new Frame3D(description.X, description.Y, description.ZSize / 2, Angle.Zero, description.Yaw, Angle.Zero);
            var robot = GameObject.CreatePrimitive(description.IsRound ? PrimitiveType.Cylinder : PrimitiveType.Cube);
            //if (Actor.ControllerId == "Left")
            //{
            //    robot.transform.position = new Vector3(description.X, 5, description.Y);
            //    robot.renderer.material.color = Color.green;
            //}
            //else
            //{
            //    robot.transform.position = new Vector3(45, 5, 0);
            //    //robot = GameObject.Instantiate(creater.Behaviour.cubePref, new Vector3(45, 5, 0), Quaternion.Euler(0, 0, 0)) as GameObject;
            //    robot.renderer.material.color = Color.red;
            //}
            robot.transform.position = new Vector3(description.X, 
                (description.IsRound ? description.ZSize : description.ZSize/2f) + 0.2f, description.Y);
            robot.AddComponent<Rigidbody>();
            robot.renderer.material.color = Color.green;
            if (description.IsRound)
                robot.transform.localScale = new Vector3(description.XSize*2f, description.ZSize, description.YSize * 2);
            else
                robot.transform.localScale = new Vector3(description.XSize, description.ZSize, description.YSize);
            robot.transform.rotation = Quaternion.Euler(0, (float)description.Yaw.Grad, 0);
            //robot.transform.rotation = Quaternion.Euler(0, 0, 0);
            var plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plane.transform.parent = robot.transform;
            plane.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            plane.renderer.material.color = Color.white;
            plane.transform.localPosition = new Vector3(0.3f, 0.90f, 0f);
            robot.rigidbody.drag = 0F; // трение
            robot.rigidbody.angularDrag = 0F;
            robot.rigidbody.useGravity = true;
            robot.rigidbody.mass = 2700;
            robot.rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX |
                                          RigidbodyConstraints.FreezeRotationZ;
            robot.AddComponent("OnCollisionScript");
            //Physics.minPenetrationForPenalty = 0.0001f;
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



           // Actor.World.Engine.DefineCamera(Actor.ObjectId + ".Camera", Actor.ObjectId, new CVARC.V2.RobotCameraSettings());
        
        }
    }
}

/*
	var state = (Actor.World as DemoWorld).WorldState;

			var description = state.Robots.Where(z => z.RobotName == Actor.ControllerId).First();


            string fileName = "red.png";
			if (Actor.ControllerId == TwoPlayersId.Right) fileName = "blue.png";

			var location = new Frame3D(description.X, description.Y, description.ZSize / 2, Angle.Zero,description.Yaw, Angle.Zero);

			if (description.IsRound)
				root.Add(new Cylinder
					{
						Height = description.ZSize,
						RTop = description.XSize,
						RBottom = description.XSize,
						Location = location,
						DefaultColor = MovementWorldManager.ToColor(description.Color),
						IsMaterial = true,
						Density = Density.Iron,
						FrictionCoefficient = 0,
						Top = new PlaneImageBrush { Image = Bitmap.FromStream(GetResourceStream(fileName)) },
						NewId = Actor.ObjectId
					});
			else
				root.Add(new Box
				{
					XSize = description.XSize,
					YSize = description.YSize,
					ZSize = description.ZSize,
					DefaultColor = MovementWorldManager.ToColor(description.Color),
					IsMaterial = true,
					NewId = Actor.ObjectId,
					Location = location,
					Density = Density.Iron,
					Top = new PlaneImageBrush { Image = Bitmap.FromStream(GetResourceStream(fileName)) },
				});
*/