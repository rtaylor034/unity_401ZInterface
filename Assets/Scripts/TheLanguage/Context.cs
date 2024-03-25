
using System.Collections.Generic;
using UnityEngine;
using Packets;
using System;
using GStructures;

#nullable enable

// All tokens are stateless with the exception of 'Referable' and 'Reference'
// functional (non-functioning) programming B)
namespace Tokens
{
    public interface IToken<out T> : IDisplayable
    {
        public IPacket<T> Evaluate();
    }
    public interface IDisplayable { }
    namespace Int
    {
        public sealed class Constant : IToken<int>
        {
            public readonly int Value;
            public Constant(int value) => Value = value;
            public IPacket<int> Evaluate() => new Static<int>(this, Value);

        }
        public sealed class BinaryOperation : IToken<int>
        {
            public enum EOperation { Add, Subtract, Multiply, Divide }
            public readonly IToken<int> Left;
            public readonly IToken<int> Right;
            public readonly EOperation Operation;
            public BinaryOperation(IToken<int> left, IToken<int> right, EOperation operation)
            {
                Left = left;
                Right = right;
                Operation = operation;
            }
            public IPacket<int> Evaluate()
            {
                Func<int, int, int> function = Operation switch
                {
                    EOperation.Add => (a, b) => a + b,
                    EOperation.Subtract => (a, b) => a - b,
                    EOperation.Multiply => (a, b) => a * b,
                    EOperation.Divide => (a, b) => a / b,
                };
                return new Packets.Function.Combine<int, int, int>(this, Left.Evaluate(), Right.Evaluate(), function);
            }
        }
        public sealed class UnaryOperation : IToken<int>
        {
            public enum EOperation { Negate }
            public readonly IToken<int> Value;
            public readonly EOperation Operation;
            public UnaryOperation(IToken<int> value, EOperation operation)
            {
                Value = value;
                Operation = operation;
            }
            public IPacket<int> Evaluate()
            {
                Func<int, int> function = Operation switch
                {
                    EOperation.Negate => x => -x
                };
                return new Packets.Function.Transform<int, int>(this, Value.Evaluate(), function);
            }
        }
    }
    namespace Select
    {
        public sealed class One<T> : IToken<T>
        {
            public readonly IToken<IEnumerable<T>> From;
            public One(IToken<IEnumerable<T>> from) => From = from;
            public IPacket<T> Evaluate() => new Packets.Select.One<T>(this, From.Evaluate());
        }
        public sealed class Multiple<T> : IToken<IEnumerable<T>>
        {
            public readonly IToken<IEnumerable<T>> From;
            public Multiple(IToken<IEnumerable<T>> from) => From = from;
            public IPacket<IEnumerable<T>> Evaluate() => new Packets.Select.Multiple<T>(this, From.Evaluate());
        }
    }
    namespace Merge
    {
        public sealed class Collect<T> : IToken<IEnumerable<T>>
        {
            public readonly IEnumerable<IToken<T>> Elements;
            public Collect(IEnumerable<IToken<T>> elements) => Elements = elements;
            public IPacket<IEnumerable<T>> Evaluate() => new Packets.Merge.Collect<T>(this, Elements.Map(token => token.Evaluate()));
        }
        public sealed class Union<T> : IToken<IEnumerable<T>>
        {
            public readonly IEnumerable<IToken<IEnumerable<T>>> Elements;
            public Union(IEnumerable<IToken<IEnumerable<T>>> elements) => Elements = elements;
            public IPacket<IEnumerable<T>> Evaluate() => new Packets.Merge.Union<T>(this, Elements.Map(token => token.Evaluate()));
        }
    }
    // our special friends
    public sealed class Referable<T> : IToken<T>
    {
        public readonly IToken<T> Value;
        public readonly string Label;
        private Option<Packets.Referable<T>> _evaluation;
        public Referable(IToken<T> value, string label)
        {
            Label = label;
            Value = value;
            _evaluation = new Option<Packets.Referable<T>>.None();
        }
        public IPacket<T> Evaluate()
        {
            _evaluation = _evaluation switch
            {
                Option<Packets.Referable<T>>.Some v => v,
                _ => new Option<Packets.Referable<T>>.Some(new Packets.Referable<T>(this, Value.Evaluate()))
            };
            return _evaluation.Unwrap();
        }
    }
    // i bet a foreach token is possible, but lets not for now yea.
    public sealed class Reference<T> : IToken<T>
    {
        public readonly Referable<T> RefersTo;
        public Reference(Referable<T> refersTo) => RefersTo = refersTo;
        public IPacket<T> Evaluate() => RefersTo.Evaluate();
    }
}