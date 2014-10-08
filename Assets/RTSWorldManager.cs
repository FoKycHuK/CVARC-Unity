using RepairTheStarship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class RTSWorldManager : IRTSWorldManager
    {
        RTSWorld world;

        public void CreateWorld(CVARC.V2.IdGenerator generator)
        {

        }

        public void Initialize(CVARC.V2.IWorld world)
        {
            world = (RTSWorld)world;
        }
    }
}
