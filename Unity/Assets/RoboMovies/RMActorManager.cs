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
        public override void CreateActorBody()
        {
            var location = new Vector3(-150 + 35, 10, 0);
            var rotation = Quaternion.Euler(0, 0, 0);
            string topTexture = "yellow";

            if (Actor.ControllerId == TwoPlayersId.Right)
            {
                location = new Vector3(150 - 35, 10, 0);
                rotation = Quaternion.Euler(0, 180, 0);
                topTexture = "green";
            }

            var actorBody = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            actorBody.AddComponent<Rigidbody>();
            
            actorBody.transform.position = location;
            actorBody.transform.rotation = rotation;
            actorBody.transform.localScale = new Vector3(24, 10, 24);
            
            actorBody.renderer.material.color = Color.magenta;

            actorBody.rigidbody.drag = 0;
            actorBody.rigidbody.angularDrag = 0;
            actorBody.rigidbody.useGravity = false;
            actorBody.rigidbody.mass = 2700;
            actorBody.rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            actorBody.AddComponent("OnCollisionScript");
            actorBody.name = Actor.ObjectId;

            var actorHead = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            actorHead.transform.position = actorBody.transform.position + Vector3.up * 10.1f;
            actorHead.transform.rotation = actorBody.transform.rotation;
            actorHead.transform.localScale = new Vector3(24, 0.1f, 24);
            actorHead.renderer.material.mainTexture = Resources.Load<Texture2D>(topTexture);
            actorHead.transform.parent = actorBody.transform;
        }
    }
}
