using System.Linq;
using Mellis;
using Mellis.Core.Interfaces;
using UnityEngine;

namespace Builtins
{
    public class Input : ClrYieldingFunction
    {
        public Input() : base("input")
        {
        }

        public override void InvokeEnter(IScriptType[] arguments)
        {
            Object.FindObjectOfType<InputBuiltinField>().Show();
        }
    }
}
