using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    public class UKeyboard : IKeyboard
    {
        public IEnumerable<string> PressedKeys 
        {
            get
            {
                var pressedKeys = new List<string>();
                if (Input.GetKey(KeyCode.W)) pressedKeys.Add("W");
                if (Input.GetKey(KeyCode.D)) pressedKeys.Add("D");
                if (Input.GetKey(KeyCode.A)) pressedKeys.Add("A");
                if (Input.GetKey(KeyCode.S)) pressedKeys.Add("S");

                if (Input.GetKey(KeyCode.I)) pressedKeys.Add("I");
                if (Input.GetKey(KeyCode.K)) pressedKeys.Add("K");
                if (Input.GetKey(KeyCode.J)) pressedKeys.Add("J");
                if (Input.GetKey(KeyCode.L)) pressedKeys.Add("L");

                if (Input.GetKey(KeyCode.Q)) pressedKeys.Add("Q");
                if (Input.GetKey(KeyCode.E)) pressedKeys.Add("E");
                if (Input.GetKey(KeyCode.U)) pressedKeys.Add("U");
                if (Input.GetKey(KeyCode.O)) pressedKeys.Add("O");

                //if (pressedKeys.Count > 5)
                //    return pressedKeys.Take(5);
                return pressedKeys.Take(2);
            }
        }

    }
}
