using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perfection;
using System;
using ResObj = Resolution.Resolution;
using Token;

#nullable enable
namespace Proxy
{
    using Token.Unsafe;
    public interface IProxy<in TOrig, out R> : Unsafe.IProxy<R> where TOrig : IToken where R : ResObj
    {
        public IToken<R> Realize(TOrig original, Rule.IRule realizingRule);
    }
    public abstract record Proxy<TOrig, R> : IProxy<TOrig, R> where TOrig : IToken where R : ResObj
    {
        public abstract IToken<R> Realize(TOrig original, Rule.IRule realizingRule);
        public IToken<R> UnsafeTypedRealize(IToken original, Rule.IRule rule) => Realize((TOrig)original, rule);
        public IToken UnsafeRealize(IToken original, Rule.IRule rule) => UnsafeTypedRealize(original, rule);
    }

    public static class Extensions
    {
        public static Proxies.Direct<IToken<R>, R> AsProxy<R>(this IToken<R> token) where R : ResObj
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
    public interface IProxy<out R> : IProxy where R : ResObj
    {
        public abstract IToken<R> UnsafeTypedRealize(IToken original, Rule.IRule rule);
    }

    public abstract record FunctionProxy<TOrig, R> : Proxy<TOrig, R>
        where TOrig : IToken<R>
        where R : ResObj
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
namespace Proxies
{
    using Proxy;
    using Token;
    using Token.Unsafe;
    using Proxy.Unsafe;
    using GExtensions;

    public record Direct<TOrig, R> : Proxy<TOrig, R> where TOrig : IToken<R> where R : ResObj
    {
        protected IToken<R> Token { get; init; }
        public override IToken<R> Realize(TOrig _, Rule.IRule __) => Token;
        public Direct(IToken<R> token) => Token = token;
        public Direct<TTo, R> Fix<TTo>() where TTo : IToken<R> => new(Token);
    }

    public sealed record CombinerTransform<TNew, TOrig, RArg, ROut> : Proxy<TOrig, ROut>
        where TOrig : Token.IHasCombineArgs<RArg>
        where TNew : Token.Combiner<RArg, ROut>
        where RArg : ResObj
        where ROut : ResObj
    {
        public override IToken<ROut> Realize(TOrig original, Rule.IRule rule)
        {
            var remapped = original.Args.Map(x => x.ApplyRules(rule.Wrapped()));
            return (TNew)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArg>>) })
                .Invoke(new object[] { remapped });
        }
    }
    public record Combiner<TNew, TOrig, RArg, ROut> : FunctionProxy<TOrig, ROut>
        where TNew : Token.Combiner<RArg, ROut>
        where TOrig : IToken<ROut>
        where RArg : ResObj
        where ROut : ResObj
    {
        public Combiner(IEnumerable<IProxy<TOrig, RArg>> proxies) : base(proxies) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            //SHAKY
            return (TokenFunction<ROut>)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArg>>) })
                .Invoke(new object[] { tokens });
        }
    }
    #region OriginalArgs
    // ---- [ OriginalArgs ] ----
    public sealed record OriginalArg1<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : Token.IHasArg1<RArg> where RArg : ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule rule) => original.Arg1.ApplyRule(rule);
    }
    public sealed record OriginalArg2<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : Token.IHasArg2<RArg> where RArg : ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule rule) => original.Arg2.ApplyRule(rule);
    }
    public sealed record OriginalArg3<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : Token.IHasArg3<RArg> where RArg : ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule rule) => original.Arg3.ApplyRule(rule);
    }
    // --------
    #endregion
    #region Functions
    // ---- [ Functions ] ----
    public record Function<TNew, TOrig, RArg1, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken<ROut>
        where TNew : Token.Function<RArg1, ROut>
        where RArg1 : ResObj
        where ROut : ResObj
    {
        public Function(IProxy<TOrig, RArg1> in1) : base(in1) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>) })
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TNew, TOrig, RArg1, RArg2, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken<ROut>
        where TNew : Token.Function<RArg1, RArg2, ROut>
        where RArg1 : ResObj
        where RArg2 : ResObj
        where ROut : ResObj
    {
        public Function(IProxy<TOrig, RArg1> in1, IProxy<TOrig, RArg2> in2) : base(in1, in2) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>), typeof(IToken<RArg2>)})
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TNew, TOrig, RArg1, RArg2, RArg3, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken<ROut>
        where TNew : Token.Function<RArg1, RArg2, RArg3, ROut>
        where RArg1 : ResObj
        where RArg2 : ResObj
        where RArg3 : ResObj
        where ROut : ResObj
    {
        public Function(IProxy<TOrig, RArg1> in1, IProxy<TOrig, RArg2> in2, IProxy<TOrig, RArg2> in3) : base(in1, in2, in3) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>), typeof(IToken<RArg2>), typeof(IToken<RArg3>) })
                .Invoke(tokens.ToArray());
        }
    }
    // --------
    #endregion

}