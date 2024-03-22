using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

namespace GameActions
{
    
    public interface IExpression<out P, in C> : Token.IToken<P, C>
        where P : IPacket
        where C : Context.IContextData
    { 
        
    }

    public interface IPacket { }
    public interface IEvaluation { }
}