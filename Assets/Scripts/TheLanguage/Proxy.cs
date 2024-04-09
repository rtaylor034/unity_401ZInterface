using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perfection;
using System;

#nullable enable
namespace Proxy
{
    using ResObj = Resolution.Resolution;
    using Token;
    namespace Unsafe
    {
        using Token.Unsafe;
        using System.Linq;
        public interface IProxy
        {
            public IToken UnsafeRealize(IToken original);
        }
        public interface IProxy<R> : IProxy where R : ResObj
        {
            public abstract IToken<R> UnsafeTypedRealize(IToken<R> original);
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
            public override IToken<R> Realize(TOrig original) => ConstructFromArgs(MakeSubstitutions(original));
            protected List<IToken> MakeSubstitutions(TOrig original)
            {
                return new(ArgProxies.Map(x => x.UnsafeRealize(original)));
            }
        }
        public interface IArgProxy<TOrig, R> : Unsafe.IProxy<R> where TOrig : IToken where R : ResObj
        {
            public IToken<R> Realize(TOrig original);
        }
    }
    public interface IProxy<TOrig, R> : Unsafe.IArgProxy<TOrig, R> where TOrig : IToken<R> where R : ResObj { }
    public abstract record Proxy<TOrig, R> : IProxy<TOrig, R> where TOrig : IToken<R> where R : ResObj
    {
        public abstract IToken<R> Realize(TOrig original);
        public IToken<R> UnsafeTypedRealize(IToken<R> original) => Realize((TOrig)original);
        public Token.Unsafe.IToken UnsafeRealize(Token.Unsafe.IToken original) => UnsafeTypedRealize((IToken<R>)original);
    }
    public interface ITokenFunctionOf<RIn1, out ROut> : IToken<ROut>
        where RIn1 : ResObj
        where ROut : ResObj
    {
        public IToken<RIn1> Arg1 { get; }
    }
    public interface ITokenFunctionOf<RIn1, RIn2, out ROut> : ITokenFunctionOf<RIn1, ROut>
        where RIn1 : ResObj
        where RIn2 : ResObj
        where ROut : ResObj
    {
        public IToken<RIn2> Arg2 { get; }
    }
    public interface ITokenFunctionOf<RIn1, RIn2, RIn3, out ROut> : ITokenFunctionOf<RIn1, RIn2, ROut>
        where RIn1 : ResObj
        where RIn2 : ResObj
        where RIn3 : ResObj
        where ROut : ResObj
    {
        public IToken<RIn3> Arg3 { get; }
    }
    public interface ITokenCombinerOf<RIn, out ROut> : IToken<ROut> where RIn : ResObj where ROut : ResObj
    {
        public IEnumerable<IToken<RIn>> Args { get; }
    }
}
namespace Proxies
{
    using Proxy;
    using Token;
    using Resolution;
    using Token.Unsafe;
    using Proxy.Unsafe;

    public record Combiner<TNew, TOrig, RIn, ROut> : Proxy.Unsafe.FunctionProxy<TOrig, ROut>
        where TNew : Token.Combiner<RIn, ROut>
        where TOrig : IToken<ROut>
        where RIn : Resolution
        where ROut : Resolution
    {
        public Combiner(IEnumerable<IArgProxy<TOrig, RIn>> proxies) : base(proxies) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            //SHAKY
            return (TokenFunction<ROut>)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RIn>>) })
                .Invoke(new object[] { tokens });
        }
    }
    public sealed record Direct<TOrig, R> : Proxy<TOrig, R> where TOrig : Token.IToken<R> where R : Resolution
    {
        private IToken<R> _token { get; init; }
        public Direct(IToken<R> token) => _token = token;
        public override IToken<R> Realize(TOrig _) => _token;
    }

    #region Functions
    // ---- [ Functions ] ----
    public record Function<TNew, TOrig, RIn1, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken<ROut>
        where TNew : Token.Function<RIn1, ROut>
        where RIn1 : Resolution
        where ROut : Resolution
    {
        public Function(IArgProxy<TOrig, RIn1> in1) : base(in1) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RIn1>) })
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TNew, TOrig, RIn1, RIn2, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken<ROut>
        where TNew : Token.Function<RIn1, RIn2, ROut>
        where RIn1 : Resolution
        where RIn2 : Resolution
        where ROut : Resolution
    {
        public Function(IArgProxy<TOrig, RIn1> in1, IArgProxy<TOrig, RIn2> in2) : base(in1, in2) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RIn1>), typeof(IToken<RIn2>)})
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TNew, TOrig, RIn1, RIn2, RIn3, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken<ROut>
        where TNew : Token.Function<RIn1, RIn2, RIn3, ROut>
        where RIn1 : Resolution
        where RIn2 : Resolution
        where RIn3 : Resolution
        where ROut : Resolution
    {
        public Function(IArgProxy<TOrig, RIn1> in1, IArgProxy<TOrig, RIn2> in2, IArgProxy<TOrig, RIn2> in3) : base(in1, in2, in3) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RIn1>), typeof(IToken<RIn2>), typeof(IToken<RIn3>) })
                .Invoke(tokens.ToArray());
        }
    }
    // --------
    #endregion

}