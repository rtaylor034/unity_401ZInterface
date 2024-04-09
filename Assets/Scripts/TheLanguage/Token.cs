using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;


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

    public interface IToken<out R> : Unsafe.IToken where R : Resolution
    {
        public ITask<R?> Resolve(Context context);
    }
    public abstract record Token<R> : IToken<R> where R : Resolution
    {
        public abstract ITask<R?> Resolve(Context context);
        public async ITask<Resolution?> ResolveUnsafe(Context context)
        {
            return await Resolve(context);
        }
    }
    namespace Unsafe
    {
        public interface IToken
        {
            public ITask<Resolution?> ResolveUnsafe(Context context);
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
            public override async ITask<R?> Resolve(Context context)
            {
                var o = await GetTokenResults(context);
                return o is not null ? TransformTokens(o) : null;
            }
            private async ITask<List<Resolution>?> GetTokenResults(Context context)
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
        public override ITask<R?> Resolve(Context context) => Task.FromResult(InfallibleResolve(context)).AsITask();
#pragma warning restore CS8619
        protected abstract R InfallibleResolve(Context context);
    }

    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IEnumerable&lt;IToken&lt;<typeparamref name="RArg1"/>&gt;>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    public abstract record Combiner<RArg, ROut> : Unsafe.TokenFunction<ROut>, Proxy.IHasCombineArgs<RArg, ROut>
        where RArg : Resolution
        where ROut : Resolution
    {
        public IEnumerable<IToken<RArg>> Args { get; private init; }
        protected Combiner(IEnumerable<IToken<RArg>> tokens) : base(tokens)
        {
            Args = tokens;
        }
        protected Combiner(params IToken<RArg>[] tokens) : base(tokens)
        {
            Args = tokens;
        }
        protected abstract ROut Evaluate(IEnumerable<RArg> inputs);
        protected override ROut TransformTokens(List<Resolution> tokens) => Evaluate(tokens.Map(x => (RArg)x));
    }

    #region Functions
    // ---- [ Functions ] ----
    
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    public abstract record Function<RArg1, ROut> : Unsafe.TokenFunction<ROut>,
        Proxy.IHasArg1<RArg1, ROut>
        where RArg1 : Resolution
        where ROut : Resolution
    {
        public IToken<RArg1> Arg1 { get; private init; }
        protected Function(IToken<RArg1> in1) : base(in1)
        {
            Arg1 = in1;
        }
        protected abstract ROut Evaluate(RArg1 in1);
        protected override ROut TransformTokens(List<Resolution> args) =>
            Evaluate((RArg1)args[0]);
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;, IToken&lt;<typeparamref name="RArg2"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    /// <typeparam name="RArg2"></typeparam>
    public abstract record Function<RArg1, RArg2, ROut> : Unsafe.TokenFunction<ROut>,
        Proxy.IHasArg1<RArg1, ROut>,
        Proxy.IHasArg2<RArg2, ROut>
        where RArg1 : Resolution
        where RArg2 : Resolution
        where ROut : Resolution
    {
        public IToken<RArg1> Arg1 { get; private init; }
        public IToken<RArg2> Arg2 { get; private init; }
        protected Function(IToken<RArg1> in1, IToken<RArg2> in2) : base(in1, in2)
        {
            Arg1 = in1;
            Arg2 = in2;
        }
        protected abstract ROut Evaluate(RArg1 in1, RArg2 in2);
        protected override ROut TransformTokens(List<Resolution> args) =>
            Evaluate((RArg1)args[0], (RArg2)args[1]);
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;, IToken&lt;<typeparamref name="RArg2"/>&gt;, IToken&lt;<typeparamref name="RArg3"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    /// <typeparam name="RArg2"></typeparam>
    /// <typeparam name="RArg3"></typeparam>
    public abstract record Function<RArg1, RArg2, RArg3, ROut> : Unsafe.TokenFunction<ROut>,
        Proxy.IHasArg1<RArg1, ROut>,
        Proxy.IHasArg2<RArg2, ROut>,
        Proxy.IHasArg3<RArg3, ROut>
        where RArg1 : Resolution
        where RArg2 : Resolution
        where RArg3 : Resolution
        where ROut : Resolution
    {
        public IToken<RArg1> Arg1 { get; private init; }
        public IToken<RArg2> Arg2 { get; private init; }
        public IToken<RArg3> Arg3 { get; private init; }
        protected Function(IToken<RArg1> in1, IToken<RArg2> in2, IToken<RArg3> in3) : base(in1, in2, in3)
        {
            Arg1 = in1;
            Arg2 = in2;
            Arg3 = in3;
        }
        protected abstract ROut Evaluate(RArg1 in1, RArg2 in2, RArg3 in3);
        protected override ROut TransformTokens(List<Resolution> args) =>
            Evaluate((RArg1)args[0], (RArg2)args[1], (RArg3)args[2]);
    }
    // --------
    #endregion

}
namespace Tokens
{
    using Token;
    using a = Resolutions;
    namespace Number
    {
        public sealed record Constant : Infallible<a.Number>
        {
            private int _value { get; init; }
            public Constant(int value) => _value = value;
            protected override a.Number InfallibleResolve(Context context) => new() { Value = _value };
        }
    }
}