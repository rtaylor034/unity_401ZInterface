using System;O
using System.Collections;
using System.Collections.Generic;
using Token;
using UnityEngine;
using UnityEngine.EventSystems;
using TypeID = Expressions.Reference.Identifier.Defined;

//bro I *SWEAR* this shit isnt as convoluted as it looks.
// EVERYTHING SHOULD BE DEFINED IN GAMESETTINGS. AAAA
namespace Context
{
    public abstract class ContextReference<T> : Token.Reference<T>
    {
        protected ContextReference(Type type) : base(new TypeID(type)) { }
    }
    namespace AbilityUse
    {
        public sealed class Expression : Expressions.Expression<IEnumerable<GameActions.IResolution>>
        {
            public Expression(IToken<IEnumerable<GameActions.IResolution>> token, Ability.IAbility ability) : base(Data.REFMAP, token) { }
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