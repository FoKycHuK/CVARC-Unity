using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AIRLab.Mathematics;
using AIRLab;


namespace Assets
{
    public class UEngine : CVARC.V2.IEngine
    {
        Dictionary<string, Frame2D> requested = new Dictionary<string, Frame2D>();

        public void Initialize(CVARC.V2.IWorld world)
        {
           
        }

        public void Stop()
        {
            foreach (var id in requested.Keys.ToArray())
                requested[id] = new Frame2D();
        }

        public void SetSpeed(string id, Frame3D speed)
        {
            requested[id] = new Frame2D(speed.X, speed.Y, -speed.Yaw);
        }

        public void UpdateSpeeds()
        {
            foreach (var e in requested.Keys)
            {
                var movingObject = GameObject.Find(e);
                var oldVelocity = movingObject.GetComponent<Rigidbody>().velocity;
                movingObject.GetComponent<Rigidbody>().velocity = new Vector3((float)requested[e].X, oldVelocity.y, (float)requested[e].Y);
                movingObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, (float)requested[e].Angle.Radian, 0);
            }
        }

        public Frame3D GetAbsoluteLocation(string id)
        {
            var obj = GameObject.Find(id);
            var pos = obj.transform.position;
            var rot = obj.transform.rotation.eulerAngles;
            var y = -rot.y;
            if (y < -180) y += 360;
            return new Frame3D(pos.x, pos.z, pos.y, Angle.FromGrad(rot.x), Angle.FromGrad(y), Angle.FromGrad(rot.z));
        }


        public byte[] GetImageFromCamera(string cameraName)
        {
            return new byte[0];
            Camera[] allCameras = Resources.FindObjectsOfTypeAll(typeof(Camera)) as Camera[];
            var camera = allCameras
                .Where(x => cameraName.Equals(x.name))
                .First();
            camera.Render();
            Texture2D image = new Texture2D(Screen.width, Screen.height);
            image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            image.Apply();
            byte[] bytes = image.EncodeToPNG();
            //Debugger.Log(DebuggerMessageType.Unity,string.Format("Took screenshot to {0}", cameraName));
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
            return !(GameObject.Find(id) == null);
        }

        public void DefineCamera(string cameraName, string host, CVARC.V2.RobotCameraSettings settings)
        {
            return;
            var cam = new GameObject(cameraName).AddComponent<Camera>();
            var robot = GameObject.Find(host);
            cam.transform.parent = robot.transform;
            var camPos = settings.Location;
            var camRot = settings.ViewAngle;
            cam.transform.localPosition = new Vector3((float)camPos.X, (float)camPos.Z / 20, (float)camPos.Y); // ???????
            cam.transform.localRotation = Quaternion.Euler(-(float)camPos.Pitch.Grad, 90 + (float)camPos.Yaw.Grad, (float)camPos.Roll.Grad);
            cam.fieldOfView = (float)camRot.Grad;
            if (robot.GetComponent<Renderer>().material.color == Color.green)
                cam.rect = new Rect(0, 0.7f, 0.3f, 0.3f);
            else
                cam.rect = new Rect(0.7f, 0.7f, 0.3f, 0.3f);
        }

        public void DefineKinect(string kinectName, string host)
        {
            var sets = new CVARC.V2.RobotCameraSettings();
            //и вот с ним работать
            throw new NotImplementedException();
        }

        public CVARC.Basic.Sensors.ImageSensorData GetImageFromKinect(string kinectName)
        {
            throw new NotImplementedException();
        }

        public Frame3D GetSpeed(string id)
        {
            var movingObject = GameObject.Find(id);
            var vel = movingObject.GetComponent<Rigidbody>().velocity;
            var angVel = movingObject.GetComponent<Rigidbody>().angularVelocity;
            return new Frame3D(vel.x, vel.y, vel.z, Angle.FromRad(angVel.y), Angle.FromRad(angVel.z), Angle.FromRad(angVel.x)); //???
        }

        Dictionary<GameObject, Tuple<float, float>> attachedParams = new Dictionary<GameObject, Tuple<float, float>>();
        public void Attach(string objectToAttach, string host, Frame3D relativePosition)
        {
            var parent = GameObject.Find(host);
            var attachment = GameObject.Find(objectToAttach);
            
            // move attachment to (0, 0, 0) relative to parent
            attachment.transform.position = parent.transform.position;
            attachment.transform.rotation = parent.transform.rotation;
            
            // set attachments position and rotation relative to parent
            var rp = relativePosition;
            attachment.transform.position += Quaternion.Euler(parent.transform.eulerAngles) * 
                new Vector3((float)rp.X, (float)rp.Z, (float)rp.Y);
            attachment.transform.rotation *= Quaternion.Euler((float)rp.Roll.Grad, (float)rp.Yaw.Grad, (float)rp.Pitch.Grad);
            
            // create unbreakable joint between attachment and parent
            var joint = attachment.AddComponent<FixedJoint>();
            joint.connectedBody = parent.GetComponent<Rigidbody>();
            joint.enableCollision = false;
            joint.breakForce = Single.PositiveInfinity;
            
            attachedParams.Add(attachment, new Tuple<float, float>(attachment.GetComponent<Rigidbody>().drag, attachment.GetComponent<Rigidbody>().angularDrag));
            attachment.GetComponent<Rigidbody>().drag = attachment.GetComponent<Rigidbody>().angularDrag = 0;
        }

        public void Detach(string objectToDetach, Frame3D absolutePosition)
        {
            var attachment = GameObject.Find(objectToDetach);
            
            var joints = attachment.GetComponents<FixedJoint>();
            foreach(var joint in joints)
                GameObject.Destroy(joint);

            if (attachedParams.ContainsKey(attachment))
            {
                var attachmentParams = attachedParams[attachment];
                attachment.GetComponent<Rigidbody>().drag = attachmentParams.Item1;
                attachment.GetComponent<Rigidbody>().angularDrag = attachmentParams.Item2;
                attachedParams.Remove(attachment);
            }
        }
        
        public void DeleteObject(string objectId)
        {
            GameObject.Destroy(GameObject.Find(objectId));
        }

        public string FindParent(string objectId)
        {
            var obj = GameObject.Find(objectId);
            if (obj == null) return null;
            
            var parent = FindParentByJoints(obj);
            if (parent == null) 
                parent = FindParentByHierarchy(obj);
            
            return parent;
        }

        string FindParentByHierarchy(GameObject obj)
        {
            if (obj.transform == null) return null;
            if (obj.transform.parent == null) return null;
            return obj.transform.parent.name;
        }

        string FindParentByJoints(GameObject obj)
        {
            var joint = obj.GetComponents<FixedJoint>().FirstOrDefault();
            if (joint == null) return null;
            if (joint.connectedBody == null) return null;
            return joint.connectedBody.name;
        }
    }
}
