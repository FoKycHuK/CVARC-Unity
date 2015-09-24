using System;
using System.Collections.Generic;
using CVARC.V2;
using UnityEngine;
using RoboMovies;


namespace Assets
{
    public class RMWorldManager : IRMWorldManager
    {
        Vector3 openClapperCapOffset = new Vector3(-6.2f, 5.8f, 0);

        public void CloseClapperboard(string clapperboardId)
        {
            Debugger.Log(RMDebugMessage.Unity, "Closing clapperboard " + clapperboardId);
            var cap = GameObject.Find(clapperboardId);
            if (cap == null) return;

            cap.transform.rotation = Quaternion.identity;
            cap.transform.Translate(-openClapperCapOffset);
            Debugger.Log(RMDebugMessage.Unity, "Clapperboard closed at " + cap.transform.position.ToString());
        }

        public void CreateClapperboard(string clapperboardId, AIRLab.Mathematics.Point2D location, SideColor color)
        {
            var clapperboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            clapperboard.transform.position = new Vector3((float)location.X, 7, (float)location.Y);
            clapperboard.transform.localScale = new Vector3(16, 10, 5);
            clapperboard.GetComponent<Renderer>().material.color = Color.black;

            var cap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cap.transform.position = new Vector3((float)location.X, 14.5f, (float)location.Y);
            cap.transform.Translate(openClapperCapOffset);
            cap.transform.rotation = Quaternion.Euler(0, 0, 60);
            cap.transform.localScale = new Vector3(16, 5, 5);
            cap.GetComponent<Renderer>().material.color = UnityColor[color];
            cap.name = clapperboardId;
        }

        public void CreateEmptyTable()
        {
            Debugger.Log(RMDebugMessage.WorldCreation, "RMWorld creation started");
            var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.transform.position = Vector3.zero;
            floor.transform.rotation = Quaternion.Euler(0, 180, 0);
            floor.transform.localScale = new Vector3(30, 1, 20);
            floor.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D>("field");
            floor.name = "floor";

            var light = new GameObject("sunshine");
            light.AddComponent<Light>();
            light.GetComponent<Light>().type = LightType.Point;
            light.GetComponent<Light>().range = 1000;
            light.transform.position = new Vector3(0, 200, 0);

            RemoveObject("Point light");

            CreateBorders();
            
            Debugger.Log(RMDebugMessage.WorldCreation, "RMWorld creation successed!");
        }

        private void CreateBorders()
        {
            var length = 300;
            var width = 200;

            for (int i = 0; i < 4; ++i)
            {
                var offset = 2.2f;

                var sizeX = i / 2 == 0 ? length + offset : offset;
                var sizeZ = i / 2 == 1 ? width + offset : offset;
                var posX = i / 2 == 1 ? length + offset : offset;
                var posZ = i / 2 == 0 ? width + offset : offset;
                var side = i % 2 == 0 ? 1 : -1;

                var border = GameObject.CreatePrimitive(PrimitiveType.Cube);
                border.transform.position = new Vector3(side * posX / 2, 5, side * posZ / 2);
                border.transform.localScale = new Vector3(sizeX, 10, sizeZ);
                border.GetComponent<Renderer>().material.color = Color.red;
            }
        }

        public void CreateLight(string lightId, AIRLab.Mathematics.Point2D location)
        {
            var radius = 3.2f;
            var light = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            
            light.transform.position = new Vector3((float)location.X, radius, (float)location.Y);
            light.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);

            light.GetComponent<Renderer>().material.color = Color.yellow;

            light.AddComponent<Rigidbody>();
            
            light.GetComponent<Rigidbody>().drag = light.GetComponent<Rigidbody>().angularDrag = 4;
            light.GetComponent<Rigidbody>().useGravity = true;
            light.GetComponent<Rigidbody>().mass = 0.2f;
            light.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            light.name = lightId;
        }

        public void CreatePopCorn(string popcornId, AIRLab.Mathematics.Point2D location)
        {
            var popcorn = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            
            popcorn.transform.position = new Vector3((float)location.X, 7, (float)location.Y);
            popcorn.transform.localScale = new Vector3(9.5f, 7, 9.5f);

            popcorn.GetComponent<Renderer>().material.color = Color.white;

            popcorn.AddComponent<Rigidbody>();
            popcorn.AddComponent<MeshCollider>();
            
            popcorn.GetComponent<Rigidbody>().drag = popcorn.GetComponent<Rigidbody>().angularDrag = 4;
            popcorn.GetComponent<Rigidbody>().useGravity = true;
            popcorn.GetComponent<Rigidbody>().mass = 0.2f;
            popcorn.GetComponent<Rigidbody>().centerOfMass = new Vector3(0, -3, 0);
            popcorn.name = popcornId;
        }

