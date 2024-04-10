using System;

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
        public static Rule<TFor, R> For<TFor, R>(Func<Creator.Base<TFor, R>, Proxy.Proxy<TFor, R>> createStatement) where TFor : Token.IToken<R> where R : ResObj
        {
            return new(createStatement(new Creator.Base<TFor, R>()));
        }

    }
    namespace Creator
    {
        using Token.Unsafe;
        using Proxy.Unsafe;
        using Token;
        using UnityEngine.UIElements;

        public interface IBase<out TFor, out R> where TFor : IToken<R> where R : ResObj { }
        public struct Base<TFor, R> : IBase<TFor, R> where TFor : IToken<R> where R : ResObj
        {
            public readonly Proxies.Direct<TFor, R> AsIs(IToken<R> token) => new(token);
            public readonly Function<TNew> TokenFunction<TNew>() where TNew : TokenFunction<R>
            { return new(); }
            public struct Function<TNew> { }
        }
        
        public static class Extensions
        {
            public static Proxies.Function<TNew, TOrig, RArg1, RArg2, ROut> WithArgs<TNew, TOrig, RArg1, RArg2, ROut>
                (this Base<TOrig, ROut>.Function<TNew> _, ArgProxy<TOrig, RArg1> arg1, ArgProxy<TOrig, RArg2> arg2)
                where TNew : Token.Function<RArg1, RArg2, ROut>
                where TOrig : IToken<ROut>
                where RArg1 : ResObj
                where RArg2 : ResObj
                where ROut : ResObj
            { return new(arg1, arg2); }

            public static Proxies.OriginalArg1<RArg, ROut> OriginalArg1<RArg, ROut>(this IBase<Token.IHasArg1<RArg, ROut>, ROut> _)
                where RArg : ResObj
                where ROut : ResObj
            { return new(); }
            public static Proxies.OriginalArg2<RArg, ROut> OriginalArg2<TOrig, RArg, ROut>(this IBase<TOrig, ROut> _)
                where TOrig : Token.IHasArg2<RArg, ROut>
                where RArg : ResObj
                where ROut : ResObj
            { return new(); }
            public static Proxies.OriginalArg3<RArg, ROut> OriginalArg3<TOrig, RArg, ROut>(this IBase<TOrig, ROut> _)
                where TOrig : Token.IHasArg3<RArg, ROut>
                where RArg : ResObj
                where ROut : ResObj
            { return new(); }
        }
    }
    
}