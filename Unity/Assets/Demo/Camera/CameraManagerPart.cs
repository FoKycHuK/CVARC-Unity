using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;

namespace Assets
{
    public class CameraManagerPart : ManagerPart
    {
        public CameraManagerPart()
            : base(()=>new CameraWorldManager())
        { }

        public override IActorManager CreateActorManagerFor(IActor actor)
        {
            return new CameraActorManager();
        }
    }
}
