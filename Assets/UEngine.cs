﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AIRLab.Mathematics;
using System.Threading;
using AIRLab;

namespace Assets
{


    public class UEngine : CVARC.V2.IEngine
    {
        Dictionary<string, Frame2D> requested = new Dictionary<string, Frame2D>();

        public void Initialize(CVARC.V2.IWorld world)
        {
           
        }

        public void SetSpeed(string id, Frame3D speed)
        {
            var movingObject = GameObject.Find(id);
            //var direction = (double)(movingObject.transform.rotation.eulerAngles.y * Math.PI / 180);
            //if (direction > Math.PI)
            //    direction -= 2 * Math.PI;
            //if (movingObject.renderer.material.color == Color.green)
            //{
            //    Debug.Log(direction);
            //    Debug.Log("X " + speed.X);
            //    Debug.Log("Y " + speed.Y);
            //    Debug.Log("Z " + speed.Z);
            //}
            //float x = (float)(Math.Cos(direction) * speed.X);
            //float y = (float)(Math.Sin(direction) * speed.X);
            //случился некий конфуз.


            //if (movingObject.renderer.material.color == Color.green)
            //{
                
            //    Debug.Log(movingObject.transform.rotation.eulerAngles.z);
            //    Debug.Log(movingObject.transform.position.x.ToString() + ' ' + movingObject.transform.position.z.ToString());
            //}
            movingObject.rigidbody.velocity = new Vector3((float)speed.X, 0, (float)speed.Y);
            movingObject.rigidbody.angularVelocity = new Vector3(0, (float)speed.Yaw.Radian, 0);
            //requested[id] = new Frame2D(x, y, speed.Yaw);

            //if (movingObject.transform.position.y != 5)
            //{
            //    var pos = movingObject.transform.position;
            //    movingObject.transform.position = new Vector3(pos.x, 5, pos.y);
            //}
            //movingObject.transform.position = new Vector3(movingObject.transform.position.x, 5, movingObject.transform.position.z);
            //movingObject.transform.rotation = new Quaternion(0, movingObject.transform.rotation.y, 0, movingObject.transform.rotation.w);
        }

        public void UpdateSpeeds()
        {
            foreach (var e in requested.Keys)
            {
                var movingObject = GameObject.Find(e);
                movingObject.rigidbody.velocity = new Vector3((float)requested[e].X, 0, (float)requested[e].Y);
                movingObject.rigidbody.angularVelocity = new Vector3(0, (float)requested[e].Angle.Radian, 0);
            }
        }

        public Frame3D GetAbsoluteLocation(string id)
        {
            var obj = GameObject.Find(id);
            var pos = obj.transform.position;
            var rot = obj.transform.rotation.eulerAngles;
            return new Frame3D(pos.x, pos.y, pos.z, Angle.FromGrad(rot.x), Angle.FromGrad(rot.y), Angle.FromGrad(rot.z));
        }


        public byte[] GetImageFromCamera(string cameraName)
        {
            Camera[] allCameras = Resources.FindObjectsOfTypeAll(typeof(Camera)) as Camera[];
            var camera = allCameras
                .Where(x => cameraName.Equals(x.name))
                .First();
            camera.Render();
            Texture2D image = new Texture2D(Screen.width, Screen.height);
            image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            image.Apply();
            byte[] bytes = image.EncodeToPNG();
            //Debug.Log(string.Format("Took screenshot to {0}", cameraName));
            return bytes;
        }

   

        public event Action<string, string> Collision;

        public void CollisionSender(string firstId, string secondId)
        {
            if (Collision != null)
                Collision(firstId, secondId);
        }

        public bool ContainBody(string id)
        {
            //var allGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            //return allGameObjects.Where(obj => obj.name.Split(':').Length == 3 && obj.name.Split(':')[2] == "CVARC_obj")
            //    .Select(obj => (CVARC.Basic.IGameObject)new CVARC.Basic.GameObject(obj.name.Split(':')[0], obj.name.Split(':')[1]));
            return !(GameObject.Find(id) == null);
        }

        public void DefineCamera(string cameraName, string host, CVARC.Basic.Sensors.RobotCameraSettings settings)
        {
            throw new NotImplementedException();
        }

        public void DefineKinect(string kinectName, string host)
        {
            throw new NotImplementedException();
        }

        public CVARC.Basic.Sensors.ImageSensorData GetImageFromKinect(string kinectName)
        {
            throw new NotImplementedException();
        }

        public Frame3D GetSpeed(string id)
        {
            var movingObject = GameObject.Find(id);
            var vel = movingObject.rigidbody.velocity;
            var angVel = movingObject.rigidbody.angularVelocity;
            return new Frame3D(vel.x, vel.y, vel.z, Angle.FromRad(angVel.y), Angle.FromRad(angVel.z), Angle.FromRad(angVel.x)); //???
        }
    }
}
