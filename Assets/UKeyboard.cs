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
                if (Input.GetKey(KeyCode.UpArrow)) pressedKeys.Add("W");
                if (Input.GetKey(KeyCode.DownArrow)) pressedKeys.Add("D");
                if (Input.GetKey(KeyCode.RightArrow)) pressedKeys.Add("R");
                if (Input.GetKey(KeyCode.LeftArrow)) pressedKeys.Add("L");
                return pressedKeys;
            }
        }

    }
}
