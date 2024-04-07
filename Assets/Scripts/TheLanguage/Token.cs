using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GExtensions;
using Perfection;


#nullable enable
namespace Resolution
{
    using Token;
    public abstract record Resolution
    {
        public abstract Context ApplyToContext(Context before);
    }
    public abstract record NonMutating : Resolution
    {
        public override Context ApplyToContext(Context context) => context;
    }
}
namespace Token
{
    using Resolution;
    #region Structures
#nullable disable
    public interface IInputProvider { }

    
    public record Scope
    {
        public void Pop() { }
        public void Add() { }
    }
    public record Context
    {
        public GameState State { get; init; }
        public IInputProvider InputProvider { get; init; }
        public Scope Scope { get; init; }
        public List<Rule.Unsafe.IProxy> Rules { get; init; }
        public Context WithResolution(Resolution resolution) => resolution.ApplyToContext(this);
    }
#nullable enable
    #endregion

    public abstract record Token<R> : Unsafe.IToken where R : Resolution
    {
        public abstract Task<R?> Resolve(Context context);
        public async Task<Resolution?> ResolveUnsafe(Context context)
        {
            return await Resolve(context);
        }
    }
    namespace Unsafe
    {
        public interface IToken
        {
            public Task<Resolution?> ResolveUnsafe(Context context);
        }
        public abstract record TokenFunction<R> : Token<R> where R : Resolution
        {
            protected List<IToken> ArgTokens { get; init; }
            protected TokenFunction(params IToken[] tokens)
            {
                ArgTokens = new(tokens);
            }
            protected TokenFunction(IEnumerable<IToken> tokens)
            {
                ArgTokens = new(tokens);
            }
            public TokenFunction(TokenFunction<R> original) : base(original)
            {
                ArgTokens = new(original.ArgTokens);
            }
            protected abstract R TransformTokens(List<Resolution> tokens);
            public override async Task<R?> Resolve(Context context)
            {
                var o = await GetTokenResults(context);
                return o is not null ? TransformTokens(o) : null;
            }
            private async Task<List<Resolution>?> GetTokenResults(Context context)
            {
                List<Resolution> o = new(ArgTokens.Count);
                List<Context> contexts = new(ArgTokens.Count + 1) { context };
                for (int i = 0; i < ArgTokens.Count; i++)
                {
                    switch (await ArgTokens[i].ResolveUnsafe(contexts[i]))
                    {
                        case Resolution resolution:
                            o[i] = resolution;
                            contexts[i + 1] = context.WithResolution(resolution);
                            continue;
                        case null:
                            if (i == 0) return null;
                            i -= 2;
                            continue;
                    }
                }
                return o;
            }
        }
    }
    public abstract record Infallible<R> : Token<R> where R : Resolution
    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        public override Task<R?> Resolve(Context context) => Task.FromResult(InfallibleResolve(context));
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        protected abstract R InfallibleResolve(Context context);
    }
    #region Function<R>s
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(Token&lt;<typeparamref name="TIn1"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="TIn1"></typeparam>
    public abstract record Function<TIn1, TOut> : Unsafe.TokenFunction<TOut>
        where TIn1 : Resolution
        where TOut : Resolution
    {

        protected Function(Token<TIn1> in1) : base(in1)
        {

        }
        protected abstract TOut Evaluate(TIn1 in1);
        protected override TOut TransformTokens(List<Resolution> args) =>
            Evaluate((TIn1)args[0]);
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(Token&lt;<typeparamref name="TIn1"/>&gt;, Token&lt;<typeparamref name="TIn2"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="TIn1"></typeparam>
    /// <typeparam name="TIn2"></typeparam>
    public abstract record Function<TIn1, TIn2, TOut> : Unsafe.TokenFunction<TOut>
        where TIn1 : Resolution
        where TIn2 : Resolution
        where TOut : Resolution
    {
        protected Function(Token<TIn1> in1, Token<TIn2> in2) : base(in1, in2)
        {

        }
        protected abstract TOut Evaluate(TIn1 in1, TIn2 in2);
        protected override TOut TransformTokens(List<Resolution> args) =>
            Evaluate((TIn1)args[0], (TIn2)args[1]);
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(Token&lt;<typeparamref name="TIn1"/>&gt;, Token&lt;<typeparamref name="TIn2"/>&gt;, Token&lt;<typeparamref name="TIn3"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="TIn1"></typeparam>
    /// <typeparam name="TIn2"></typeparam>
    /// <typeparam name="TIn3"></typeparam>
    public abstract record Function<TIn1, TIn2, TIn3, TOut> : Unsafe.TokenFunction<TOut>
        where TIn1 : Resolution
        where TIn2 : Resolution
        where TIn3 : Resolution
        where TOut : Resolution
    {
        protected Function(Token<TIn1> in1, Token<TIn2> in2, Token<TIn3> in3) : base(in1, in2, in3)
        {

        }
        protected abstract TOut Evaluate(TIn1 in1, TIn2 in2, TIn3 in3);
        protected override TOut TransformTokens(List<Resolution> args) =>
            Evaluate((TIn1)args[0], (TIn2)args[1], (TIn3)args[2]);
    }
    #endregion
    public abstract record Combiner<TIn, TOut> : Unsafe.TokenFunction<TOut>
        where TIn : Resolution
        where TOut : Resolution
    {
        protected Combiner(IEnumerable<Token<TIn>> tokens) : base(tokens) { }
        protected Combiner(params Token<TIn>[] tokens) : base(tokens) { }
        protected abstract TOut Evaluate(IEnumerable<TIn> inputs);
        protected override TOut TransformTokens(List<Resolution> tokens) => Evaluate(tokens.Map(x => (TIn)x));
    }

}
namespace Tokens
{

}
namespace Rule
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
    #region Function<R>s
    public abstract record Function<TToken, TIn1, TOut> : Unsafe.FunctionProxy<TOut>
        where TToken : Token.Function<TIn1, TOut>
        where TIn1 : Resolution.Resolution
        where TOut : Resolution.Resolution
    {
        protected Function(Proxy<TIn1> in1) : base(typeof(TToken), in1) { }
    }
    public abstract record Function<TToken, TIn1, TIn2, TOut> : Unsafe.FunctionProxy<TOut>
        where TToken : Token.Function<TIn1, TIn2, TOut>
        where TIn1 : Resolution.Resolution
        where TIn2 : Resolution.Resolution
        where TOut : Resolution.Resolution
    {
        protected Function(Proxy<TIn1> in1, Proxy<TIn2> in2) : base(typeof(TToken), in1, in2) { }
    }
    public abstract record Function<TToken, TIn1, TIn2, TIn3, TOut> : Unsafe.FunctionProxy<TOut>
        where TToken : Token.Function<TIn1, TIn2, TIn3, TOut>
        where TIn1 : Resolution.Resolution
        where TIn2 : Resolution.Resolution
        where TIn3 : Resolution.Resolution
        where TOut : Resolution.Resolution
    {
        protected Function(Proxy<TIn1> in1, Proxy<TIn2> in2, Proxy<TIn2> in3) : base(typeof(TToken), in1, in2, in3) { }
    }
    #endregion
    public abstract record Combiner<TToken, TIn, TOut> : Unsafe.FunctionProxy<TOut>
        where TToken : Token.Combiner<TIn, TOut>
        where TIn : Resolution.Resolution
        where TOut : Resolution.Resolution
    {
        protected Combiner(IEnumerable<Proxy<TIn>> proxies) : base(typeof(TToken), proxies) { }
        protected override Token.Unsafe.TokenFunction<TOut> RealizeArgs(List<Token.Unsafe.IToken> tokens)
        {
            //SHAKY
            return (Token.Unsafe.TokenFunction<TOut>)TokenType.GetConstructor(new Type[] { typeof(IEnumerable<Token<TIn>>) })
                .Invoke(new object[] { tokens });
        }
    }
}
