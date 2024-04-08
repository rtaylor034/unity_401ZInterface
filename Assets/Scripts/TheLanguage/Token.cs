using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;


#nullable enable
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
        public List<Proxy.Unsafe.IProxy> Rules { get; init; }
        public Context WithResolution(Resolution resolution) => resolution._ChangeContext(this);
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
#pragma warning disable CS8619
        public override Task<R?> Resolve(Context context) => Task.FromResult(InfallibleResolve(context));
#pragma warning restore CS8619
        protected abstract R InfallibleResolve(Context context);
    }
    #region Functions
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(Token&lt;<typeparamref name="RIn1"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RIn1"></typeparam>
    public abstract record Function<RIn1, ROut> : Unsafe.TokenFunction<ROut>
        where RIn1 : Resolution
        where ROut : Resolution
    {
        protected Function(Token<RIn1> in1) : base(in1) { }
        protected abstract ROut Evaluate(RIn1 in1);
        protected override ROut TransformTokens(List<Resolution> args) =>
            Evaluate((RIn1)args[0]);
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(Token&lt;<typeparamref name="RIn1"/>&gt;, Token&lt;<typeparamref name="RIn2"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RIn1"></typeparam>
    /// <typeparam name="RIn2"></typeparam>
    public abstract record Function<RIn1, RIn2, ROut> : Unsafe.TokenFunction<ROut>
        where RIn1 : Resolution
        where RIn2 : Resolution
        where ROut : Resolution
    {
        protected Function(Token<RIn1> in1, Token<RIn2> in2) : base(in1, in2) { }
        protected abstract ROut Evaluate(RIn1 in1, RIn2 in2);
        protected override ROut TransformTokens(List<Resolution> args) =>
            Evaluate((RIn1)args[0], (RIn2)args[1]);
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(Token&lt;<typeparamref name="RIn1"/>&gt;, Token&lt;<typeparamref name="RIn2"/>&gt;, Token&lt;<typeparamref name="RIn3"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RIn1"></typeparam>
    /// <typeparam name="RIn2"></typeparam>
    /// <typeparam name="RIn3"></typeparam>
    public abstract record Function<RIn1, RIn2, RIn3, ROut> : Unsafe.TokenFunction<ROut>
        where RIn1 : Resolution
        where RIn2 : Resolution
        where RIn3 : Resolution
        where ROut : Resolution
    {
        protected Function(Token<RIn1> in1, Token<RIn2> in2, Token<RIn3> in3) : base(in1, in2, in3) { }
        protected abstract ROut Evaluate(RIn1 in1, RIn2 in2, RIn3 in3);
        protected override ROut TransformTokens(List<Resolution> args) =>
            Evaluate((RIn1)args[0], (RIn2)args[1], (RIn3)args[2]);
    }
    #endregion
    public abstract record Combiner<RIn, ROut> : Unsafe.TokenFunction<ROut>
        where RIn : Resolution
        where ROut : Resolution
    {
        protected Combiner(IEnumerable<Token<RIn>> tokens) : base(tokens) { }
        protected Combiner(params Token<RIn>[] tokens) : base(tokens) { }
        protected abstract ROut Evaluate(IEnumerable<RIn> inputs);
        protected override ROut TransformTokens(List<Resolution> tokens) => Evaluate(tokens.Map(x => (RIn)x));
    }

}
namespace Tokens
{

}