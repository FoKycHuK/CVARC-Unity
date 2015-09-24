using CVARC.V2;


namespace Assets
{
    public class RMManagerPart : ManagerPart
    {
        public RMManagerPart() : base(() => new RMWorldManager()) { }

        public override IActorManager CreateActorManagerFor(IActor actor)
        {
            return new RMActorManager();
        }
    }
}
