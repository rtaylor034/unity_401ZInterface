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
    public sealed record Direct<TOrig, R> : Proxy<TOrig, R> where TOrig : IToken where R : class, ResObj
    {
        public Direct(IToken<R> token)
        {
            _token = token;
        }
        public Direct<TTo, R> Fix<TTo>() where TTo : IToken<R> => new(_token);
        public override IToken<R> Realize(TOrig _, Rule.IRule? __) => _token;

        private readonly IToken<R> _token;
    }

    public sealed record CombinerTransform<TNew, TOrig, RArg, ROut> : Proxy<TOrig, ROut>
        where TOrig : IHasCombinerArgs<RArg>
        where TNew : Token.ICombiner<RArg, ROut>
        where RArg : class, ResObj
        where ROut : class, ResObj
    {
        public override IToken<ROut> Realize(TOrig original, Rule.IRule? rule)
        {
            return (TNew)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArg>>) })
                .Invoke(new object[] { original.Args.Map(x => x.ApplyRule(rule)) }) ;
        }
    }

    public record Combiner<TNew, TOrig, RArg, ROut> : FunctionProxy<TOrig, ROut>
        where TNew : IHasCombinerArgs<RArg>, IToken<ROut>
        where TOrig : IToken
        where RArg : class, ResObj
        where ROut : class, ResObj
    {
        public Combiner(IEnumerable<IProxy<TOrig, RArg>> proxies) : base(proxies) { }

        protected override IToken<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (IToken<ROut>)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArg>>) })
                .Invoke(new object[] { tokens });
        }
    }

    public sealed record SubEnvironment<TNew, TOrig, REnv, ROut> : Proxy<TOrig, ROut>
        where TNew : Token.SubEnvironment<REnv, ROut>
        where TOrig : IToken
        where REnv : Resolution.Operation
        where ROut : class, ResObj
    {
        public IProxy<ROut> SubTokenProxy { get; init; }
        public SubEnvironment(IEnumerable<IProxy<TOrig, REnv>> proxies)
        {
            _envModifiers = new() { Elements = proxies };
        }
        public SubEnvironment(params IProxy<TOrig, REnv>[] proxies) : this((IEnumerable<IProxy<TOrig, REnv>>)proxies) { }
        public override IToken<ROut> Realize(TOrig original, Rule.IRule? rule)
        {
            return (SubEnvironment<REnv, ROut>)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<REnv>>) })
            .Invoke(new object[] { _envModifiers.Elements.Map(x => x.Realize(original, rule)) }) with
            {
                SubToken = SubTokenProxy.UnsafeTypedRealize(original, rule)
            };
        }

        private readonly PList<IProxy<TOrig, REnv>> _envModifiers;
    }

    public sealed record Variable<TOrig, R> : Proxy<TOrig, Resolutions.DeclareVariable>
        where TOrig : IToken
        where R : class, ResObj
    {
        public Variable(string label, IProxy<TOrig, R> proxy)
        {
            _label = label;
            _objectProxy = proxy;
        }
        public override IToken<Resolutions.DeclareVariable> Realize(TOrig original, IRule? rule)
        {
            return new Tokens.Variable<R>(_label, _objectProxy.Realize(original, rule));
        }

        private readonly string _label;
        private readonly IProxy<TOrig, R> _objectProxy;
    }
    #region OriginalArgs
    // ---- [ OriginalArgs ] ----
    public sealed record OriginalArg1<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : IHasArg1<RArg> where RArg : class, ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule? rule) { return original.Arg1.ApplyRule(rule); }
    }
    public sealed record OriginalArg2<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : IHasArg2<RArg> where RArg : class, ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule? rule) { return original.Arg2.ApplyRule(rule); }
    }
    public sealed record OriginalArg3<TOrig, RArg> : Proxy<TOrig, RArg> where TOrig : IHasArg3<RArg> where RArg : class, ResObj
    {
        public override IToken<RArg> Realize(TOrig original, Rule.IRule? rule) { return original.Arg3.ApplyRule(rule); }
    }
    // --------
    #endregion
    #region Functions
    // ---- [ Functions ] ----
    public record Function<TNew, TOrig, RArg1, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken
        where TNew : IFunction<RArg1, ROut>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    {
        public Function(IProxy<TOrig, RArg1> in1) : base(in1) { }
        protected override IToken<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (IToken<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>) })
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TNew, TOrig, RArg1, RArg2, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken
        where TNew : IFunction<RArg1, RArg2, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where ROut : class, ResObj
    {
        public Function(IProxy<TOrig, RArg1> in1, IProxy<TOrig, RArg2> in2) : base(in1, in2) { }
        protected override IToken<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (IToken<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>), typeof(IToken<RArg2>) })
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TNew, TOrig, RArg1, RArg2, RArg3, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken
        where TNew : IFunction<RArg1, RArg2, RArg3, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where RArg3 : class, ResObj
        where ROut : class, ResObj
    {
        public Function(IProxy<TOrig, RArg1> in1, IProxy<TOrig, RArg2> in2, IProxy<TOrig, RArg2> in3) : base(in1, in2, in3) { }
        protected override IToken<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (IToken<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>), typeof(IToken<RArg2>), typeof(IToken<RArg3>) })
                .Invoke(tokens.ToArray());
        }
    }
    // --------
    #endregion

}