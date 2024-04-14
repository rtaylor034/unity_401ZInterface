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

    public static class Extensions
    {
        public static Proxies.Direct<IToken<R>, R> AsProxy<R>(this IToken<R> token) where R : class, ResObj
        {
            return new(token);
        }
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
        /// <summary>
        /// Expected to be [ : ]<see cref="Token.Unsafe.TokenFunction{T}"/>
        /// </summary>
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
