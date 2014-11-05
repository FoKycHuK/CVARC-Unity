using CVARC.V2;
using Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class CameraCompetitions : Competitions
    {
        public CameraCompetitions()
            : base(new CameraLogicPart(), new UnityEnginePack(), new CameraManagerPart())
        { }
    }
}
