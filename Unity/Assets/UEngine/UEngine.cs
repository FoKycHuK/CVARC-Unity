using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AIRLab.Mathematics;
using System.Threading;
using AIRLab;
using CVARC.Basic.Sensors;
using CVARC.V2;

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
                var oldVelocity = movingObject.rigidbody.velocity;
                movingObject.rigidbody.velocity = new Vector3((float)requested[e].X, oldVelocity.y, (float)requested[e].Y);
                movingObject.rigidbody.angularVelocity = new Vector3(0, (float)requested[e].Angle.Radian, 0);
            }
        }

        public Frame3D GetAbsoluteLocation(string id)
        {
            var obj = GameObject.Find(id);
            var pos = obj.transform.position;
            var rot = obj.transform.rotation.eulerAngles;
            var y = -rot.y;
            if (y < -180) y += 360;
            //Debugger.Log(DebuggerMessageType.Unity,y);
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
            //var allGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            //return allGameObjects.Where(obj => obj.name.Split(':').Length == 3 && obj.name.Split(':')[2] == "CVARC_obj")
            //    .Select(obj => (CVARC.Basic.IGameObject)new CVARC.Basic.GameObject(obj.name.Split(':')[0], obj.name.Split(':')[1]));
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
            // Debugger.Log(DebuggerMessageType.Unity,camPos.Pitch.Grad + " " + camPos.Roll.Grad + " " + camPos.Yaw.Grad);
            cam.transform.localPosition = new Vector3((float)camPos.X, (float)camPos.Z / 20, (float)camPos.Y); // ???????
            cam.transform.localRotation = Quaternion.Euler(-(float)camPos.Pitch.Grad, 90 + (float)camPos.Yaw.Grad, (float)camPos.Roll.Grad);
            cam.fieldOfView = (float)camRot.Grad;
            if (robot.renderer.material.color == Color.green)
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
            var vel = movingObject.rigidbody.velocity;
            var angVel = movingObject.rigidbody.angularVelocity;
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
            joint.connectedBody = parent.rigidbody;
            joint.enableCollision = false;
            joint.breakForce = Single.PositiveInfinity;
            
            attachedParams.Add(attachment, new Tuple<float, float>(attachment.rigidbody.drag, attachment.rigidbody.angularDrag));
            attachment.rigidbody.drag = attachment.rigidbody.angularDrag = 0;

            // Второй способ: приаттачивание с помощью родительского трансформа.
            // Не стоит использовать, т.к. юнька при аттаче localScale ребенка 
            // становится зависимым от localScale родителя.
            //// physics no affects the attachments rigidbody
            //if (attachment.rigidbody != null)
            //    attachment.rigidbody.isKinematic = true;
          
            //// move attacment to (0, 0, 0) relative to parent transform position
            //attachment.transform.position = parent.transform.position;
            //attachment.transform.rotation = parent.transform.rotation;

            //// set parent
            //attachment.transform.parent = parent.transform;
         
            //// set attachments position and rotation relative to parent
            //var rp = relativePosition;
            //attachment.transform.localPosition = new Vector3((float)rp.X / parent.transform.localScale.x, 
            //                                                 (float)rp.Z / parent.transform.localScale.y, 
            //                                                 (float)rp.Y / parent.transform.localScale.z);
            //attachment.transform.localRotation = Quaternion.Euler((float)rp.Roll.Grad, (float)rp.Yaw.Grad, (float)rp.Pitch.Grad);
        }

        public void Detach(string objectToDetach, Frame3D absolutePosition)
		{
            var attachment = GameObject.Find(objectToDetach);
            
            var joints = attachment.GetComponents<FixedJoint>();
            foreach(var joint in joints)
                GameObject.Destroy(joint);

            var attachmentParams = attachedParams[attachment];
            attachment.rigidbody.drag = attachmentParams.Item1;
            attachment.rigidbody.angularDrag = attachmentParams.Item2;
            attachedParams.Remove(attachment);

        	//attachment.transform.parent = null;

            //if (attachment.rigidbody != null)
            //    attachment.rigidbody.isKinematic = false;
            
            //var ap = absolutePosition;
            //attachment.transform.position = new Vector3((float)ap.X, (float)ap.Z, (float)ap.Y);
            //attachment.transform.rotation = Quaternion.Euler((float)ap.Roll.Grad, (float)ap.Yaw.Grad, (float)ap.Pitch.Grad);
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
