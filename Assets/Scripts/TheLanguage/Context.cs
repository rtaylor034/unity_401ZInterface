using System;
using System.Collections;
using System.Collections.Generic;
using Token;
using UnityEngine;
using UnityEngine.EventSystems;
using Expressions.References.Identifier;
using Context.Global.Tokens;

namespace Context
{
    public abstract class ContextReference<T> : Expressions.References.Token<T>
    {
        protected ContextReference(Type typeId) : base(new Contextual(typeId)) { }
    }
    namespace Global
    {
        public static class Data
        {
            public readonly static Expressions.References.Map RefMap = new(new Expressions.References.IProvider.None(), new()
            {
                { new Contextual(typeof(AllUnits)), new Expressions.References.Referable() }
            });
        }
        namespace Tokens
        {
            public sealed class AllUnits : ContextReference<IEnumerable<Unit>>
            {
                public AllUnits() : base(typeof(AllUnits)) { }
            }
        }
        public abstract class Expression<T> : Expressions.Expression<T>
        {
            public Expression(IToken<T> token) : base(Data.RefMap, token) { }
        }
    }

    public static class Methods
    {
        public static string FormatIdentifier(string name)
        {
            return "#" + name.ToUpper();
        }
    }
}