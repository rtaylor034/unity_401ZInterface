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
        public IToken<R> Realize(TOrig original, Rule.IRule? realizingRule);
    }
    public abstract record Proxy<TOrig, R> : IProxy<TOrig, R> where TOrig : IToken where R : class, ResObj
    {
        public abstract IToken<R> Realize(TOrig original, Rule.IRule? realizingRule);
        public IToken<R> UnsafeTypedRealize(IToken original, Rule.IRule? rule) => Realize((TOrig)original, rule);
        public IToken UnsafeRealize(IToken original, Rule.IRule? rule) => UnsafeTypedRealize(original, rule);
    }

    public static class Create
    {
        public static IProxy<TFor, R> For<TFor, R>(Func<Creator.Base<TFor, R>, IProxy<TFor, R>> createStatement) where TFor : Token.IToken<R> where R : class, ResObj
        {
            return createStatement(new Creator.Base<TFor, R>());
        }

    }
}
namespace Proxy.Unsafe
{
    using Token.Unsafe;
    public interface IProxy
    {
        public IToken UnsafeRealize(IToken original, Rule.IRule? rule);
    }
    public interface IProxy<out R> : IProxy where R : class, ResObj
    {
        public abstract IToken<R> UnsafeTypedRealize(IToken original, Rule.IRule? rule);
    }

