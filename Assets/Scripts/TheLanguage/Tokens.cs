using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
using ResObj = Resolution.IResolution;
using GExtensions;
using Token;
using Res = Resolutions;

#nullable enable
namespace Tokens
{

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
                    EOp.Negate => -operand.Value
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
            protected override Res.Multi<R> InfallibleResolve(Context context) => new() { Values = _value.Yield() };
        }
    }
    namespace Select
    {
        public sealed record One<R> : Token<R> where R : class, ResObj
        {
            public override bool IsFallible => throw new NotImplementedException();
            public override ITask<R?> Resolve(Context context)
            {
                throw new NotImplementedException();
            }
        }
    }
}