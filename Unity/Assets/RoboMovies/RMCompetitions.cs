using Assets;
using CVARC.V2;
using RoboMovies;


namespace RMCompetitions
{
    public class Level1 : Competitions
    {
        public Level1()
            : base(new RMLogicPartHelper(), new UnityEnginePack(), new RMManagerPart())
        { }
    }
}
