using System;

#nullable enable
// A 'Rule' is just a wrapper around a proxy that can 'try' to apply it.
namespace Rule
{
    using Proxy;
    using ResObj = Resolution.Resolution;
    
    public interface IRule
        {
            public Token.Unsafe.IToken TryApply(Token.Unsafe.IToken original, out bool matched);
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
        public Token.Unsafe.IToken TryApply(Token.Unsafe.IToken original, out bool matched)
        {
            //theres gotta be a way to make this 1 line.
            matched = (original is TFor);
            return (original is TFor matchingToken) ? Apply(matchingToken) : original;
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
        using Token;

        // SHAKY: should technically be 'TFor : Token<ROut>', but OriginalArg type inference wont work.
        public interface IBase<out TFor, out R> where TFor : Token.Unsafe.IToken where R : ResObj { }
        public struct Base<TFor, R> : IBase<TFor, R> where TFor : IToken<R> where R : ResObj
        {
            public readonly Proxies.Direct<TFor, R> AsIs(IToken<R> token) => new(token);
            public readonly Function<TNew> TokenFunction<TNew>() where TNew : Token.Unsafe.TokenFunction<R>
            { return new(); }
            public struct Function<TNew> { }
        }
        
        public static class Extensions
        {
            public static Proxies.Function<TNew, TOrig, RArg1, RArg2, ROut> WithArgs<TNew, TOrig, RArg1, RArg2, ROut>
                (this Base<TOrig, ROut>.Function<TNew> _, IProxy<TOrig, RArg1> arg1, IProxy<TOrig, RArg2> arg2)
                where TNew : Token.Function<RArg1, RArg2, ROut>
                where TOrig : IToken<ROut>
                where RArg1 : ResObj
                where RArg2 : ResObj
                where ROut : ResObj
            { return new(arg1, arg2); }

            // SHAKY
            public static Proxies.OriginalArg1<Token.IHasArg1<RArg>, RArg> OriginalArg1<RArg, ROut>(this IBase<Token.IHasArg1<RArg>, ROut> _)
                where RArg : ResObj
                where ROut : ResObj
            { return new(); }
            public static Proxies.OriginalArg2<Token.IHasArg2<RArg>, RArg> OriginalArg2<RArg, ROut>(this IBase<Token.IHasArg2<RArg>, ROut> _)
                where RArg : ResObj
                where ROut : ResObj
            { return new(); }
            public static Proxies.OriginalArg3<Token.IHasArg3<RArg>, RArg> OriginalArg3<RArg, ROut>(this IBase<Token.IHasArg3<RArg>, ROut> _)
                where RArg : ResObj
                where ROut : ResObj
            { return new(); }
        }
    }
    
}