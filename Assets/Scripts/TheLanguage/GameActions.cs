using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameActions
{
    public abstract class Expression : Expressions.Expression<IEnumerable<IResolution>>
    {
        public Expression(Token.IToken<IEnumerable<IResolution>> token) : base(Expressions.Context.GLOBAL, token) { }
    }
    public interface IResolution { }
}