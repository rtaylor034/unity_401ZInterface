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

    namespace Scope
    {
        public sealed record New<R> : Token.Unsafe.TokenFunction<R> where R : class, ResObj
        {
            public IToken<R> SubToken { get; init; }
            public New(params IToken<Res.Referable>[] referables) : base(referables) { }
            public override bool IsFallibleFunction => SubToken.IsFallible;
            protected override ITask<R?> TransformTokens(Context context, List<ResObj> _) => SubToken.ResolveWithRules(context);
        }
        public sealed record Reference<R> : Infallible<R> where R : class, ResObj
        {
            public string ToLabel { get; private init; }
            public Reference(string toLabel) => ToLabel = toLabel;
            protected override R InfallibleResolve(Context context)
            {
                return (context.Scope.Get(ToLabel) is R val) ? val :
                    throw new Exception($"Reference token resolved to non-existant or wrongly-typed object.\n" +
                    $"Label: {ToLabel}\n" +
                    $"Expected: {typeof(R).Name}\n" +
                    $"Recieved: {context.Scope.Get(ToLabel)?.GetType().Name}\n" +
                    $"Current Scope:\n" +
                    $"{context.Scope.Variables.AccumulateInto("", (msg, x) => msg + $"> {x.Key} :{x.Value}")}");
            }
        }
        public sealed record Variable : Token<Res.Referable>
        {
            public override bool IsFallible => ObjectToken.IsFallible;
            public Token.Unsafe.IToken ObjectToken { get; private init; }
            public string Label { get; private init; }
            public Variable(string label, Token.Unsafe.IToken token)
            {
                ObjectToken = token;
                Label = label;
            }
            public override async ITask<Res.Referable?> Resolve(Context context)
            {
                return new() { Label = Label, Object = await ObjectToken.ResolveUnsafe(context) };
            }
        }
    }
    namespace Number
    {
        public sealed record Constant : Infallible<Res.Number>
        {
            private int _value { get; init; }
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
                return new() { Values = inputs.Map(multi => multi.Values).Flatten() };
            }
        }
        public sealed record Yield<R> : Infallible<Res.Multi<R>> where R : class, ResObj
        {
            private R _value { get; init; }
            public Yield(R value) => _value = value;
            protected override Res.Multi<R> InfallibleResolve(Context context) => new() { Values = _value.Yield() };
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