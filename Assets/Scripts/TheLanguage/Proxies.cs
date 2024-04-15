using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perfection;
using System;
using ResObj = Resolution.IResolution;
using Token;
using Proxy;
using Token.Unsafe;
using Proxy.Unsafe;
using Rule;

#nullable enable
namespace Proxies
{
    public record Direct<TOrig, R> : Proxy<TOrig, R> where TOrig : IToken<R> where R : class, ResObj
    {
        protected IToken<R> Token { get; init; }
        public override IToken<R> Realize(TOrig _, Rule.IRule __) => Token;
        public Direct(IToken<R> token) => Token = token;
        public Direct<TTo, R> Fix<TTo>() where TTo : IToken<R> => new(Token);
    }

    public sealed record CombinerTransform<TNew, TOrig, RArg, ROut> : Proxy<TOrig, ROut>
        where TOrig : Token.IHasCombineArgs<RArg>
        where TNew : Token.Combiner<RArg, ROut>
        where RArg : class, ResObj
        where ROut : class, ResObj
    {
        public override IToken<ROut> Realize(TOrig original, Rule.IRule rule)
        {
            var remapped = original.Args.Map(x => x.ApplyRule(rule));
            return (TNew)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArg>>) })
                .Invoke(new object[] { remapped });
        }
    }
    public record Combiner<TNew, TOrig, RArg, ROut> : FunctionProxy<TOrig, ROut>
        where TNew : Token.Combiner<RArg, ROut>
        where TOrig : IToken<ROut>
        where RArg : class, ResObj
        where ROut : class, ResObj
    {
        public Combiner(IEnumerable<IProxy<TOrig, RArg>> proxies) : base(proxies) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            //SHAKY
            return (TokenFunction<ROut>)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArg>>) })
                .Invoke(new object[] { tokens });
        }
    }
    public record SubEnvironment<TOrig, R> : Proxy<TOrig, R>
        where TOrig : IToken<R>
        where R : class, ResObj
    {
        protected List<IProxy<TOrig, Resolution.Operation>> EnvModifiers { get; init; }
        public IProxy<R> SubTokenProxy { get; init; }
        public SubEnvironment(params IProxy<TOrig, Resolution.Operation>[] proxies)
        {
            EnvModifiers = new(proxies);
        }
        public SubEnvironment(IEnumerable<IProxy<TOrig, Resolution.Operation>> proxies)
        {
            EnvModifiers = new(proxies);
        }
        public SubEnvironment(SubEnvironment<TOrig, R> original) : base(original)
        {
            EnvModifiers = new(original.EnvModifiers);
            SubTokenProxy = original.SubTokenProxy;
        }
        public override IToken<R> Realize(TOrig original, Rule.IRule rule) =>
            new Tokens.SubEnvironment<R>(EnvModifiers.Map(x => x.UnsafeRealize(original, rule))) { SubToken = SubTokenProxy.UnsafeTypedRealize(original, rule) };
    }
    public record Variable<TOrig, R> : Proxy<TOrig, Resolutions.DeclareVariable>
        where TOrig : IToken
        where R : class, ResObj
    {
        protected IProxy<TOrig, R> ObjectProxy { get; init; }
        protected string Label { get; init; }
        public Variable(string label, IProxy<TOrig, R> proxy)
        {
            Label = label;
            ObjectProxy = proxy;
        }
        public override IToken<Resolutions.DeclareVariable> Realize(TOrig original, IRule realizingRule)
        {
            return new Tokens.Variable<R>(Label, ObjectProxy.Realize(original, realizingRule));
        }
    }
    #region OriginalArgs
    // ---- [ OriginalArgs ] ----
    public sealed record OriginalArg1<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : Token.IHasArg1<RArg> where RArg : class, ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule rule)
        {
            return original.Arg1.ApplyRule(rule);
        }
    }
    public sealed record OriginalArg2<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : Token.IHasArg2<RArg> where RArg : class, ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule rule) => original.Arg2.ApplyRule(rule);
    }
    public sealed record OriginalArg3<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : Token.IHasArg3<RArg> where RArg : class, ResObj
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
        where RArg1 : class, ResObj
        where ROut : class, ResObj
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
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where ROut : class, ResObj
    {
        public Function(IProxy<TOrig, RArg1> in1, IProxy<TOrig, RArg2> in2) : base(in1, in2) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>), typeof(IToken<RArg2>) })
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TNew, TOrig, RArg1, RArg2, RArg3, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken<ROut>
        where TNew : Token.Function<RArg1, RArg2, RArg3, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where RArg3 : class, ResObj
        where ROut : class, ResObj
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