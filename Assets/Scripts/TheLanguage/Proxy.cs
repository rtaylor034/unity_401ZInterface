using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perfection;
using System;

#nullable enable
namespace Proxy
{
    using Token;
    namespace Unsafe
    {
        using Token.Unsafe;
        using System.Linq;
        public interface IProxy
        {
            public IToken UnsafeRealize(IToken original);
        }
        public abstract record FunctionProxy<R> : Proxy<R> where R : Resolution.Resolution
        {
            /// <summary>
            /// Expected to be [ : ]<see cref="Token.Unsafe.TokenFunction{T}"/>
            /// </summary>
            protected Type TokenType { get; init; }
            protected List<IProxy> ArgProxies { get; init; }
            protected FunctionProxy(Type tokenType, params IProxy[] proxies)
            {
                TokenType = tokenType;
                ArgProxies = new(proxies);
            }
            protected FunctionProxy(Type tokenType, IEnumerable<IProxy> proxies)
            {
                TokenType = tokenType;
                ArgProxies = new(proxies);
            }
            public FunctionProxy(FunctionProxy<R> original) : base(original)
            {
                TokenType = original.TokenType;
                ArgProxies = new(original.ArgProxies);
            }
            protected abstract TokenFunction<R> ConstructFromArgs(List<IToken> tokens);
            public override IToken<R> Realize(IToken<R> original) => ConstructFromArgs(MakeSubstitutions(original));
            protected List<IToken> MakeSubstitutions(IToken<R> original)
            {
                return new(ArgProxies.Map(x => x.UnsafeRealize(original)));
            }
        }
    }
    public abstract record Proxy<R> : Unsafe.IProxy where R : Resolution.Resolution
    {
        public abstract IToken<R> Realize(IToken<R> original);
        public Token.Unsafe.IToken UnsafeRealize(Token.Unsafe.IToken original) => Realize((IToken<R>)original);
    }

    /// <summary>
    /// provides syntax sugar when making proxies.
    /// </summary>
    public static class Builder
    {
        public static Proxies.Function<TToken, RIn1, R> WithArgs<R, TToken, RIn1>(this OfType<TToken, R> _, Proxy<RIn1> in1)
            where TToken : Token.Function<RIn1, R>
            where RIn1 : Resolution.Resolution
            where R : Resolution.Resolution
            { return new(in1); }
        public static Proxies.Function<TToken, RIn1, RIn2, R> WithArgs<R, TToken, RIn1, RIn2>(this OfType<TToken, R> _, Proxy<RIn1> in1, Proxy<RIn2> in2)
            where TToken : Token.Function<RIn1, RIn2, R>
            where RIn1 : Resolution.Resolution
            where RIn2 : Resolution.Resolution
            where R : Resolution.Resolution
            { return  new(in1, in2); }
        public static Proxies.Function<TToken, RIn1, RIn2, RIn3, R> WithArgs<R, TToken, RIn1, RIn2, RIn3>(this OfType<TToken, R> _, Proxy<RIn1> in1, Proxy<RIn2> in2, Proxy<RIn2> in3)
            where TToken : Token.Function<RIn1, RIn2, RIn3, R>
            where RIn1 : Resolution.Resolution
            where RIn2 : Resolution.Resolution
            where RIn3 : Resolution.Resolution
            where R : Resolution.Resolution
            { return new(in1, in2, in3); }

        public static Proxies.Combiner<TToken, RIn1, R> WithArgs<R, TToken, RIn1>(this OfType<TToken, R> _, IEnumerable<Proxy<RIn1>> ins)
            where TToken : Token.Combiner<RIn1, R>
            where RIn1 : Resolution.Resolution
            where R : Resolution.Resolution
            { return new(ins); }
        public static Proxies.Direct<TToken, R> AsProxy<TToken, R>(this TToken token)
            where TToken : IToken<R>
            where R : Resolution.Resolution
            { return new(token); }
    }
    /// <summary>
    /// exists solely for <see cref="Build"/>.
    /// </summary>
    /// <typeparam name="TToken"></typeparam>
    public struct OfType<TToken, R> where TToken : IToken<R> where R : Resolution.Resolution { }
}
namespace Proxies
{
    using Proxy;
    using Token;
    using Resolution;
    using Token.Unsafe;

    public record Combiner<TToken, RIn, ROut> : Proxy.Unsafe.FunctionProxy<ROut>
        where TToken : Token.Combiner<RIn, ROut>
        where RIn : Resolution
        where ROut : Resolution
    {
        public Combiner(IEnumerable<Proxy<RIn>> proxies) : base(typeof(TToken), proxies) { }
        protected override Token.Unsafe.TokenFunction<ROut> ConstructFromArgs(List<Token.Unsafe.IToken> tokens)
        {
            //SHAKY
            return (Token.Unsafe.TokenFunction<ROut>)TokenType.GetConstructor(new Type[] { typeof(IEnumerable<IToken<RIn>>) })
                .Invoke(new object[] { tokens });
        }
    }
    public sealed record Direct<TToken, R> : Proxy.Proxy<R> where TToken : Token.IToken<R> where R : Resolution
    {
        private TToken _token { get; init; } 
        public Direct(TToken token) => _token = token;
        public override IToken<R> Realize(IToken<R> _) => _token;
    }

    #region Functions
    // ---- [ Functions ] ----
    public record Function<TToken, RIn1, ROut> : Proxy.Unsafe.FunctionProxy<ROut>
        where TToken : Token.Function<RIn1, ROut>
        where RIn1 : Resolution
        where ROut : Resolution
    {
        public Function(Proxy<RIn1> in1) : base(typeof(TToken), in1) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                TokenType.GetConstructor(new Type[] { typeof(IToken<RIn1>) })
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TToken, RIn1, RIn2, ROut> : Proxy.Unsafe.FunctionProxy<ROut>
        where TToken : Token.Function<RIn1, RIn2, ROut>
        where RIn1 : Resolution
        where RIn2 : Resolution
        where ROut : Resolution
    {
        public Function(Proxy<RIn1> in1, Proxy<RIn2> in2) : base(typeof(TToken), in1, in2) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                TokenType.GetConstructor(new Type[] { typeof(IToken<RIn1>), typeof(IToken<RIn2>)})
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TToken, RIn1, RIn2, RIn3, ROut> : Proxy.Unsafe.FunctionProxy<ROut>
        where TToken : Token.Function<RIn1, RIn2, RIn3, ROut>
        where RIn1 : Resolution
        where RIn2 : Resolution
        where RIn3 : Resolution
        where ROut : Resolution
    {
        public Function(Proxy<RIn1> in1, Proxy<RIn2> in2, Proxy<RIn2> in3) : base(typeof(TToken), in1, in2, in3) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                TokenType.GetConstructor(new Type[] { typeof(IToken<RIn1>), typeof(IToken<RIn2>), typeof(IToken<RIn3>) })
                .Invoke(tokens.ToArray());
        }
    }
    // --------
    #endregion

}