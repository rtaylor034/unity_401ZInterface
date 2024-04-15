using System.Collections;
using System.Collections.Generic;
using Perfection;
using System;
using ResObj = Resolution.IResolution;
using Token;

#nullable enable
namespace Proxy
{
    using Token.Unsafe;
    public interface IProxy<in TOrig, out R> : Unsafe.IProxy<R> where TOrig : IToken where R : class, ResObj
    {
        public IToken<R> Realize(TOrig original, Rule.IRule realizingRule);
    }
    public abstract record Proxy<TOrig, R> : IProxy<TOrig, R> where TOrig : IToken where R : class, ResObj
    {
        public abstract IToken<R> Realize(TOrig original, Rule.IRule realizingRule);
        public IToken<R> UnsafeTypedRealize(IToken original, Rule.IRule rule) => Realize((TOrig)original, rule);
        public IToken UnsafeRealize(IToken original, Rule.IRule rule) => UnsafeTypedRealize(original, rule);
    }
}
namespace Proxy.Unsafe
{
    using Token.Unsafe;
    public interface IProxy
    {
        public IToken UnsafeRealize(IToken original, Rule.IRule rule);
    }
    public interface IProxy<out R> : IProxy where R : class, ResObj
    {
        public abstract IToken<R> UnsafeTypedRealize(IToken original, Rule.IRule rule);
    }

    public abstract record FunctionProxy<TOrig, R> : Proxy<TOrig, R>
        where TOrig : IToken<R>
        where R : class, ResObj
    {
        protected List<IProxy> ArgProxies { get; init; }
        protected FunctionProxy(params IProxy[] proxies)
        {
            ArgProxies = new(proxies);
        }
        protected FunctionProxy(IEnumerable<IProxy> proxies)
        {
            ArgProxies = new(proxies);
        }
        public FunctionProxy(FunctionProxy<TOrig, R> original) : base(original)
        {
            ArgProxies = new(original.ArgProxies);
        }
        protected abstract TokenFunction<R> ConstructFromArgs(List<IToken> tokens);
        public override IToken<R> Realize(TOrig original, Rule.IRule rule) => ConstructFromArgs(MakeSubstitutions(original, rule));
        protected List<IToken> MakeSubstitutions(TOrig original, Rule.IRule rule)
        {
            return new(ArgProxies.Map(x => x.UnsafeRealize(original, rule)));
        }
    }
}
namespace Proxy.Creator
{
    // these objects exist only to aide in the process of creating proxies for rules.
    // it provides a system that allows C# to make type inferences all the way down; without it, creating proxies is generic-type specification hell.
    // i do not condone the terroristic acts committed in this namespace, but the token/proxy/rule system itself is air-tight, so if something is wrong, you should know.

    public interface IBase<out TFor, out TFor_, out R> where TFor : Token.Unsafe.IToken where R : class, ResObj { }
    public struct Base<TFor, R> : IBase<TFor, TFor, R> where TFor : Token.IToken<R> where R : class, ResObj
    {
        public readonly Proxies.Direct<TFor, R> AsIs(Token.IToken<R> token) => new(token);
        public readonly Proxies.SubEnvironment<TFor, R> SubEnvironment(params Proxy.Unsafe.IProxy[] envModifiers) => new(envModifiers);
        public readonly IFunction<TNew, TNew> TokenFunction<TNew>() where TNew : Token.Unsafe.TokenFunction<R>
        { return new Function<TNew>(); }
        public interface IFunction<out TNew, out TNew_> { }
        public struct Function<TNew> : IFunction<TNew, TNew> { }
    }

    public static class Extensions
    {
        public static Proxies.Function<TNew, TOrig, RArg1, ROut> WithArgs<TNew, TOrig, RArg1, ROut>
            (this Base<TOrig, ROut>.IFunction<Token.Function<RArg1, ROut>, TNew> _, IProxy<TOrig, RArg1> arg1)
            where TNew : Token.Function<RArg1, ROut>
            where TOrig : Token.IToken<ROut>
            where RArg1 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1); }
        public static Proxies.Function<TNew, TOrig, RArg1, RArg2, ROut> WithArgs<TNew, TOrig, RArg1, RArg2, ROut>
            (this Base<TOrig, ROut>.IFunction<Token.Function<RArg1, RArg2, ROut>, TNew> _, IProxy<TOrig, RArg1> arg1, IProxy<TOrig, RArg2> arg2)
            where TNew : Token.Function<RArg1, RArg2, ROut>
            where TOrig : Token.IToken<ROut>
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1, arg2); }
        public static Proxies.Function<TNew, TOrig, RArg1, RArg2, RArg3, ROut> WithArgs<TNew, TOrig, RArg1, RArg2, RArg3, ROut>
            (this Base<TOrig, ROut>.IFunction<Token.Function<RArg1, RArg2, RArg3, ROut>, TNew> _, IProxy<TOrig, RArg1> arg1, IProxy<TOrig, RArg2> arg2, IProxy<TOrig, RArg2> arg3)
            where TNew : Token.Function<RArg1, RArg2, RArg3, ROut>
            where TOrig : Token.IToken<ROut>
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where RArg3 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1, arg2, arg3); }
        // SHAKY
        public static Proxies.OriginalArg1<TOrig, RArg> OriginalArg1<TOrig, RArg, ROut>(this IBase<TOrig, Token.IHasArg1<RArg>, ROut> _)
            where TOrig : Token.IHasArg1<RArg>
            where RArg : class, ResObj
            where ROut : class, ResObj
        { return new(); }
        public static Proxies.OriginalArg2<TOrig, RArg> OriginalArg2<TOrig, RArg, ROut>(this IBase<TOrig, Token.IHasArg2<RArg>, ROut> _)
            where TOrig : Token.IHasArg2<RArg>
            where RArg : class, ResObj
            where ROut : class, ResObj
        { return new(); }
        public static Proxies.OriginalArg3<TOrig, RArg> OriginalArg3<TOrig, RArg, ROut>(this IBase<TOrig, Token.IHasArg3<RArg>, ROut> _)
            where TOrig : Token.IHasArg3<RArg>
            where RArg : class, ResObj
            where ROut : class, ResObj
        { return new(); }
        public static Proxies.Variable<TOrig, R> AsVariable<TOrig, R>(this IProxy<TOrig, R> proxy, string label)
            where TOrig : Token.Unsafe.IToken
            where R : class, ResObj
        { return new(label, proxy); }
    }
}