using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
// A 'Rule' is just a wrapper around a proxy that can 'try' to apply it.
namespace Rule
{
    using ResObj = Resolution.Resolution;
    
    public interface IRule
        {
            public Token.Unsafe.IToken TryApply(Token.Unsafe.IToken original, out bool matched);
        }
    public record Rule<TFor, R> : IRule where TFor : Token.IToken<R> where R : ResObj
    {
        private Proxy.Proxy<TFor, R> _proxy { get; init; }
        public Rule(Proxy.Proxy<TFor, R> proxy)
        {
            _proxy = proxy;
        }
        public Token.IToken<R> Apply(TFor original)
        {
            return _proxy.Realize(original);
        }
        public Token.Unsafe.IToken TryApply(Token.Unsafe.IToken original, out bool matched)
        {
            //theres gotta be a way to make this 1 line.
            matched = (original is TFor);
            return (original is TFor matchingToken) ? Apply(matchingToken) : original;
        }
    }
    public static class Create
    {

    }
}