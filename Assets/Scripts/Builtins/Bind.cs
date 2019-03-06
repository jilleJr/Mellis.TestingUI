using System;
using System.Linq;
using Mellis;
using Mellis.Core.Interfaces;
using Mellis.Lang.Base.Entities;

namespace Builtins
{
    public class Bind : ClrFunction
    {
        public Bind() : base("bind")
        {
        }

        public override IScriptType Invoke(IScriptType[] arguments)
        {
            var func = arguments[0];
            if (func is ClrFunctionBase clr)
            {
                return Processor.Factory.Create(new BoundFunction(
                    clr.Definition, arguments.Skip(1).ToArray()
                ));
            }
            else
            {
                throw new ArgumentException("Function 'bind' needs first parameter to be a function yao :(.", nameof(arguments));
            }
        }

        private class BoundFunction : ClrFunction
        {
            private readonly IScriptType[] _arguments;
            private readonly IClrFunction _innerFunction;

            public BoundFunction(IClrFunction function, params IScriptType[] arguments)
                : base($"bind`{arguments.Length}:{function.FunctionName}")
            {
                _arguments = arguments;
                _innerFunction = function;
            }

            public override IScriptType Invoke(IScriptType[] arguments)
            {
                return _innerFunction.Invoke(_arguments.Concat(arguments).ToArray());
            }
        }
    }
}