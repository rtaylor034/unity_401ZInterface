using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Context
{
    public interface IContext { }

    public class Any : IContext { }
    public class Global : Any
    {

    }
    public class Ability : Global
    {

    }

    namespace Token
    {
        public interface IToken<out T, in C> where C : IContext
        {
            public T Resolve(GameState game, C context);
        }
        public class Reference<T, C> : IToken<T, C> where C : IContext
        {
            private IToken<T, C> _refersTo;
            public T Resolve(GameState game, C context) => _refersTo.Resolve(game, context);
            public Reference(Global _, IToken<T, C> refersTo)
            {
                _refersTo = refersTo;
            }
        }
    }
}