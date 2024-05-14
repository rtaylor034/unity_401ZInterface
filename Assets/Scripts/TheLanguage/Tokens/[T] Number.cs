using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r = Resolutions;
using Resolutions;
namespace Tokens.Number
{

    public sealed record Add : PureFunction<r.Number, r.Number, r.Number>
    {
        public Add(IToken<r.Number> operand1, IToken<r.Number> operand2) : base(operand1, operand2) { }
        protected override r.Number EvaluatePure(r.Number a, r.Number b) { return new() { Value = a.Value + b.Value }; }
    }

    public sealed record Subtract : PureFunction<r.Number, r.Number, r.Number>
    {
        public Subtract(IToken<r.Number> operand1, IToken<r.Number> operand2) : base(operand1, operand2) { }
        protected override r.Number EvaluatePure(r.Number a, r.Number b) { return new() { Value = a.Value - b.Value }; }
    }

    public sealed record Multiply : PureFunction<r.Number, r.Number, r.Number>
    {
        public Multiply(IToken<r.Number> operand1, IToken<r.Number> operand2) : base(operand1, operand2) { }
        protected override r.Number EvaluatePure(r.Number a, r.Number b) { return new() { Value = a.Value * b.Value }; }
    }

    public sealed record Negate : PureFunction<r.Number, r.Number>
    {
        public Negate(IToken<r.Number> operand) : base(operand) { }
        protected override r.Number EvaluatePure(r.Number operand) { return new() { Value = -operand.Value }; }
    }
    namespace Compare
    {
        public sealed record GreaterThan : PureFunction<r.Number, r.Number, r.Bool>
        {
            public GreaterThan(IToken<r.Number> a, IToken<r.Number> b) : base(a, b) { }
            protected override r.Bool EvaluatePure(r.Number in1, r.Number in2)
            {
                return new() { IsTrue = in1.Value > in2.Value };
            }
        }
    }
}