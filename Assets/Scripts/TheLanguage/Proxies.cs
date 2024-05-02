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
        where TOrig : IHasCombinerArgs<RArg>, IToken<ROut>
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

    public record Combiner<TNew, TOrig, RArgs, ROut> : FunctionProxy<TOrig, ROut>
        where TNew : Token.ICombiner<RArgs, ROut>
        where TOrig : IToken
        where RArgs : class, ResObj
        where ROut : class, ResObj
    {
        public Combiner(IEnumerable<IProxy<TOrig, RArgs>> proxies) : base(proxies) { }

        protected override IToken<ROut> ConstructFromArgs(TOrig _, List<IToken> tokens)
        {
            return (IToken<ROut>)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArgs>>) })
                .Invoke(new object[] { tokens.Map(x => (IToken<RArgs>)x) });
        }
    }

    public sealed record SubEnvironment<TOrig, ROut> : Proxy<TOrig, ROut>
        where TOrig : IToken
        where ROut : class, ResObj
    {
        public IProxy<TOrig, ROut> SubTokenProxy { get; init; }
        public SubEnvironment(IEnumerable<IProxy<TOrig, ResObj>> proxies)
        {
            _envModifiers = new() { Elements = proxies };
        }
        public SubEnvironment(params IProxy<TOrig, ResObj>[] proxies) : this((IEnumerable<IProxy<TOrig, ResObj>>)proxies) { }
        public override IToken<ROut> Realize(TOrig original, Rule.IRule? rule)
        {
            return new Tokens.SubEnvironment<ROut>(_envModifiers.Elements.Map(x => x.UnsafeRealize(original, rule)))
            {
                SubToken = SubTokenProxy.Realize(original, rule)
            };
        }

        private readonly PList<IProxy<TOrig, ResObj>> _envModifiers;
    }
    public sealed record Accumulator<TNew, TOrig, RElement, RGen, RInto> : FunctionProxy<TOrig, RInto>
        where TNew : Token.Accumulator<RElement, RGen, RInto>
        where TOrig : IToken
        where RElement : class, ResObj
        where RGen : class, ResObj
        where RInto : class, ResObj
    {
        public Accumulator(IProxy<TOrig, Resolution.IMulti<RElement>> iterator, VariableIdentifier<RElement> elementVariable, IProxy<TOrig, RGen> generator) : base(iterator, generator)
        {
            _elementIdentifier = elementVariable;
        }
        protected override IToken<RInto> ConstructFromArgs(TOrig _, List<IToken> tokens)
        {
            return (Token.Accumulator<RElement, RGen, RInto>)typeof(TNew)
                .GetConstructor(new Type[] { typeof(IToken<Resolution.IMulti<RElement>>), typeof(string), typeof(IToken<RGen>) })
                .Invoke(new object[] { (IToken<Resolution.IMulti<RElement>>)tokens[0], _elementIdentifier, (IToken<RGen>)tokens[1] });
        }
        private readonly VariableIdentifier<RElement> _elementIdentifier;
    }
    public sealed record Variable<TOrig, R> : Proxy<TOrig, Resolutions.DeclareVariable<R>>
        where TOrig : IToken
        where R : class, ResObj
    {
        public Variable(VariableIdentifier<R> identifier, IProxy<TOrig, R> proxy)
        {
            _identifier = identifier;
            _objectProxy = proxy;
        }
        public override IToken<Resolutions.DeclareVariable<R>> Realize(TOrig original, IRule? rule)
        {
            return new Tokens.Variable<R>(_identifier, _objectProxy.Realize(original, rule));
        }

        private readonly VariableIdentifier<R> _identifier;
        private readonly IProxy<TOrig, R> _objectProxy;
    }
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

    // ---- [ Functions ] ----
    public record Function<TNew, TOrig, RArg1, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken
        where TNew : IFunction<RArg1, ROut>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    {
        public Function(IProxy<TOrig, RArg1> in1) : base(in1) { }
        protected override IToken<ROut> ConstructFromArgs(TOrig _, List<IToken> tokens)
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
        protected override IToken<ROut> ConstructFromArgs(TOrig _, List<IToken> tokens)
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
        protected override IToken<ROut> ConstructFromArgs(TOrig _, List<IToken> tokens)
        {
            return (IToken<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>), typeof(IToken<RArg2>), typeof(IToken<RArg3>) })
                .Invoke(tokens.ToArray());
        }
    }
    // --------

    // ---- [ Recursive Call] ----
    public record RecursiveCall<RArg1, ROut> : FunctionProxy<Tokens.Recursive<RArg1, ROut>, ROut>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    {
        protected override IToken<ROut> ConstructFromArgs(Tokens.Recursive<RArg1, ROut> original, List<IToken> tokens)
        {
            return new Tokens.Recursive<RArg1, ROut>(
                (IToken<RArg1>)tokens[0],
                original.RecursiveProxy);
        }
    }
    public record RecursiveCall<RArg1, RArg2, ROut> : FunctionProxy<Tokens.Recursive<RArg1, RArg2, ROut>, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where ROut : class, ResObj
    {
        protected override IToken<ROut> ConstructFromArgs(Tokens.Recursive<RArg1, RArg2, ROut> original, List<IToken> tokens)
        {
            return new Tokens.Recursive<RArg1, RArg2, ROut>(
                (IToken<RArg1>)tokens[0],
                (IToken<RArg2>)tokens[1],
                original.RecursiveProxy);
        }
    }
    public record RecursiveCall<RArg1, RArg2, RArg3, ROut> : FunctionProxy<Tokens.Recursive<RArg1, RArg2, RArg3, ROut>, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where RArg3 : class, ResObj
        where ROut : class, ResObj
    {
        protected override IToken<ROut> ConstructFromArgs(Tokens.Recursive<RArg1, RArg2, RArg3, ROut> original, List<IToken> tokens)
        {
            return new Tokens.Recursive<RArg1, RArg2, RArg3, ROut>(
                (IToken<RArg1>)tokens[0],
                (IToken<RArg2>)tokens[1],
                (IToken<RArg3>)tokens[2],
                original.RecursiveProxy);
        }
    }

    // --------
}