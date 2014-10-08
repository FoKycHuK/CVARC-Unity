using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class UKeyboard : IKeyboard
    {
        public IEnumerable<string> PressedKeys
        {
            get { yield break; }
        }
    }
}
