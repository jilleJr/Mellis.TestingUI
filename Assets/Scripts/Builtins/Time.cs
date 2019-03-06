using System.Linq;
using Mellis;
using Mellis.Core.Interfaces;

namespace Builtins
{
    public class Time : ClrFunction
    {
        public Time() : base("time")
        {
        }

        public override IScriptType Invoke(IScriptType[] arguments)
        {
            return Processor.Factory.Create(System.DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
