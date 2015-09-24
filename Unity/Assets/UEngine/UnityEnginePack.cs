using CVARC.V2;


namespace Assets
{
    public class UnityEnginePack : EnginePart
    {
        public UnityEnginePack()
            : base(()=>new UEngine(), ()=>new UKeyboard())
        { }
    }
}
