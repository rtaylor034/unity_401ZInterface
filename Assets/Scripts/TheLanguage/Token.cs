using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
using ResObj = Resolution.IResolution;
using GExtensions;

#nullable enable
namespace Tokens
{
    using Token;
    using Res = Resolutions;
    namespace Number
    {
        public sealed record Constant : Infallible<Res.Number>
        {
            private int _value { get; init; }
            public Constant(int value) => _value = value;
            protected override Res.Number InfallibleResolve(Context context) => new() { Value = _value };
        }
        public sealed record BinaryOperation : Function<Res.Number, Res.Number, Res.Number>
        {
            public enum EOp { Add, Subtract, Multiply, FloorDivide }
            public EOp Operation { get; init; }
            public BinaryOperation(IToken<Res.Number> operand1, IToken<Res.Number> operand2) : base(operand1, operand2) { }

            protected override Res.Number Evaluate(Res.Number a, Res.Number b) => new()
            {
                Value = Operation switch
                {
                    EOp.Add => a.Value + b.Value,
                    EOp.Subtract => a.Value - b.Value,
                    EOp.Multiply => a.Value * b.Value,
                    EOp.FloorDivide => a.Value / b.Value
                }
            };
        }
        public sealed record UnaryOperation : Function<Res.Number, Res.Number>
        {
            public enum EOp { Negate }
            public EOp Operation { get; init; }
            public UnaryOperation(IToken<Res.Number> operand) : base(operand) { }
            protected override Res.Number Evaluate(Res.Number operand) => new()
            {
                Value = Operation switch
                {
                    EOp.Negate => - operand.Value
                }
            };
        }
    }
    namespace Multi
    {
        public sealed record Union<R> : Combiner<Res.Multi<R>, Res.Multi<R>> where R : class, ResObj
        {
            protected override Res.Multi<R> Evaluate(IEnumerable<Res.Multi<R>> inputs)
            {
                return new() { Values = inputs.Map(multi => multi.Values).Flatten() };
            }
        }
        public sealed record Yield<R> : Infallible<Res.Multi<R>> where R : class, ResObj
        {
            private R _value { get; init; }
            public Yield(R value) => _value = value;
            protected override Res.Multi<R> InfallibleResolve(Context context) => new() { Values = _value.Wrapped() };
        }
    }
    
}
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
#pragma warning disable CS8619
        public override ITask<R?> Resolve(Context context) => Task.FromResult(InfallibleResolve(context)).AsITask();
#pragma warning restore CS8619
        protected abstract R InfallibleResolve(Context context);
    }

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
        protected abstract ROut Evaluate(IEnumerable<RArg> inputs);
        protected override ROut TransformTokens(List<ResObj> tokens) => Evaluate(tokens.Map(x => (RArg)x));
        
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
        protected abstract ROut Evaluate(RArg1 in1, RArg2 in2, RArg3 in3);
        protected override ROut TransformTokens(List<ResObj> args) =>
            Evaluate((RArg1)args[0], (RArg2)args[1], (RArg3)args[2]);
    }
    // --------
    #endregion
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
            => token.ApplyRules(rule.Wrapped());
    }
}
namespace Token.Unsafe
{
    using Token;
    public interface IToken
    {
        public ITask<ResObj?> ResolveWithRulesUnsafe(Context context);
        public ITask<ResObj?> ResolveUnsafe(Context context);
    }
    public abstract record TokenFunction<R> : Token<R> where R : class, ResObj
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
                        contexts[i + 1] = context.WithResolution(resolution);
                        continue;
                    case null:
                        i -= 2;
                        if (i < 0) return null;
                        continue;
                }
            }
            return o;
        }
    }
    public interface IHasArg1 : IToken { }
    public interface IHasArg2 : IHasArg1 { }
    public interface IHasArg3 : IHasArg2 { }
}
