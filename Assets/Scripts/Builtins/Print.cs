using System.Linq;
using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;

namespace Builtins
{
    public class Print : ClrFunction
    {
        public Print() : base("print")
        {
        }

        public override IScriptType Invoke(IScriptType[] arguments)
        {
            string joined = string.Join(" ", arguments.Select(o => o.ToString()));
            string message = $"<color=#999><i>(from {Processor.CurrentSource})</i></color> {joined}";
            ConsoleLogger.Debug(message);
            Debug.Log(string.Join("\t", arguments.Cast<object>()));

            return null;
        }
    }
}
