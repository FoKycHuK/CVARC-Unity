using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using UnityEngine;
using RoboMovies;

namespace Assets
{
    public class RMActorManager : ActorManager<IActor>, IRMActorManager
    {
        const float robotRadius = 12;
        const float robotHeight = 24;
        const float robotMass = 2.7f;

        public override void CreateActorBody()
        {
            var location = new Vector3(-150 + 35, robotHeight / 2, 0);
            var rotation = Quaternion.Euler(0, 0, 0);
            string topTexture = "yellow";

            if (Actor.ControllerId == TwoPlayersId.Right)
            {
                location = new Vector3(150 - 35, robotHeight / 2, 0);
                rotation = Quaternion.Euler(0, 180, 0);
                topTexture = "green";
            }

            var actorBody = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            actorBody.AddComponent<Rigidbody>();
            
            actorBody.transform.position = location;
            actorBody.transform.rotation = rotation;
            actorBody.transform.localScale = new Vector3(robotRadius * 2, robotHeight / 2, robotRadius * 2);
            
            actorBody.GetComponent<Renderer>().material.color = Color.magenta;

            actorBody.GetComponent<Rigidbody>().drag = 0;
            actorBody.GetComponent<Rigidbody>().angularDrag = 0;
            actorBody.GetComponent<Rigidbody>().useGravity = true;
            actorBody.GetComponent<Rigidbody>().mass = robotMass;
            actorBody.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ |
                                              RigidbodyConstraints.FreezePositionY;
            actorBody.AddComponent<MeshCollider>();

            actorBody.AddComponent<OnCollisionScript>();
            actorBody.name = Actor.ObjectId;

            var actorHead = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            actorHead.transform.position = actorBody.transform.position + Vector3.up * (robotHeight / 2 + 0.1f);
            actorHead.transform.rotation = actorBody.transform.rotation;
            actorHead.transform.localScale = new Vector3(robotRadius * 2, 0.1f, robotRadius * 2);
            actorHead.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D>(topTexture);
            actorHead.transform.parent = actorBody.transform;

            var floor = GameObject.Find("floor");
            foreach (var robotCollider in actorBody.GetComponents<Collider>())
                Physics.IgnoreCollision(floor.GetComponent<MeshCollider>(), robotCollider);
        }
    }
}