    public abstract record FunctionProxy<TOrig, R> : Proxy<TOrig, R>
        where TOrig : IToken
        where R : class, ResObj
    {
        public override IToken<R> Realize(TOrig original, Rule.IRule? rule) { return ConstructFromArgs(MakeSubstitutions(original, rule)); }

        protected readonly PList<IProxy> ArgProxies;
        protected abstract IToken<R> ConstructFromArgs(List<IToken> tokens);
        protected FunctionProxy(IEnumerable<IProxy> proxies)
        {
            ArgProxies = new() { Elements = proxies };
        }
        protected FunctionProxy(params IProxy[] proxies) : this(proxies as IEnumerable<IProxy>) { }
        protected List<IToken> MakeSubstitutions(TOrig original, Rule.IRule? rule)
        {
            return new(ArgProxies.Elements.Map(x => x.UnsafeRealize(original, rule)));
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
        public readonly Proxies.Direct<TFor, RThis> AsIs<RThis>(Token.IToken<RThis> token) where RThis : class, ResObj
        {
            return new(token);
        }
        public readonly ISubEnvironment<TNew, TNew> TokenEnvironment<TNew>() where TNew : Token.Unsafe.IFunction<R>
        { return new SubEnvironment<TNew>(); }
        public readonly IFunction<TNew, TNew> TokenFunction<TNew>() where TNew : Token.Unsafe.IFunction<R>
        { return new Function<TNew>(); }
        public interface ISubEnvironment<out TNew, out TNew_> { }
        public interface IFunction<out TNew, out TNew_> { }
        public struct Function<TNew> : IFunction<TNew, TNew> { }
        public struct SubEnvironment<TNew> : ISubEnvironment<TNew, TNew> { }
    }

    public static class Extensions
    {
        public static Proxies.Function<TNew, TOrig, RArg1, ROut> WithArgs<TNew, TOrig, RArg1, ROut>
            (this Base<TOrig, ROut>.IFunction<Token.IFunction<RArg1, ROut>, TNew> _, IProxy<TOrig, RArg1> arg1)
            where TNew : Token.IFunction<RArg1, ROut>
            where TOrig : Token.IToken<ROut>
            where RArg1 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1); }
        public static Proxies.Function<TNew, TOrig, RArg1, RArg2, ROut> WithArgs<TNew, TOrig, RArg1, RArg2, ROut>
            (this Base<TOrig, ROut>.IFunction<Token.IFunction<RArg1, RArg2, ROut>, TNew> _, IProxy<TOrig, RArg1> arg1, IProxy<TOrig, RArg2> arg2)
            where TNew : Token.IFunction<RArg1, RArg2, ROut>
            where TOrig : Token.IToken<ROut>
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1, arg2); }
        public static Proxies.Function<TNew, TOrig, RArg1, RArg2, RArg3, ROut> WithArgs<TNew, TOrig, RArg1, RArg2, RArg3, ROut>
            (this Base<TOrig, ROut>.IFunction<Token.IFunction<RArg1, RArg2, RArg3, ROut>, TNew> _, IProxy<TOrig, RArg1> arg1, IProxy<TOrig, RArg2> arg2, IProxy<TOrig, RArg2> arg3)
            where TNew : Token.IFunction<RArg1, RArg2, RArg3, ROut>
            where TOrig : Token.IToken<ROut>
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where RArg3 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1, arg2, arg3); }
        public static Proxies.CombinerTransform<TNew, TOrig, RArgs, ROut> WithOriginalArgs<TNew, TOrig, RArgs, ROut>
            (this Base<TOrig, ROut>.IFunction<Token.ICombiner<RArgs, ROut>, TNew> _)
            where TNew : Token.ICombiner<RArgs, ROut>
            where TOrig : Token.ICombiner<RArgs, ROut>
            where RArgs : class, ResObj
            where ROut : class, ResObj
        { return new(); }
        public static Proxies.Combiner<TNew, TOrig, RArgs, ROut> WithArgs<TNew, TOrig, RArgs, ROut>
            (this Base<TOrig, ROut>.IFunction<Token.ICombiner<RArgs, ROut>, TNew> _, params IProxy<TOrig, RArgs>[] args)
            where TNew : Token.ICombiner<RArgs, ROut>
            where TOrig : Token.IToken<ROut>
            where RArgs : class, ResObj
            where ROut : class, ResObj
        { return new(args); }
        public static Proxies.Combiner<TNew, TOrig, RArgs, ROut> WithArgs<TNew, TOrig, RArgs, ROut>
            (this Base<TOrig, ROut>.IFunction<Token.ICombiner<RArgs, ROut>, TNew> _, IEnumerable<IProxy<TOrig, RArgs>> args)
            where TNew : Token.ICombiner<RArgs, ROut>
            where TOrig : Token.IToken<ROut>
            where RArgs : class, ResObj
            where ROut : class, ResObj
        { return new(args); }
        public static Proxies.SubEnvironment<TNew, TOrig, REnv, ROut> Environment<TNew, TOrig, REnv, ROut>
            (this Base<TOrig, ROut>.ISubEnvironment<Token.SubEnvironment<REnv, ROut>, TNew> _, params IProxy<TOrig, REnv>[] envModifiers)
            where TNew : Token.SubEnvironment<REnv, ROut>
            where TOrig : Token.IToken<ROut>
            where REnv : Resolution.Operation
            where ROut : class, ResObj
        { return new(envModifiers); }
        public static Proxies.SubEnvironment<TNew, TOrig, REnv, ROut> Environment<TNew, TOrig, REnv, ROut>
            (this Base<TOrig, ROut>.ISubEnvironment<Token.SubEnvironment<REnv, ROut>, TNew> _, IEnumerable<IProxy<TOrig, REnv>> envModifiers)
            where TNew : Token.SubEnvironment<REnv, ROut>
            where TOrig : Token.IToken<ROut>
            where REnv : Resolution.Operation
            where ROut : class, ResObj
        { return new(envModifiers); }
        //TODO: combiner transform, combiner, subenvironment.

        public static Proxies.OriginalArg1<TOrig, RArg> OriginalArg1<TOrig, RArg, ROut>(this IBase<TOrig, Token.Unsafe.IHasArg1<RArg>, ROut> _)
            where TOrig : Token.Unsafe.IHasArg1<RArg>
            where RArg : class, ResObj
            where ROut : class, ResObj
        { return new(); }
        public static Proxies.OriginalArg2<TOrig, RArg> OriginalArg2<TOrig, RArg, ROut>(this IBase<TOrig, Token.Unsafe.IHasArg2<RArg>, ROut> _)
            where TOrig : Token.Unsafe.IHasArg2<RArg>
            where RArg : class, ResObj
            where ROut : class, ResObj
        { return new(); }
        public static Proxies.OriginalArg3<TOrig, RArg> OriginalArg3<TOrig, RArg, ROut>(this IBase<TOrig, Token.Unsafe.IHasArg3<RArg>, ROut> _)
            where TOrig : Token.Unsafe.IHasArg3<RArg>
            where RArg : class, ResObj
            where ROut : class, ResObj
        { return new(); }
        public static Proxies.Variable<TOrig, R> AsVariable<TOrig, R>(this IProxy<TOrig, R> proxy, string label)
            where TOrig : Token.Unsafe.IToken
            where R : class, ResObj
        { return new(label, proxy); }
    }
}