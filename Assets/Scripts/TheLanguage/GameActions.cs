using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

namespace GameActions
{
    
    public interface IExpression<out R, in C> : Token.IToken<R, C>
        where R : IResolution
        where C : Context.IContextData
    { 
        
    }
    
    public interface IPacket<out R> : ResolutionProtocol.IProtocol<R> where R : IResolution { }
    public interface IResolution { }
}