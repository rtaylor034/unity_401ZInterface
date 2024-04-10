using System;

#nullable enable
// A 'Rule' is just a wrapper around a proxy that can 'try' to apply it.
namespace Rule
{
    using Proxy;
    using ResObj = Resolution.Resolution;
    
    public interface IRule
    {
        public Token.IToken<R>? TryApplyTyped<R>(Token.IToken<R> original) where R : ResObj;
        public Token.Unsafe.IToken? TryApply(Token.Unsafe.IToken original);
    }
    public record Rule<TFor, R> : IRule where TFor : Token.IToken<R> where R : ResObj
    {
        private IProxy<TFor, R> _proxy { get; init; }
        public Rule(IProxy<TFor, R> proxy)
        {
            _proxy = proxy;
        }
        public Token.IToken<R> Apply(TFor original)
        {
            return _proxy.Realize(original);
        }
        public Token.Unsafe.IToken? TryApply(Token.Unsafe.IToken original)
        {
            return (original is TFor match) ? Apply(match) : null;
        }
        public Token.IToken<ROut>? TryApplyTyped<ROut>(Token.IToken<ROut> original) where ROut : ResObj
        {
            return (original is TFor match) ? (Token.IToken<ROut>)Apply(match) : null;
        }
    }
    public static class Create
    {
        public static Rule<TFor, R> For<TFor, R>(Func<Creator.Base<TFor, R>, IProxy<TFor, R>> createStatement) where TFor : Token.IToken<R> where R : ResObj
        {
            return new(createStatement(new Creator.Base<TFor, R>()));
        }

    }
    namespace Creator
    {
        // these objects exist only to aide in the process of creating proxies.
        // it provides a system that allows C# to make type inferences all the way down; without it, creating proxies is generic-type specification hell.
        // i do not condone the terroristic acts committed in this namespace, but the token/proxy/rule system itself is air-tight, so if something is wrong, you should know.

        public interface IBase<out TFor, out TFor_, out R> where TFor : Token.Unsafe.IToken where R : ResObj { }
        public struct Base<TFor, R> : IBase<TFor, TFor, R> where TFor : Token.IToken<R> where R : ResObj
        {
            public readonly Proxies.Direct<TFor, R> AsIs(Token.IToken<R> token) => new(token);
            public readonly IFunction<TNew, TNew> TokenFunction<TNew>() where TNew : Token.Unsafe.TokenFunction<R>
            { return new Function<TNew>(); }
            public interface IFunction<out TNew, out TNew_>{ }
            public struct Function<TNew> : IFunction<TNew, TNew> { }
        }
        
        public static class Extensions
        {
            public static Proxies.Function<TNew, TOrig, RArg1, ROut> WithArgs<TNew, TOrig, RArg1, ROut>
                (this Base<TOrig, ROut>.IFunction<Token.Function<RArg1, ROut>, TNew> _, IProxy<TOrig, RArg1> arg1)
                where TNew : Token.Function<RArg1, ROut>
                where TOrig : Token.IToken<ROut>
                where RArg1 : ResObj
                where ROut : ResObj
            { return new(arg1); }
            public static Proxies.Function<TNew, TOrig, RArg1, RArg2, RArg3, ROut> WithArgs<TNew, TOrig, RArg1, RArg2, RArg3, ROut>
                (this Base<TOrig, ROut>.IFunction<Token.Function<RArg1, RArg2, RArg3, ROut>, TNew> _, IProxy<TOrig, RArg1> arg1, IProxy<TOrig, RArg2> arg2, IProxy<TOrig, RArg2> arg3)
                where TNew : Token.Function<RArg1, RArg2, RArg3, ROut>
                where TOrig : Token.IToken<ROut>
                where RArg1 : ResObj
                where RArg2 : ResObj
                where RArg3 : ResObj
                where ROut : ResObj
            { return new(arg1, arg2, arg3); }
            public static Proxies.Function<TNew, TOrig, RArg1, RArg2, ROut> WithArgs<TNew, TOrig, RArg1, RArg2, ROut>
                (this Base<TOrig, ROut>.IFunction<Token.Function<RArg1, RArg2, ROut>, TNew> _, IProxy<TOrig, RArg1> arg1, IProxy<TOrig, RArg2> arg2)
                where TNew : Token.Function<RArg1, RArg2, ROut>
                where TOrig : Token.IToken<ROut>
                where RArg1 : ResObj
                where RArg2 : ResObj
                where ROut : ResObj
            { return new(arg1, arg2); }

            // SHAKY
            public static Proxies.OriginalArg1<TOrig, RArg> OriginalArg1<TOrig, RArg, ROut>(this IBase<TOrig, Token.IHasArg1<RArg>, ROut> _)
                where TOrig : Token.IHasArg1<RArg>
                where RArg : ResObj
                where ROut : ResObj
            { return new(); }
            public static Proxies.OriginalArg2<TOrig, RArg> OriginalArg2<TOrig, RArg, ROut>(this IBase<TOrig, Token.IHasArg2<RArg>, ROut> _)
                where TOrig : Token.IHasArg2<RArg>
                where RArg : ResObj
                where ROut : ResObj
            { return new(); }
            public static Proxies.OriginalArg3<TOrig, RArg> OriginalArg3<TOrig, RArg, ROut>(this IBase<TOrig, Token.IHasArg3<RArg>, ROut> _)
                where TOrig : Token.IHasArg3<RArg>
                where RArg : ResObj
                where ROut : ResObj
            { return new(); }
        }
    }
    
}