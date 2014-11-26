using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class RTSManagerPart : ManagerPart
    {
        public RTSManagerPart()
            : base(()=>new RTSWorldManager())
        { }

        public override IActorManager CreateActorManagerFor(IActor actor)
        {
            return new RTSActorManager();
        }
    }
}
