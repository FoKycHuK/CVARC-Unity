using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class UnityEnginePack : EnginePart
    {
        public UnityEnginePack()
            : base(new UEngine(), new UKeyboard())
        { }
    }
}
