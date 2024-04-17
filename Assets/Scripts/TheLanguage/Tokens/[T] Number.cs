using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r_ = Resolutions;
namespace Tokens.Number
{
    public sealed record Constant : Infallible<r_.Number>
    {
        private readonly int _value;
        public Constant(int value) => _value = value;
        protected override r_.Number InfallibleResolve(Context context) => new() { Value = _value };
    }
    public sealed record Add : PureFunction<r_.Number, r_.Number, r_.Number>
    {
        public Add(IToken<r_.Number> operand1, IToken<r_.Number> operand2) : base(operand1, operand2) { }
        protected override r_.Number EvaluatePure(r_.Number a, r_.Number b) => new() { Value = a.Value + b.Value };
    }
    public sealed record Subtract : PureFunction<r_.Number, r_.Number, r_.Number>
    {
        public Subtract(IToken<r_.Number> operand1, IToken<r_.Number> operand2) : base(operand1, operand2) { }
        protected override r_.Number EvaluatePure(r_.Number a, r_.Number b) => new() { Value = a.Value - b.Value };
    }
    public sealed record Multiply : PureFunction<r_.Number, r_.Number, r_.Number>
    {
        public Multiply(IToken<r_.Number> operand1, IToken<r_.Number> operand2) : base(operand1, operand2) { }
        protected override r_.Number EvaluatePure(r_.Number a, r_.Number b) => new() { Value = a.Value * b.Value };
    }
    public sealed record Negate : PureFunction<r_.Number, r_.Number>
    {
        public Negate(IToken<r_.Number> operand) : base(operand) { }
        protected override r_.Number EvaluatePure(r_.Number operand) => new() { Value = -operand.Value };
    }
}