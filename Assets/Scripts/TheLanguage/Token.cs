using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
using ResObj = Resolution.Resolution;

#nullable enable
namespace Token
{
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
        public Context WithResObj(ResObj resolution) => resolution._ChangeContext(this);
    }
#nullable enable
    #endregion

    public interface IToken<out R> : Unsafe.IToken where R : ResObj
    {
        public ITask<R?> Resolve(Context context);
    }
    public abstract record Token<R> : IToken<R> where R : ResObj
    {
        public abstract ITask<R?> Resolve(Context context);
        public async ITask<ResObj?> ResolveUnsafe(Context context)
        {
            return await Resolve(context);
        }
    }
    public abstract record Infallible<R> : Token<R> where R : ResObj
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
    public abstract record Combiner<RArg, ROut> : Unsafe.TokenFunction<ROut>, IHasCombineArgs<RArg, ROut>
        where RArg : ResObj
        where ROut : ResObj
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
        protected override ROut TransformTokens(List<ResObj> tokens) => Evaluate(tokens.Map(x => (RArg)x));
        
    }

    #region Functions
    // ---- [ Functions ] ----
    public interface IHasArg1<out RArg, out ROut> : Unsafe.IHasArg1<ROut>
        where RArg : ResObj
        where ROut : ResObj
    { public IToken<RArg> Arg1 { get; } }
    public interface IHasArg2<out RArg, out ROut> : Unsafe.IHasArg2<ROut>
        where RArg : ResObj
        where ROut : ResObj
    { public IToken<RArg> Arg2 { get; } }
    public interface IHasArg3<out RArg, out ROut> : Unsafe.IHasArg3<ROut>
        where RArg : ResObj
        where ROut : ResObj
    { public IToken<RArg> Arg3 { get; } }
    public interface IHasCombineArgs<out RArgs, out ROut> : IToken<ROut> where RArgs : ResObj where ROut : ResObj
    {
        public IEnumerable<IToken<RArgs>> Args { get; }
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    public abstract record Function<RArg1, ROut> : Unsafe.TokenFunction<ROut>,
        IHasArg1<RArg1, ROut>
        where RArg1 : ResObj
        where ROut : ResObj
    {
        public IToken<RArg1> Arg1 { get; private init; }
        protected Function(IToken<RArg1> in1) : base(in1)
        {
            Arg1 = in1;
        }
        protected abstract ROut Evaluate(RArg1 in1);
        protected override ROut TransformTokens(List<ResObj> args) =>
            Evaluate((RArg1)args[0]);
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;, IToken&lt;<typeparamref name="RArg2"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    /// <typeparam name="RArg2"></typeparam>
    public abstract record Function<RArg1, RArg2, ROut> : Unsafe.TokenFunction<ROut>,
        IHasArg1<RArg1, ROut>,
        IHasArg2<RArg2, ROut>
        where RArg1 : ResObj
        where RArg2 : ResObj
        where ROut : ResObj
    {
        public IToken<RArg1> Arg1 { get; private init; }
        public IToken<RArg2> Arg2 { get; private init; }
        protected Function(IToken<RArg1> in1, IToken<RArg2> in2) : base(in1, in2)
        {
            Arg1 = in1;
            Arg2 = in2;
        }
        protected abstract ROut Evaluate(RArg1 in1, RArg2 in2);
        protected override ROut TransformTokens(List<ResObj> args) =>
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
        IHasArg1<RArg1, ROut>,
        IHasArg2<RArg2, ROut>,
        IHasArg3<RArg3, ROut>
        where RArg1 : ResObj
        where RArg2 : ResObj
        where RArg3 : ResObj
        where ROut : ResObj
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
        protected override ROut TransformTokens(List<ResObj> args) =>
            Evaluate((RArg1)args[0], (RArg2)args[1], (RArg3)args[2]);
    }
    // --------
    #endregion

}
namespace Token.Unsafe
{
    using Token;
    public interface IToken
    {
        public ITask<ResObj?> ResolveUnsafe(Context context);
    }
    public abstract record TokenFunction<R> : Token<R> where R : ResObj
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
        protected abstract R TransformTokens(List<ResObj> tokens);
        public override async ITask<R?> Resolve(Context context)
        {
            var o = await GetTokenResults(context);
            return o is not null ? TransformTokens(o) : null;
        }
        private async ITask<List<ResObj>?> GetTokenResults(Context context)
        {
            List<ResObj> o = new(ArgTokens.Count);
            List<Context> contexts = new(ArgTokens.Count + 1) { context };
            for (int i = 0; i < ArgTokens.Count; i++)
            {
                switch (await ArgTokens[i].ResolveUnsafe(contexts[i]))
                {
                    case ResObj resolution:
                        o[i] = resolution;
                        contexts[i + 1] = context.WithResObj(resolution);
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
    public interface IHasArg1<out ROut> : Token.IToken<ROut> where ROut : ResObj { }
    public interface IHasArg2<out ROut> : IHasArg1<ROut> where ROut : ResObj { }
    public interface IHasArg3<out ROut> : IHasArg2<ROut> where ROut : ResObj { }
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
        public sealed record Add_EX : Function<a.Number, a.Number, a.Number>
        {
            public Add_EX(IToken<a.Number> in1, IToken<a.Number> in2) : base(in1, in2) { }
            protected override a.Number Evaluate(a.Number in1, a.Number in2)
            {
                return new() { Value = in1.Value + in2.Value };
            }
        }
    }
}