using CVARC.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class DemoManagerPart : ManagerPart
    {
        public DemoManagerPart()
            : base(()=>new  DemoWorldManager())
        { }

        public override IActorManager CreateActorManagerFor(IActor actor)
        {
            return new DemoActorManager();
        }
    }
}
