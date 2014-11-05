using Assets;
using CVARC.V2;
using Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoCompetitions
{
    public class Level1 : Competitions
    {
        public Level1()
            : base(new MovementLogicPart(), new UnityEnginePack(), new DemoManagerPart())
        { }
    }
}
