using CVARC.V2;
using System;
using System.Collections.Generic;
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

                foreach (KeyCode e in Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKey(e))
                        pressedKeys.Add(e.ToString());

                return pressedKeys;
            }
        }

    }
}
