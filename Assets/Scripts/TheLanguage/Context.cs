using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Context
{
    public interface IContextData { }

    namespace Any
    {
        public class Data : IContextData
        {

        }
        namespace Tokens
        {

        }
    }
    namespace Global
    {
        public class Data : Any.Data
        {
            public int TestValue;
        }
        namespace Tokens
        {
            public sealed class TestValue : Token.IToken<int, Data> { public int Resolve(Data context) => context.TestValue; }
        }
    }
    namespace Ability
    {
        public class Data : Global.Data
        {
            public Unit Source;
            public Unit Target;
        }
        namespace Tokens
        {
            public sealed class Source : Token.IToken<Unit, Data> { public Unit Resolve(Data context) => context.Source; }
            public sealed class Target : Token.IToken<Unit, Data> { public Unit Resolve(Data context) => context.Target; }
        }
    }

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
}