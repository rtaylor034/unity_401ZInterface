using System;O
using System.Collections;
using System.Collections.Generic;
using Token;
using UnityEngine;
using UnityEngine.EventSystems;
using TypeID = Expressions.References.Identifier.Contextual;
using Context.Global.Tokens;

//bro I *SWEAR* this shit isnt as convoluted as it looks.

// EVERYTHING SHOULD BE DEFINED IN GAMESETTINGS. AAAA
namespace Context
{
    public abstract class ContextReference<T> : Expressions.References.Token<T>
    {
        protected ContextReference(Type type) : base(new TypeID(type)) { }
    }
    namespace AbilityUse
    {
        //whole lotta boilerplate :D
        public static class Data
        {
            public readonly static Expressions.References.Map REFMAP = new(new Expressions.References.IProvider.None(), new()
            {
                { new TypeID(typeof(Tokens.Source)), new Expressions.References.Referable() }
            });
        }
        public sealed class Expression<T> : Expressions.Expression<T>
        {
            public Expression(IToken<T> token) : base(Data.REFMAP, token) { }
        }

        namespace Tokens
        {
            public sealed class Source : ContextReference<Unit>
            {
                public Source() : base(typeof(Source)) { }
            }
        }
    }
}