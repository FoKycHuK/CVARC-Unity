using CVARC.V2;
using Demo;
using System;
using System.Linq;
using UnityEngine;


//из этого файла была удалена куча комментов. найти их можно в одном из последних коммитов на http://github.com/fokychuk/cvarc-unity
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
         

            var robot = GameObject.CreatePrimitive(description.IsRound ? PrimitiveType.Cylinder : PrimitiveType.Cube);
            robot.transform.position = new Vector3(description.X, description.IsRound ? description.ZSize : description.ZSize/2f, description.Y);
            robot.AddComponent<Rigidbody>();
            robot.GetComponent<Renderer>().material.color = Color.green;
            if (description.IsRound)
                robot.transform.localScale = new Vector3(description.XSize*2f, description.ZSize, description.YSize * 2);
            else
                robot.transform.localScale = new Vector3(description.XSize, description.ZSize, description.YSize);
            robot.transform.rotation = Quaternion.Euler(0, (float)description.Yaw.Grad, 0);
            var plane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plane.transform.parent = robot.transform;
            plane.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            plane.GetComponent<Renderer>().material.color = Color.white;
            plane.transform.localPosition = new Vector3(0.3f, 0.90f, 0f);
            robot.GetComponent<Rigidbody>().drag = 0F; // трение
            robot.GetComponent<Rigidbody>().angularDrag = 0F;
            robot.GetComponent<Rigidbody>().useGravity = true;
            robot.GetComponent<Rigidbody>().mass = 2700;
            robot.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX |
                                          RigidbodyConstraints.FreezeRotationZ;
            robot.AddComponent<OnCollisionScript>();
            robot.name = Actor.ObjectId;

            var floor = GameObject.Find("floor");
            foreach (var robotCollider in robot.GetComponents<Collider>())
                Physics.IgnoreCollision(floor.GetComponent<MeshCollider>(), robotCollider);
        }
    }
}