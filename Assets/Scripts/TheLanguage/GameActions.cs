using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

namespace GameActions
{
    public class ExpressionSet<C> where C : Context.IContext
    {
        private List<IExpression<IPacket, C>> _expressions;

        public ExpressionSet()
        {
            _expressions = new();
        }
    }
    public interface IExpression<out P, in C> : Context.Token.IToken<P, C>
        where P : IPacket
        where C : Context.IContext
    {

    }
    public interface IPacket { }
    public interface IEvaluation { }
}