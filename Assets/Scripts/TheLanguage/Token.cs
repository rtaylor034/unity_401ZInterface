using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GExtensions;
using Perfection;


#nullable enable
namespace Token
{
    #region Structures
#nullable disable
    public interface IInputProvider { }
    public interface IStateResolution { }
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
    }
    public record Resolved<T>
    {
        public T Value { get; init; }
        public Context NewContext { get; init; }
    }
#nullable enable
    #endregion

    public abstract record Token<T> : Unsafe.IToken
    {
        public abstract Task<Resolved<T>?> Resolve(Context context);
        public async Task<Resolved<object>?> ResolveUnsafe(Context context)
        {
            return await Resolve(context) is Resolved<T> resolution
            ? new() {
                Value = resolution.Value,
                NewContext = resolution.NewContext
            }
            : null;
        }
        
    }
    namespace Unsafe
    {
        public interface IToken
        {
            public Task<Resolved<object>?> ResolveUnsafe(Context context);
        }
        public abstract record TokenFunction<T> : Token<T>
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
            public TokenFunction(TokenFunction<T> original) : base(original)
            {
                ArgTokens = new(original.ArgTokens);
            }
            protected abstract T TransformTokens(List<object> tokens);
            public override async Task<Resolved<T>?> Resolve(Context context)
            {
                return await GetTokenResults(context) is Resolved<List<object>> success
                ? new() {
                    Value = TransformTokens(success.Value),
                    NewContext = success.NewContext,
                }
                : null;
            }
            private async Task<Resolved<List<object>>?> GetTokenResults(Context context)
            {
                List<object> o = new(ArgTokens.Count);
                List<Context> contexts = new(ArgTokens.Count + 1) { context };
                for (int i = 0; i < ArgTokens.Count; i++)
                {
                    switch (await ArgTokens[i].ResolveUnsafe(contexts[i]))
                    {
                        case Resolved<object> resolution:
                            o[i] = resolution.Value;
                            o[i+1] = resolution.NewContext;
                            continue;
                        case null:
                            if (i == 0) return null;
                            i -= 2;
                            continue;
                    }
                }
                return new()
                { Value = o, NewContext = contexts[^1] };
            }
        }
    }
    public abstract record Infallible<T> : Token<T>
    {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
        public override Task<Resolved<T>?> Resolve(Context context) => Task.FromResult(InfallibleResolve(context));
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        protected abstract Resolved<T> InfallibleResolve(Context context);
    }
    public abstract record NonMutating<T> : Token<T>
    {
        protected abstract Task<T?> NonMutatingResolve(Context context);
        public override async Task<Resolved<T>?> Resolve(Context context)
        {
            return await NonMutatingResolve(context) is T value
            ? new() {
                Value = value,
                NewContext = context,
            }
            : null;
        }
    }
    public abstract record Pure<T> : Token<T>
    {
        protected abstract T PureResolve(Context context);
        public override Task<Resolved<T>?> Resolve(Context context)
        {
            return Task.FromResult<Resolved<T>?>(new()
            {
                Value = PureResolve(context),
                NewContext = context,
            });
        }
        
    }
    #region Function<T>s
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(Token&lt;<typeparamref name="TIn1"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="TIn1"></typeparam>
    public abstract record Function<TIn1, TOut> : Unsafe.TokenFunction<TOut>
    {

        protected Function(Token<TIn1> in1) : base(in1)
        {

        }
        protected abstract TOut Evaluate(TIn1 in1);
        protected override TOut TransformTokens(List<object> args) =>
            Evaluate((TIn1)args[0]);
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(Token&lt;<typeparamref name="TIn1"/>&gt;, Token&lt;<typeparamref name="TIn2"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="TIn1"></typeparam>
    /// <typeparam name="TIn2"></typeparam>
    public abstract record Function<TIn1, TIn2, TOut> : Unsafe.TokenFunction<TOut>
    {
        protected Function(Token<TIn1> in1, Token<TIn2> in2) : base(in1, in2)
        {

        }
        protected abstract TOut Evaluate(TIn1 in1, TIn2 in2);
        protected override TOut TransformTokens(List<object> args) =>
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
    {
        protected Function(Token<TIn1> in1, Token<TIn2> in2, Token<TIn3> in3) : base(in1, in2, in3)
        {

        }
        protected abstract TOut Evaluate(TIn1 in1, TIn2 in2, TIn3 in3);
        protected override TOut TransformTokens(List<object> args) =>
            Evaluate((TIn1)args[0], (TIn2)args[1], (TIn3)args[2]);
    }
    #endregion
    public abstract record Combiner<TIn, TOut> : Unsafe.TokenFunction<TOut>
    {
        protected Combiner(IEnumerable<Token<TIn>> tokens) : base(tokens) { }
        protected Combiner(params Token<TIn>[] tokens) : base(tokens) { }
        protected abstract TOut Evaluate(IEnumerable<TIn> inputs);
        protected override TOut TransformTokens(List<object> tokens) => Evaluate(tokens.Map(x => (TIn)x));
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
        public abstract record FunctionProxy<T> : Proxy<T>
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
            public FunctionProxy(FunctionProxy<T> original) : base(original)
            {
                TokenType = original.TokenType;
                ArgProxies = new(original.ArgProxies);
            }
            protected virtual TokenFunction<T> RealizeArgs(List<IToken> tokens)
            {
                // SHAKY
                //this is... silly...
                return (TokenFunction<T>)TokenType.GetConstructor(tokens.Map(x => x.GetType()).ToArray())
                    .Invoke(tokens.ToArray());
            }
            public override Token<T> Realize(Token<T> original) => RealizeArgs(MakeSubstitutions(original));
            protected List<IToken> MakeSubstitutions(Token<T> original)
            {
                return new(ArgProxies.Map(x => x.UnsafeRealize(original)));
            }
        }
    }
    public abstract record Proxy<T> : Unsafe.IProxy
    {
        public abstract Token<T> Realize(Token<T> original);
        public Token.Unsafe.IToken UnsafeRealize(Token.Unsafe.IToken original) => Realize((Token<T>)original);
    }
    #region Function<T>s
    public abstract record Function<TToken, TIn1, TOut> : Unsafe.FunctionProxy<TOut>
        where TToken : Token.Function<TIn1, TOut>
    {
        protected Function(Proxy<TIn1> in1) : base(typeof(TToken), in1) { }
    }
    public abstract record Function<TToken, TIn1, TIn2, TOut> : Unsafe.FunctionProxy<TOut>
        where TToken : Token.Function<TIn1, TIn2, TOut>
    {
        protected Function(Proxy<TIn1> in1, Proxy<TIn2> in2) : base(typeof(TToken), in1, in2) { }
    }
    public abstract record Function<TToken, TIn1, TIn2, TIn3, TOut> : Unsafe.FunctionProxy<TOut>
        where TToken : Token.Function<TIn1, TIn2, TIn3, TOut>
    {
        protected Function(Proxy<TIn1> in1, Proxy<TIn2> in2, Proxy<TIn2> in3) : base(typeof(TToken), in1, in2, in3) { }
    }
    #endregion
    public abstract record Combiner<TToken, TIn, TOut> : Unsafe.FunctionProxy<TOut>
        where TToken : Token.Combiner<TIn, TOut>
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
