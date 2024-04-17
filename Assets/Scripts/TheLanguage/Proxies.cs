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
    public sealed record Direct<TOrig, R> : Proxy<TOrig, R> where TOrig : IToken<R> where R : class, ResObj
    {
        private readonly IToken<R> _token;
        public override IToken<R> Realize(TOrig _, Rule.IRule __) => _token;
        public Direct(IToken<R> token) => _token = token;
        public Direct<TTo, R> Fix<TTo>() where TTo : IToken<R> => new(_token);
    }

    public sealed record CombinerTransform<TNew, TOrig, RArg, ROut> : Proxy<TOrig, ROut>
        where TOrig : Token.IHasCombineArgs<RArg>
        where TNew : Token.Combiner<RArg, ROut>
        where RArg : class, ResObj
        where ROut : class, ResObj
    {
        public override IToken<ROut> Realize(TOrig original, Rule.IRule rule)
        {
            return (TNew)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArg>>) })
                .Invoke(new object[] { original.Args.Map(x => x.ApplyRule(rule)) }) ;
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
            return (TokenFunction<ROut>)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArg>>) })
                .Invoke(new object[] { tokens });
        }
    }
    public sealed record SubEnvironment<TOrig, R> : Proxy<TOrig, R>
        where TOrig : IToken<R>
        where R : class, ResObj
    {
        private readonly PList<IProxy<TOrig, Resolution.Operation>> _envModifiers;
        public IProxy<R> SubTokenProxy { get; init; }
        public SubEnvironment(params IProxy<TOrig, Resolution.Operation>[] proxies) :
            this((IEnumerable<IProxy<TOrig, Resolution.Operation>>)proxies)
        { }
        public SubEnvironment(IEnumerable<IProxy<TOrig, Resolution.Operation>> proxies)
        {
            _envModifiers = new() { Elements = proxies };
        }
        public override IToken<R> Realize(TOrig original, Rule.IRule rule) =>
            new Tokens.SubEnvironment<R>(_envModifiers.Elements.Map(x => x.UnsafeRealize(original, rule))) { SubToken = SubTokenProxy.UnsafeTypedRealize(original, rule) };
    }
    public sealed record Variable<TOrig, R> : Proxy<TOrig, Resolutions.DeclareVariable>
        where TOrig : IToken
        where R : class, ResObj
    {
        private readonly IProxy<TOrig, R> _objectProxy;
        private readonly string _label;
        public Variable(string label, IProxy<TOrig, R> proxy)
        {
            _label = label;
            _objectProxy = proxy;
        }
        public override IToken<Resolutions.DeclareVariable> Realize(TOrig original, IRule realizingRule)
        {
            return new Tokens.Variable<R>(_label, _objectProxy.Realize(original, realizingRule));
        }
    }
    #region OriginalArgs
    // ---- [ OriginalArgs ] ----
    public sealed record OriginalArg1<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : Token.IHasArg1<RArg> where RArg : class, ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule rule) { return original.Arg1.ApplyRule(rule); }
    }
    public sealed record OriginalArg2<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : Token.IHasArg2<RArg> where RArg : class, ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule rule) { return original.Arg2.ApplyRule(rule); }
    }
    public sealed record OriginalArg3<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : Token.IHasArg3<RArg> where RArg : class, ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule rule) { return original.Arg3.ApplyRule(rule); }
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