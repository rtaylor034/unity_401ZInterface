using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Context;

namespace Token
{
    public interface IToken<out T, in C> where C : IContextData
    {
        public T Resolve(C context);
    }
    public class Reference<T, C> : IToken<T, C> where C : IContextData
    {
        private IToken<T, C> _refersTo;
        public T Resolve(C context) => _refersTo.Resolve(context);
        public Reference(IToken<T, C> refersTo)
        {
            _refersTo = refersTo;
        }
    }
}