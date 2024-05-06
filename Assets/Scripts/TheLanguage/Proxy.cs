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
    public interface IProxy<in TOrig, out R> : Unsafe.IProxyOf<TOrig>, Unsafe.IProxy<R> where TOrig : IToken where R : class, ResObj
    {
        public IToken<R> Realize(TOrig original, Rule.IRule? realizingRule);
    }
    public abstract record Proxy<TOrig, R> : IProxy<TOrig, R> where TOrig : IToken where R : class, ResObj
    {
        public abstract IToken<R> Realize(TOrig original, Rule.IRule? realizingRule);
        public IToken<R> UnsafeTypedRealize(IToken original, Rule.IRule? rule) { return Realize((TOrig)original, rule); }
        public IToken UnsafeContextualRealize(TOrig original, Rule.IRule? rule) { return Realize(original, rule); }
        public IToken UnsafeRealize(IToken original, Rule.IRule? rule) => UnsafeTypedRealize(original, rule);
    }

    public static class Create
    {
        public static IProxy<TFor, R> For<TFor, R>(Creator.Statement<TFor, R> createStatement) where TFor : Token.IToken<R> where R : class, ResObj
        {
            return createStatement(new Creator.Base<TFor>());
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
    public interface IProxyOf<in TOrig> : IProxy where TOrig : IToken
    {
        public abstract IToken UnsafeContextualRealize(TOrig original, Rule.IRule? rule);
    }

    public abstract record FunctionProxy<TOrig, R> : Proxy<TOrig, R>
        where TOrig : IToken
        where R : class, ResObj
    {
        public sealed override IToken<R> Realize(TOrig original, Rule.IRule? rule) { return ConstructFromArgs(original, MakeSubstitutions(original, rule)); }

        protected readonly PList<IProxy> ArgProxies;
        protected abstract IToken<R> ConstructFromArgs(TOrig original, List<IToken> tokens);
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
    using IToken = Token.Unsafe.IToken;
    // these objects exist only to aide in the process of creating proxies for rules.
    // it provides a system that allows C# to make type inferences all the way down; without it, creating proxies is generic-type specification hell.
    // i do not condone the terroristic acts committed in this namespace, but the token/proxy/rule system itself is air-tight, so if something is wrong, you should know.
    public delegate IProxy<TFor, R> Statement<TFor, R>(Base<TFor> p) where TFor : IToken where R : class, ResObj;
    public interface IBase<out TFor, out TFor_> where TFor : IToken { }
    public struct Base<TFor> : IBase<TFor, TFor> where TFor : IToken
    {
        public readonly Proxies.Direct<TFor, R> AsIs<R>(Token.IToken<R> token) where R : class, ResObj
        {
            return new(token);
        }
        public readonly IMaker<TNew, TNew> Function<TNew>() where TNew : Token.Unsafe.IFunction
        { return new Maker<TNew>(); }

        public readonly Proxies.IfElse<TFor, R> IfElse<R>(Statement<TFor, Resolutions.Bool> condition) where R : class, ResObj
        {
            return new(condition(new()));
        }
        public readonly Proxies.SubEnvironment<TFor, R> SubEnvironment<R>(IEnumerable<Statement<TFor, ResObj>> envModifiers) where R : class, ResObj
        {
            return new(envModifiers.Map(x => x(new())));
        }
        public readonly Proxies.SubEnvironment<TFor, R> SubEnvironment<R>(params Statement<TFor, ResObj>[] envModifiers) where R : class, ResObj => SubEnvironment<R>(envModifiers.IEnumerable());
        public interface IMaker<out TNew, out TNew_> { }
        public struct Maker<TNew> : IMaker<TNew, TNew> { }
    }
    public static class Extensions
    {
        public static Proxies.RecursiveCall<RArg1, ROut> Recurse<RArg1, ROut>(this Base<Tokens.Recursive<RArg1, ROut>> _, Statement<Tokens.Recursive<RArg1, ROut>, RArg1> arg1)
            where RArg1 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1(new())); }
        public static Proxies.RecursiveCall<RArg1, RArg2, ROut> Recurse<RArg1, RArg2, ROut>(this Base<Tokens.Recursive<RArg1, RArg2, ROut>> _, Statement<Tokens.Recursive<RArg1, RArg2, ROut>, RArg1> arg1, Statement<Tokens.Recursive<RArg1, RArg2, ROut>, RArg2> arg2)
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1(new()), arg2(new())); }
        public static Proxies.RecursiveCall<RArg1, RArg2, RArg3, ROut> Recurse<RArg1, RArg2, RArg3, ROut>(this Base<Tokens.Recursive<RArg1, RArg2, RArg3, ROut>> _, Statement<Tokens.Recursive<RArg1, RArg2, RArg3, ROut>, RArg1> arg1, Statement<Tokens.Recursive<RArg1, RArg2, RArg3, ROut>, RArg2> arg2, Statement<Tokens.Recursive<RArg1, RArg2, RArg3, ROut>, RArg3> arg3)
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where RArg3 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1(new()), arg2(new()), arg3(new())); }

        public static Proxies.Function<TNew, TOrig, RArg1, ROut> WithArgs<TNew, TOrig, RArg1, ROut>
            (this Base<TOrig>.IMaker<Token.IFunction<RArg1, ROut>, TNew> _, Statement<TOrig, RArg1> arg1)
            where TNew : Token.IFunction<RArg1, ROut>
            where TOrig : IToken
            where RArg1 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1(new())); }
        public static Proxies.Function<TNew, TOrig, RArg1, RArg2, ROut> WithArgs<TNew, TOrig, RArg1, RArg2, ROut>
            (this Base<TOrig>.IMaker<Token.IFunction<RArg1, RArg2, ROut>, TNew> _, Statement<TOrig, RArg1> arg1, Statement<TOrig, RArg2> arg2)
            where TNew : Token.IFunction<RArg1, RArg2, ROut>
            where TOrig : IToken
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1(new()), arg2(new())); }
        public static Proxies.Function<TNew, TOrig, RArg1, RArg2, RArg3, ROut> WithArgs<TNew, TOrig, RArg1, RArg2, RArg3, ROut>
            (this Base<TOrig>.IMaker<Token.IFunction<RArg1, RArg2, RArg3, ROut>, TNew> _, Statement<TOrig, RArg1> arg1, Statement<TOrig, RArg2> arg2, Statement<TOrig, RArg2> arg3)
            where TNew : Token.IFunction<RArg1, RArg2, RArg3, ROut>
            where TOrig : IToken
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where RArg3 : class, ResObj
            where ROut : class, ResObj
        { return new(arg1(new()), arg2(new()), arg3(new())); }

        public static Proxies.CombinerTransform<TNew, TOrig, RArgs, ROut> WithOriginalArgs<TNew, TOrig, RArgs, ROut>
            (this Base<TOrig>.IMaker<Token.ICombiner<RArgs, ROut>, TNew> _)
            where TNew : Token.ICombiner<RArgs, ROut>
            where TOrig : Token.ICombiner<RArgs, ROut>
            where RArgs : class, ResObj
            where ROut : class, ResObj
        { return new(); }
        
        public static Proxies.Combiner<TNew, TOrig, RArgs, ROut> WithArgs<TNew, TOrig, RArgs, ROut>
            (this Base<TOrig>.IMaker<Token.ICombiner<RArgs, ROut>, TNew> _, IEnumerable<Statement<TOrig, RArgs>> args)
            where TNew : Token.ICombiner<RArgs, ROut>
            where TOrig : IToken
            where RArgs : class, ResObj
            where ROut : class, ResObj
        { return new(args.Map(x => x(new()))); }
        public static Proxies.Combiner<TNew, TOrig, RArgs, ROut> WithArgs<TNew, TOrig, RArgs, ROut>
            (this Base<TOrig>.IMaker<Token.ICombiner<RArgs, ROut>, TNew> _, params Statement<TOrig, RArgs>[] args)
            where TNew : Token.ICombiner<RArgs, ROut>
            where TOrig : IToken
            where RArgs : class, ResObj
            where ROut : class, ResObj
        => WithArgs(_, args.IEnumerable());

        public static Proxies.Accumulator<TNew, TOrig, RElement, RGen, RInto> OverTokens<TNew, TOrig, RElement, RGen, RInto>
            (this Base<TOrig>.IMaker<Token.Accumulator<RElement, RGen, RInto>, TNew> _, Statement<TOrig, Resolution.IMulti<RElement>> iterator, out VariableIdentifier<RElement> elementIdentifier, Statement<TOrig, RGen> generator)
            where TNew : Token.Accumulator<RElement, RGen, RInto>
            where TOrig : IToken
            where RElement : class, ResObj
            where RGen : class, ResObj
            where RInto : class, ResObj
        {
            elementIdentifier = new();
            return new(iterator(new()), elementIdentifier, generator(new()));
        }

        public static Proxies.OriginalArg1<TOrig, RArg> OriginalArg1<TOrig, RArg>(this IBase<TOrig, Token.Unsafe.IHasArg1<RArg>> _)
            where TOrig : Token.Unsafe.IHasArg1<RArg>
            where RArg : class, ResObj
        { return new(); }
        public static Proxies.OriginalArg2<TOrig, RArg> OriginalArg2<TOrig, RArg>(this IBase<TOrig, Token.Unsafe.IHasArg2<RArg>> _)
            where TOrig : Token.Unsafe.IHasArg2<RArg>
            where RArg : class, ResObj
        { return new(); }
        public static Proxies.OriginalArg3<TOrig, RArg> OriginalArg3<TOrig, RArg>(this IBase<TOrig, Token.Unsafe.IHasArg3<RArg>> _)
            where TOrig : Token.Unsafe.IHasArg3<RArg>
            where RArg : class, ResObj
        { return new(); }

        public static Proxies.Variable<TOrig, R> Variable<TOrig, R>(this Base<TOrig> _, out VariableIdentifier<R> newIdentifier, Statement<TOrig, R> proxy)
            where TOrig : Token.Unsafe.IToken
            where R : class, ResObj
        {
            newIdentifier = new();
            return new(newIdentifier, proxy(new()));
        }
    }
}
