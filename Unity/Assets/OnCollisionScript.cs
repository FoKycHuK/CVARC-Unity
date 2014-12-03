﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class OnCollisionScript : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            if (RoundScript.CollisionInfo.Item3 == 0)
            {
                RoundScript.CollisionInfo.Item3 = 1;
                RoundScript.CollisionInfo.Item1 = collision.gameObject.name;
            }
            else
            {
                RoundScript.CollisionInfo.Item2 = collision.gameObject.name;
                RoundScript.CollisionInfo.Item3 = 2;
            }
        }
        //void OnCollisionExit(Collision collision)
        //{
        //    var conflictObject = collision.gameObject;
        //    conflictObject.rigidbody.velocity = new Vector3(0, 0, 0);
        //    conflictObject.rigidbody.angularVelocity = new Vector3(0, 0, 0);
        //}
    }
}