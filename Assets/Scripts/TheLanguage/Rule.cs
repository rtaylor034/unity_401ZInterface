using System;
using ResObj = Resolution.IResolution;

#nullable enable
// A 'Rule' is just a wrapper around a proxy that can 'try' to apply it.
namespace Rule
{
    using Proxy;
    
    public interface IRule
    {
        public Token.IToken<R>? TryApplyTyped<R>(Token.IToken<R> original) where R : class, ResObj;
        public Token.Unsafe.IToken? TryApply(Token.Unsafe.IToken original);
    }

    public record Rule<TFor, R> : IRule where TFor : Token.IToken<R> where R : class, ResObj
    {
        public Rule(IProxy<TFor, R> proxy)
        {
            _proxy = proxy;
        }
        public Token.IToken<R> Apply(TFor original)
        {
            return _proxy.Realize(original, this);
        }
        public Token.Unsafe.IToken? TryApply(Token.Unsafe.IToken original)
        {
            return (original is TFor match) ? Apply(match) : null;
        }
        public Token.IToken<ROut>? TryApplyTyped<ROut>(Token.IToken<ROut> original) where ROut : class, ResObj
        {
            return (original is TFor match) ? (Token.IToken<ROut>)Apply(match) : null;
        }
        private readonly IProxy<TFor, R> _proxy;
    }
    
}