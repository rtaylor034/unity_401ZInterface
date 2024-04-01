using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DefinedID = Expressions.Reference.Identifier.Defined;

namespace Ability
{
    public interface IAbility
    {
        public Token.IToken<IEnumerable<GameActions.IResolution>> ActionToken { get; }
    }
    namespace Attack
    {
        public sealed class Data : IAbility
        {
            public Token.IToken<IEnumerable<GameActions.IResolution>> ActionToken { get; private set; }
            public Data(Token.IToken<IEnumerable<GameActions.IResolution>> token)
            {
                ActionToken = token;
            }
        }
        namespace DefinedTokens
        {
            public sealed class Source : Token.Reference<Unit>
            {
                public Source() : base(new DefinedID(typeof(Source))) { }
            }
            public sealed class Target : Token.Reference<Unit>
            {
                public Target() : base(new DefinedID(typeof(Target))) { }
            }
        }
    }
}
