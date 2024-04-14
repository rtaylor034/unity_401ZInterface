using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
using ResObj = Resolution.IResolution;
using GExtensions;

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
        public List<Rule.IRule> Rules { get; init; }
        public Context WithResolution(ResObj resolution) => resolution.ChangeContext(this);
    }
#nullable enable
    #endregion

    public interface IToken<out R> : Unsafe.IToken where R : class, ResObj
    {
        public ITask<R?> ResolveWithRules(Context context);
        public ITask<R?> Resolve(Context context);
    }
    public abstract record Token<R> : IToken<R> where R : class, ResObj
    {
        public abstract bool IsFallible { get; }
        public abstract ITask<R?> Resolve(Context context);

        public async ITask<R?> ResolveWithRules(Context context)
        {
            return await this.ApplyRules(context.Rules).Resolve(context);
        }
        public async ITask<ResObj?> ResolveUnsafe(Context context)
        {
            return await Resolve(context);
        }
        public async ITask<ResObj?> ResolveWithRulesUnsafe(Context context)
        {
            return await ResolveWithRules(context);
        }
    }
    public abstract record Infallible<R> : Token<R> where R : class, ResObj
    {
        public override bool IsFallible => false;
#pragma warning disable CS8619
        public override ITask<R?> Resolve(Context context) => Task.FromResult(InfallibleResolve(context)).AsITask();
#pragma warning restore CS8619
        protected abstract R InfallibleResolve(Context context);
    }
    #region Functions
    // ---- [ Functions ] ----
    public interface IHasArg1<out RArg> : Unsafe.IHasArg1 where RArg : class, ResObj
    { public IToken<RArg> Arg1 { get; } }
    public interface IHasArg2<out RArg> : Unsafe.IHasArg2 where RArg : class, ResObj
    { public IToken<RArg> Arg2 { get; } }
    public interface IHasArg3<out RArg> : Unsafe.IHasArg3 where RArg : class, ResObj
    { public IToken<RArg> Arg3 { get; } }
    public interface IHasCombineArgs<out RArgs> : Unsafe.IToken where RArgs : class, ResObj
    {
        public IEnumerable<IToken<RArgs>> Args { get; }
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    public abstract record Function<RArg1, ROut> : Unsafe.TokenFunction<ROut>,
        IHasArg1<RArg1>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    {
        public IToken<RArg1> Arg1 { get; private init; }
        protected Function(IToken<RArg1> in1) : base(in1)
        {
            Arg1 = in1;
        }
        protected abstract ITask<ROut?> Evaluate(RArg1 in1);
        protected override ITask<ROut?> TransformTokens(List<ResObj> args) =>
            Evaluate((RArg1)args[0]);
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;, IToken&lt;<typeparamref name="RArg2"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    /// <typeparam name="RArg2"></typeparam>
    public abstract record Function<RArg1, RArg2, ROut> : Unsafe.TokenFunction<ROut>,
        IHasArg1<RArg1>,
        IHasArg2<RArg2>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where ROut : class, ResObj
    {
        public IToken<RArg1> Arg1 { get; private init; }
        public IToken<RArg2> Arg2 { get; private init; }
        protected Function(IToken<RArg1> in1, IToken<RArg2> in2) : base(in1, in2)
        {
            Arg1 = in1;
            Arg2 = in2;
        }
        protected abstract ITask<ROut?> Evaluate(RArg1 in1, RArg2 in2);
        protected override ITask<ROut?> TransformTokens(List<ResObj> args) =>
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
        IHasArg1<RArg1>,
        IHasArg2<RArg2>,
        IHasArg3<RArg3>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where RArg3 : class, ResObj
        where ROut : class, ResObj
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
        protected abstract ITask<ROut?> Evaluate(RArg1 in1, RArg2 in2, RArg3 in3);
        protected override ITask<ROut?> TransformTokens(List<ResObj> args) =>
            Evaluate((RArg1)args[0], (RArg2)args[1], (RArg3)args[2]);
    }
    #region Pure Functions
    // -- [ Pure Functions ] --
    public abstract record PureFunction<RArg1, ROut> : Function<RArg1, ROut>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    {
        public sealed override bool IsFallibleFunction => false;
        protected PureFunction(IToken<RArg1> in1) : base(in1) { }
        protected abstract ROut EvaluatePure(RArg1 in1);
        protected sealed override ITask<ROut?> Evaluate(RArg1 in1) => Task.FromResult(EvaluatePure(in1)).AsITask();
    }
    public abstract record PureFunction<RArg1, RArg2, ROut> : Function<RArg1, RArg2, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where ROut : class, ResObj
    {
        public sealed override bool IsFallibleFunction => false;
        protected PureFunction(IToken<RArg1> in1, IToken<RArg2> in2) : base(in1, in2) { }
        protected abstract ROut EvaluatePure(RArg1 in1, RArg2 in2);
        protected sealed override ITask<ROut?> Evaluate(RArg1 in1, RArg2 in2) => Task.FromResult(EvaluatePure(in1, in2)).AsITask();
    }
    public abstract record PureFunction<RArg1, RArg2, RArg3, ROut> : Function<RArg1, RArg2, RArg3, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where RArg3 : class, ResObj
        where ROut : class, ResObj
    {
        public sealed override bool IsFallibleFunction => false;
        protected PureFunction(IToken<RArg1> in1, IToken<RArg2> in2, IToken<RArg3> in3) : base(in1, in2, in3) { }
        protected abstract ROut EvaluatePure(RArg1 in1, RArg2 in2, RArg3 in3);
        protected sealed override ITask<ROut?> Evaluate(RArg1 in1, RArg2 in2, RArg3 in3) => Task.FromResult(EvaluatePure(in1, in2, in3)).AsITask();
    }
    // ----
    #endregion
    // --------
    #endregion

    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IEnumerable&lt;IToken&lt;<typeparamref name="RArg"/>&gt;>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg"></typeparam>
    public abstract record Combiner<RArg, ROut> : Unsafe.TokenFunction<ROut>, IHasCombineArgs<RArg>
        where RArg : class, ResObj
        where ROut : class, ResObj
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
        protected abstract ITask<ROut?> Evaluate(IEnumerable<RArg> inputs);
        protected sealed override ITask<ROut?> TransformTokens(List<ResObj> tokens) => Evaluate(tokens.Map(x => (RArg)x));

    }
    public abstract record PureCombiner<RArg, ROut> : Combiner<RArg, ROut>
        where RArg : class, ResObj
        where ROut : class, ResObj
    {
        public sealed override bool IsFallibleFunction => false;
        protected PureCombiner(params IToken<RArg>[] tokens) : this(tokens as IEnumerable<IToken<RArg>>) { }
        protected PureCombiner(IEnumerable<IToken<RArg>> tokens) : base(tokens) { }
        protected abstract ROut EvaluatePure(IEnumerable<RArg> inputs);
        protected sealed override ITask<ROut?> Evaluate(IEnumerable<RArg> inputs) => Task.FromResult(EvaluatePure(inputs)).AsITask();

    }
    public static class Extensions
    {
        public static IToken<R> ApplyRules<R>(this IToken<R> token, IEnumerable<Rule.IRule> rules) where R : class, ResObj
        {
            var o = token;
            foreach (var rule in rules)
            {
                if (rule.TryApplyTyped(o) is IToken<R> newToken) o = newToken;
            }
            return o;
        }
        public static IToken<R> ApplyRule<R>(this IToken<R> token, Rule.IRule rule) where R : class, ResObj
            => token.ApplyRules(rule.Yield());

    }
}
namespace Token.Unsafe
{
    using Token;
    public interface IToken
    {
        public bool IsFallible { get; }
        public ITask<ResObj?> ResolveWithRulesUnsafe(Context context);
        public ITask<ResObj?> ResolveUnsafe(Context context);
    }
    public abstract record TokenFunction<R> : Token<R> where R : class, ResObj
    {
        //abstract and move this definition to pure
        public sealed override bool IsFallible => IsFallibleFunction || ArgTokens.Map(x => x.IsFallible).HasMatch(x => x == true);
        public abstract bool IsFallibleFunction { get; }
        protected List<IToken> ArgTokens { get; init; }
        private State _state { get; set; }
        protected TokenFunction(params IToken[] tokens) : this(tokens as IEnumerable<IToken>) { }
        protected TokenFunction(IEnumerable<IToken> tokens)
        {
            ArgTokens = new(tokens);
            _state = new(ArgTokens.Count);
        }
        public TokenFunction(TokenFunction<R> original) : base(original)
        {
            ArgTokens = new(original.ArgTokens);
            _state = new(ArgTokens.Count);
        }
        protected abstract ITask<R?> TransformTokens(List<ResObj> tokens);
        public sealed override async ITask<R?> Resolve(Context context)
        {
            _state.Contexts[_state.Index] = context;
            while (_state.Index >= 0) 
            {
                if (_state.Index == ArgTokens.Count)
                {
                    if (await TransformTokens(_state.Inputs) is R o) return o;
                    _state.Index--;
                }
                switch (await ArgTokens[_state.Index].ResolveUnsafe(_state.Contexts[_state.Index]))
                {
                    case ResObj resolution:
                        _state.Inputs[_state.Index] = resolution;
                        _state.Contexts[_state.Index + 1] = context.WithResolution(resolution);
                        _state.Index++;
                        continue;
                    case null:
                        while (--_state.Index >= 0 && !ArgTokens[_state.Index].IsFallible) { }
                        continue;
                }
            }
            _state.Index = 0;
            return null;
        }
        // I dont want this to exist, but im not sure how else to have full backward cancel traversability.
        private class State
        {
            public int Index { get; set; }
            public List<Context> Contexts { get; set; }
            public List<ResObj> Inputs { get; set; }
            public State(int argCount)
            {
                Index = 0;
                Contexts = new(argCount + 1);
                Inputs = new(argCount);
            }
        }
    }
    public interface IHasArg1 : IToken { }
    public interface IHasArg2 : IHasArg1 { }
    public interface IHasArg3 : IHasArg2 { }
}
