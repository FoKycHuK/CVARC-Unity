using CVARC.V2;


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
