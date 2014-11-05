using Assets;
using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepairTheStarship
{
    public class Level1 : Competitions
    {
        public Level1()
            : base(new RTSLogicPart<Level1SensorData>(), new UnityEnginePack(), new RTSManagerPart())
        { }
    }
}
