using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using GExtensions;
using JetBrains.Annotations;
using Perfection;
using Token;
using UnityEngine.UIElements;


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
        public List<Rule.Unsafe.IRule> Rules { get; init; }
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
        public Task<Resolved<T>?> ResolveWith(Context context)
        {

        }
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
    public abstract record Function<TIn1, TOut> : Unsafe.TokenFunction<TOut>, Rule.IArg1<TIn1>
    {
        public Token<TIn1> ArgToken1 { get; private init; }
        protected Function(Token<TIn1> in1) : base(in1)
        {
            ArgToken1 = in1;
        }
        protected abstract TOut Evaluate(TIn1 in1);
        protected override TOut TransformTokens(List<object> args) =>
            Evaluate((TIn1)args[0]);
    }
    public abstract record Function<TIn1, TIn2, TOut> : Unsafe.TokenFunction<TOut>, Rule.IArg2<TIn1, TIn2>
    {
        public Token<TIn1> ArgToken1 { get; private init; }
        public Token<TIn2> ArgToken2 { get; private init; }
        protected Function(Token<TIn1> in1, Token<TIn2> in2) : base(in1, in2)
        {
            ArgToken1 = in1;
            ArgToken2 = in2;
        }
        protected abstract TOut Evaluate(TIn1 in1, TIn2 in2);
        protected override TOut TransformTokens(List<object> args) =>
            Evaluate((TIn1)args[0], (TIn2)args[1]);
    }
    public abstract record Function<TIn1, TIn2, TIn3, TOut> : Unsafe.TokenFunction<TOut>, Rule.IArg3<TIn1, TIn2, TIn3>
    {
        public Token<TIn1> ArgToken1 { get; private init; }
        public Token<TIn2> ArgToken2 { get; private init; }
        public Token<TIn3> ArgToken3 { get; private init; }
        protected Function(Token<TIn1> in1, Token<TIn2> in2, Token<TIn3> in3) : base(in1, in2, in3)
        {
            ArgToken1 = in1;
            ArgToken2 = in2;
            ArgToken3 = in3;
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
    namespace Unsafe
    {
        public interface IRule
        {
            public Token.Unsafe.IToken ApplyUnsafe(Token.Unsafe.IToken original);
        }
        
    }
    public interface IArg1<T1>
    {
        public Token<T1> ArgToken1 { get; }
    }
    public interface IArg2<T1, T2> : IArg1<T1>
    {
        public Token<T2> ArgToken2 { get; }
    }
    public interface IArg3<T1, T2, T3> : IArg2<T1, T2>
    {
        public Token<T3> ArgToken3 { get; }
    }
    public record Rule<TToken, T> : Unsafe.IRule where TToken : Token.Unsafe.IToken
    {
        public Func<TToken, RuleToken<T>> RuleFunction { get; init; }
        public Token.Unsafe.IToken ApplyUnsafe(Token.Unsafe.IToken original)
        {
            return original switch
            {
                TToken token => RuleFunction(token)
                _ => original
            };
        }
    }
    public abstract record RuleToken<T>
    {
        public Token<T> Generate()
        {

        }
    }
    public record FromFunction<TToken, TIn1, TIn2, TOut> : RuleToken<TOut> where TToken : Token.Function<TIn1, TIn2, TOut>
    {

    }
}
