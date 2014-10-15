using CVARC.V2;
using DemoCompetitions;
using RepairTheStarship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class DemoWorldManager : WorldManager<DemoWorld>
    {


        public override void CreateWorld(IdGenerator generator)
        {
            var myPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            myPlane.transform.position = new Vector3(0, 0, 0);
            myPlane.transform.rotation = Quaternion.Euler(0, 0, 0);
            myPlane.transform.localScale = new Vector3(5, 5, 5);
        }
    }
}
