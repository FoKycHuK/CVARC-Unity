using CVARC.V2;
using DemoCompetitions;
using RepairTheStarship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets
{
    public class RTSWorldManager : WorldManager<RTSWorld>, IRTSWorldManager
    {

		public void CreateDetail(string detailId, AIRLab.Mathematics.Point2D detailLocation, DetailColor color)
		{
			
		}

		public void CreateEmptyTable()
		{
			
		}

		public void CreateWall(string wallId, AIRLab.Mathematics.Point2D centerLocation, WallData settings)
		{
			
		}

		public void RemoveDetail(string detailId)
		{
			
		}

		public void ShutTheWall(string wallId)
		{
			
		}

		public override void CreateWorld(IdGenerator generator)
		{
			//оставить пустым!
		}
	}
}
