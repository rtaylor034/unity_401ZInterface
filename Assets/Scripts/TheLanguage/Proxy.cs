using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perfection;
using System;

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
            protected virtual TokenFunction<R> RealizeArgs(List<IToken> tokens)
            {
                // SHAKY
                return (TokenFunction<R>)TokenType.GetConstructor(tokens.Map(x => x.GetType()).ToArray())
                    .Invoke(tokens.ToArray());
            }
            public override Token<R> Realize(Token<R> original) => RealizeArgs(MakeSubstitutions(original));
            protected List<IToken> MakeSubstitutions(Token<R> original)
            {
                return new(ArgProxies.Map(x => x.UnsafeRealize(original)));
            }
        }
    }
    public abstract record Proxy<R> : Unsafe.IProxy where R : Resolution.Resolution
    {
        public abstract Token<R> Realize(Token<R> original);
        public Token.Unsafe.IToken UnsafeRealize(Token.Unsafe.IToken original) => Realize((Token<R>)original);
    }
    public static class Builder
    {
        public static Proxies.Function<TToken, TIn1, R> WithArgs<R, TToken, TIn1>(this OfToken<TToken, R> _, Proxy<TIn1> in1)
            where TToken : Token.Function<TIn1, R>
            where TIn1 : Resolution.Resolution
            where R : Resolution.Resolution
        {
            return new(in1);
        }
        public static Proxies.Function<TToken, TIn1, TIn2, R> WithArgs<R, TToken, TIn1, TIn2>(this OfToken<TToken, R> _, Proxy<TIn1> in1, Proxy<TIn2> in2)
            where TToken : Token.Function<TIn1, TIn2, R>
            where TIn1 : Resolution.Resolution
            where TIn2 : Resolution.Resolution
            where R : Resolution.Resolution
        {
            return new(in1, in2);
        }
        public static Proxies.Function<TToken, TIn1, TIn2, TIn3, R> WithArgs<R, TToken, TIn1, TIn2, TIn3>(this OfToken<TToken, R> _, Proxy<TIn1> in1, Proxy<TIn2> in2, Proxy<TIn2> in3)
            where TToken : Token.Function<TIn1, TIn2, TIn3, R>
            where TIn1 : Resolution.Resolution
            where TIn2 : Resolution.Resolution
            where TIn3 : Resolution.Resolution
            where R : Resolution.Resolution
        {
            return new(in1, in2, in3);
        }
        public static Proxies.Combiner<TToken, TIn1, R> WithArgs<R, TToken, TIn1>(this OfToken<TToken, R> _, IEnumerable<Proxy<TIn1>> ins)
            where TToken : Token.Combiner<TIn1, R>
            where TIn1 : Resolution.Resolution
            where R : Resolution.Resolution
        {
            return new(ins);
        }
    }
    /// <summary>
    /// exists solely for <see cref="Build"/>.
    /// </summary>
    /// <typeparam name="TToken"></typeparam>
    public struct OfToken<TToken, R> where TToken : Token<R> where R : Resolution.Resolution
    {
        public static OfToken<TToken, R> Create() => new();
    }
}
namespace Proxies
{
    using Proxy;
    using Token;
    using Resolution;
    #region Functions
    public record Function<TToken, TIn1, TOut> : Proxy.Unsafe.FunctionProxy<TOut>
        where TToken : Token.Function<TIn1, TOut>
        where TIn1 : Resolution
        where TOut : Resolution
    {
        public Function(Proxy<TIn1> in1) : base(typeof(TToken), in1) { }
    }
    public record Function<TToken, TIn1, TIn2, TOut> : Proxy.Unsafe.FunctionProxy<TOut>
        where TToken : Token.Function<TIn1, TIn2, TOut>
        where TIn1 : Resolution
        where TIn2 : Resolution
        where TOut : Resolution
    {
        public Function(Proxy<TIn1> in1, Proxy<TIn2> in2) : base(typeof(TToken), in1, in2) { }
    }
    public record Function<TToken, TIn1, TIn2, TIn3, TOut> : Proxy.Unsafe.FunctionProxy<TOut>
        where TToken : Token.Function<TIn1, TIn2, TIn3, TOut>
        where TIn1 : Resolution
        where TIn2 : Resolution
        where TIn3 : Resolution
        where TOut : Resolution
    {
        public Function(Proxy<TIn1> in1, Proxy<TIn2> in2, Proxy<TIn2> in3) : base(typeof(TToken), in1, in2, in3) { }
    }
    #endregion
    public record Combiner<TToken, TIn, TOut> : Proxy.Unsafe.FunctionProxy<TOut>
        where TToken : Token.Combiner<TIn, TOut>
        where TIn : Resolution
        where TOut : Resolution
    {
        public Combiner(IEnumerable<Proxy<TIn>> proxies) : base(typeof(TToken), proxies) { }
        protected override Token.Unsafe.TokenFunction<TOut> RealizeArgs(List<Token.Unsafe.IToken> tokens)
        {
            //SHAKY
            return (Token.Unsafe.TokenFunction<TOut>)TokenType.GetConstructor(new Type[] { typeof(IEnumerable<Token<TIn>>) })
                .Invoke(new object[] { tokens });
        }
    }
}