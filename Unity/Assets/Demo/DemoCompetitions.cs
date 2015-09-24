using Assets;
using CVARC.V2;
using Demo;


namespace DemoCompetitions
{
    public class Level1 : Competitions
    {
        public Level1()
            : base(new DemoLogicPartHelper(), new UnityEnginePack(), new DemoManagerPart())
        { }
    }
}
