using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AIRLab.Mathematics;
using System.Threading;

namespace Assets
{


    public class UEngine : CVARC.V2.IEngine
    {
        public void Initialize(CVARC.V2.IWorld world)
        {
           
        }

        public void SetSpeed(string id, Frame3D speed)
        {
            Debug.Log(string.Format("{0,-10}{1}", id, speed.X));
            GameObject MovingObject = GameObject.Find(id);
            MovingObject.rigidbody.freezeRotation = true;
            MovingObject.transform.Translate(new Vector3((float)speed.X, (float)speed.Y, (float)speed.Z) * Time.deltaTime);
            //    //MovingObject.rigidbody.velocity = new Vector3((float)speed.X, (float)speed.Y, (float)speed.Z);
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

        public bool ContainBody(string id)
        {
            // т.к. юнити сама создает кучу непонятных лишних объектов в мире, пусть будет так:
            // Имя (gameObject.name) любого объекта, который будет нужен в методе GetAllObjects будет иметь такой формат:
            // %id%:%type%:CVARC_obj
            //var allGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            //return allGameObjects.Where(obj => obj.name.Split(':').Length == 3 && obj.name.Split(':')[2] == "CVARC_obj")
            //    .Select(obj => (CVARC.Basic.IGameObject)new CVARC.Basic.GameObject(obj.name.Split(':')[0], obj.name.Split(':')[1]));

            return true;
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
            throw new NotImplementedException();
        }


    }
}
