using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using AIRLab.Mathematics;
using CVARC.Basic.Sensors;
using System.Threading;

namespace Assets
{
    public class UEngine : CVARC.Basic.IEngine
    {
        creater behaviour;

        public UEngine(creater behaviour)
        {
            this.behaviour = behaviour;
        }



        public void Initialize(CVARC.Basic.ISceneSettings settings)
        {
            
            var robot1 = UnityEngine.Object.Instantiate(behaviour.cubePref, new Vector3(0, 5, 0), Quaternion.Euler(0, 0, 0));
            var robot2 = UnityEngine.Object.Instantiate(behaviour.cubePref, new Vector3(0, 5, 5), Quaternion.Euler(0, 0, 0));
            var robot3 = UnityEngine.Object.Instantiate(behaviour.cubePref, new Vector3(0, 5, 10), Quaternion.Euler(0, 0, 0));
            robot1.name = "1";
            robot2.name = "2";
            robot2.name = "0";
        }

        public void SetSpeed(string id, Frame3D speed)
        {

            var tast = new Task<object>(() =>
                {
                    Debug.Log(string.Format("{0,-10}{1}", id, speed.X));
                    GameObject MovingObject = GameObject.Find(id);
                    MovingObject.rigidbody.freezeRotation = true;
                    MovingObject.transform.Translate(new Vector3((float)speed.X, (float)speed.Y, (float)speed.Z) * Time.deltaTime);
                    return new object();
                });
            behaviour.tasks.Enqueue(tast);
            tast.Wait();
        //    //MovingObject.rigidbody.velocity = new Vector3((float)speed.X, (float)speed.Y, (float)speed.Z);

        }

        public Frame3D GetAbsoluteLocation(string id)
        {
        //    var obj = GameObject.Find(id);
        //    var pos = obj.transform.position;
        //    var rot = obj.transform.rotation.eulerAngles;
        //    return new Frame3D(pos.x, pos.y, pos.z, Angle.FromGrad(rot.x), Angle.FromGrad(rot.y), Angle.FromGrad(rot.z));
        //
            return new Frame3D();
        }


        public byte[] GetImageFromCamera(string cameraName)
        {
            //Camera[] allCameras = Resources.FindObjectsOfTypeAll(typeof(Camera)) as Camera[];
            //var camera = allCameras
            //    .Where(x => cameraName.Equals(x.name))
            //    .First();
            //camera.Render();
            //Texture2D image = new Texture2D(Screen.width, Screen.height);
            //image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            //image.Apply();
            //byte[] bytes = image.EncodeToPNG();
            ////Debug.Log(string.Format("Took screenshot to {0}", cameraName));
            //return bytes;
            return new byte[0];
        }

        public IEnumerable<CVARC.Basic.IGameObject> GetAllObjects()
        {
            // т.к. юнити сама создает кучу непонятных лишних объектов в мире, пусть будет так:
            // Имя (gameObject.name) любого объекта, который будет нужен в методе GetAllObjects будет иметь такой формат:
            // %id%:%type%:CVARC_obj
            //var allGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            //return allGameObjects.Where(obj => obj.name.Split(':').Length == 3 && obj.name.Split(':')[2] == "CVARC_obj")
            //    .Select(obj => (CVARC.Basic.IGameObject)new CVARC.Basic.GameObject(obj.name.Split(':')[0], obj.name.Split(':')[1]));
            yield break;        }


        public void DefineCamera(string cameraName, string host, RobotCameraSettings settings)
        {
           // throw new NotImplementedException();
        }

        public void DefineKinect(string kinectName, string host)
        {
            throw new NotImplementedException();
        }

        public ImageSensorData GetImageFromKinect(string kinectName)
        {
            throw new NotImplementedException();
        }

        public string GetReplay()
        {
            throw new NotImplementedException();
        }

        public Frame3D GetSpeed(string id)
        {
            throw new NotImplementedException();
        }

        public event CVARC.Basic.OnCollisionEventHandler OnCollision;

        public void PerformAction(string id, string action)
        {
            throw new NotImplementedException();
        }

        public void RaiseOnCollision(string firstBodyId, string secondBodyId, CVARC.Basic.CollisionType collisionType)
        {
            throw new NotImplementedException();
        }

        public void RunEngine(double timeInSeconds, bool inRealTime)
        {
            behaviour.Clockdown = (long)(timeInSeconds * 1000);
            while (behaviour.Clockdown > 0) Thread.Sleep(0);
        }
    }
}