        public void CreatePopCornDispenser(string dispenserId, AIRLab.Mathematics.Point2D location)
        {
            var dispenser = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dispenser.transform.position = new Vector3((float)location.X, 14, (float)location.Y);
            dispenser.transform.localScale = new Vector3(6, 28, 6);
            dispenser.GetComponent<Renderer>().material.color = Color.blue;
            dispenser.name = dispenserId;
        }

        public void CreateStairs(string stairsId, AIRLab.Mathematics.Point2D centerLocation, SideColor color)
        {
            var offset = new Vector3((float)centerLocation.X, 0, (float)centerLocation.Y);
        
            float ySize = 7;
            Func<float, float> getYSize = y => -ySize / (7 * 4) * (y - 39) + ySize;

            float width = 50;
            float bottomLength = 60;
            float topLength = bottomLength - 21;
         	int stairsCount = 3;
            var stairStep = (bottomLength - topLength) / stairsCount;
            
            for (var length = topLength; length <= bottomLength; length += stairStep)
            {
                var height = getYSize(length);
                var stair = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stair.transform.position = new Vector3(0, 0, (bottomLength - length) / 2);
                stair.transform.Translate(offset + Vector3.up * height / 2);
                stair.transform.localScale = new Vector3(width, height, length);
                stair.GetComponent<Renderer>().material.color = UnityColor[color];
                stair.name = length == topLength ? stairsId : "wall";
            }

            Action<float> addBorder = x =>
            {
                var border = GameObject.CreatePrimitive(PrimitiveType.Cube);
                border.transform.localScale = new Vector3(3, ySize + 2.2f, bottomLength);
                border.transform.position = new Vector3(x, (ySize + 2.2f) / 2, 0) + offset;
                border.GetComponent<Renderer>().material.color = Color.blue;
            };

            addBorder(width / 2);
            addBorder(-width / 2);
        }

        public void CreateStand(string standId, AIRLab.Mathematics.Point2D location, SideColor color)
        {
            var stand = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            
            stand.transform.position = new Vector3((float)location.X, 3.5f, (float)location.Y);
            stand.transform.localScale = new Vector3(6, 3.45f, 6);

            stand.GetComponent<Renderer>().material.color = UnityColor[color];

            stand.AddComponent<Rigidbody>();
            stand.AddComponent<MeshCollider>();

            stand.GetComponent<Rigidbody>().drag = stand.GetComponent<Rigidbody>().angularDrag = 4;
            stand.GetComponent<Rigidbody>().useGravity = true;
            stand.GetComponent<Rigidbody>().mass = 0.3f;
            stand.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            stand.name = standId;
        }

        public void CreateStartingArea(AIRLab.Mathematics.Point2D centerLocation, SideColor color)
        {
            var offset = new Vector3((float)centerLocation.X, 1.5f, (float)centerLocation.Y);

            var top = GameObject.CreatePrimitive(PrimitiveType.Cube);
            top.transform.position = new Vector3(0, 0, 40 / 2) + offset;
            
            var bottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bottom.transform.position = new Vector3(0, 0, -40 / 2) + offset;
            
            top.transform.localScale = bottom.transform.localScale = new Vector3(40, 3, 3);
            bottom.GetComponent<Renderer>().material.color = top.GetComponent<Renderer>().material.color = UnityColor[color];
        }

        public void RemoveObject(string objectId)
        {
            GameObject.Destroy(GameObject.Find(objectId));
        }

        public void CreateWorld(IdGenerator generator)
        {
            // Method Stub
        }

        public void Initialize(IWorld world)
        {
            // Method Stub
        }

        Dictionary<SideColor, Color> UnityColor = new Dictionary<SideColor, Color>
        {
            { SideColor.Green, Color.green },
            { SideColor.Yellow, Color.yellow },
            { SideColor.Any, Color.magenta } 
        };

        public void ClimbUpStairs(string actorId, string stairsId)
        {
            var actor = GameObject.Find(actorId);
            var stairs = GameObject.Find(stairsId);

            actor.transform.position = stairs.transform.position + new Vector3(0, 19, 0);
        }
    }
}
