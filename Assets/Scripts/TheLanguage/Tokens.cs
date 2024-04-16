using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
using ResObj = Resolution.IResolution;
using Token;
using Res = Resolutions;

#nullable enable
namespace Tokens
{

    public sealed record SubEnvironment<R> : Token.Unsafe.TokenFunction<R> where R : class, ResObj
    {
        public IToken<R> SubToken { get; init; }
        public sealed override bool IsFallibleFunction => SubToken.IsFallible;
        public SubEnvironment(params IToken<Resolution.Operation>[] envModifiers) : base(envModifiers) { }
        public SubEnvironment(IEnumerable<Token.Unsafe.IToken> envModifiers) : base(envModifiers) { }
        protected sealed override async ITask<R?> TransformTokens(Context context, List<ResObj> _) => await SubToken.ResolveWithRules(context);
    }
    public sealed record Variable<R> : Token<Res.DeclareVariable> where R : class, ResObj
    {
        public override bool IsFallible => ObjectToken.IsFallible;
        public readonly IToken<R> ObjectToken;
        public readonly string Label;
        public Variable(string label, IToken<R> token)
        {
            ObjectToken = token;
            Label = label;
        }
        public override async ITask<Res.DeclareVariable?> Resolve(Context context)
        {
            return (await ObjectToken.ResolveWithRules(context) is Res.DeclareVariable res) ?
                new() { Label = Label, Object = res } : null;
        }
    }
    public sealed record Reference<R> : Infallible<R> where R : class, ResObj
    {
        private readonly string _toLabel;
        public Reference(string toLabel) => _toLabel = toLabel;
        protected override R InfallibleResolve(Context context)
        {
            return (context.Variables[_toLabel] is R val) ? val :
                throw new Exception($"Reference token resolved to non-existent or wrongly-typed object.\n" +
                $"Label: '{_toLabel}'\n" +
                $"Expected: {typeof(R).Name}\n" +
                $"Recieved: {context.Variables[_toLabel]?.GetType().Name}\n" +
                $"Current Scope:\n" +
                $"{context.Variables.AccumulateInto("", (msg, x) => msg + $"> '{x.key}' : {x.val}")}");
        }
    }
    namespace Number
    {
        public sealed record Constant : Infallible<Res.Number>
        {
            private readonly int _value;
            public Constant(int value) => _value = value;
            protected override Res.Number InfallibleResolve(Context context) => new() { Value = _value };
        }
        public sealed record Add : PureFunction<Res.Number, Res.Number, Res.Number>
        {
            public Add(IToken<Res.Number> operand1, IToken<Res.Number> operand2) : base(operand1, operand2) { }
            protected override Res.Number EvaluatePure(Res.Number a, Res.Number b) => new() { Value = a.Value + b.Value };
        }
        public sealed record Subtract : PureFunction<Res.Number, Res.Number, Res.Number>
        {
            public Subtract(IToken<Res.Number> operand1, IToken<Res.Number> operand2) : base(operand1, operand2) { }
            protected override Res.Number EvaluatePure(Res.Number a, Res.Number b) => new() { Value = a.Value - b.Value };
        }
        public sealed record Multiply : PureFunction<Res.Number, Res.Number, Res.Number>
        {
            public Multiply(IToken<Res.Number> operand1, IToken<Res.Number> operand2) : base(operand1, operand2) { }
            protected override Res.Number EvaluatePure(Res.Number a, Res.Number b) => new() { Value = a.Value * b.Value };
        }
        public sealed record Negate : PureFunction<Res.Number, Res.Number>
        {
            public Negate(IToken<Res.Number> operand) : base(operand) { }
            protected override Res.Number EvaluatePure(Res.Number operand) => new() { Value = -operand.Value };
        }
    }
    namespace Multi
    {
        public sealed record Union<R> : PureCombiner<Res.Multi<R>, Res.Multi<R>> where R : class, ResObj
        {
            protected override Res.Multi<R> EvaluatePure(IEnumerable<Res.Multi<R>> inputs)
            {
                return new() { Elements = new(inputs.Map(multi => multi.Elements).Flatten()) };
            }
        }
        public sealed record Yield<R> : Infallible<Res.Multi<R>> where R : class, ResObj
        {
            private readonly R _value;
            public Yield(R value) => _value = value;
            protected override Res.Multi<R> InfallibleResolve(Context context) => new() { Elements = new(_value.Yield()) };
        }
    }
    namespace Select
    {
        public sealed record One<R> : Function<Res.Multi<R>, R> where R : class, ResObj
        {
            public One(IToken<Res.Multi<R>> from) : base(from) { }
            public override bool IsFallibleFunction => true;
            protected override ITask<R?> Evaluate(Context context, Res.Multi<R> in1)
            {
                throw new NotImplementedException();
            }
        }
        public sealed record Multiple<R> : Function<Res.Multi<R>, Res.Multi<R>> where R : class, ResObj
        {
            public Multiple(IToken<Res.Multi<R>> from) : base(from) { }
            public override bool IsFallibleFunction => true;
            protected override ITask<Res.Multi<R>?> Evaluate(Context context, Res.Multi<R> in1)
            {
                throw new NotImplementedException();
            }
        }
    }
}