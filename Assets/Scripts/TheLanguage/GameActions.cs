using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

namespace GameActions
{
    public class ExpressionSet<C> where C : Context.IContextData
    {
        public List<IExpression<IPacket, C>> _expressions;

        public ExpressionSet()
        {
            _expressions = new();
        }
    }
    public interface IExpression<out P, in C> : Token.IToken<P, C>
        where P : IPacket
        where C : Context.IContextData
    { }

    public interface IPacket { }
    public interface IEvaluation { }
}