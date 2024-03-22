using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Context;

namespace Token
{
    public interface IToken<out T, in C> : IDisplayable where C : IContextData
    {
        public ResolutionProtocol.IProtocol<T> Evaluate(C context);
    }
    public interface IDisplayable { }
}