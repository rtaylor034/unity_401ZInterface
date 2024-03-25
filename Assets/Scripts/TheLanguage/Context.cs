
using System.Collections.Generic;
using UnityEngine;
using Token;
using Packet;
using System;
using GStructures;

#nullable enable

// All tokens are stateless with the exception of 'Referable' and 'Reference'
// functional (non-functioning) programming B)
namespace Context
{
    public interface IContextData { }
    
    namespace Any
    {
        public class Data : IContextData
        {
            
        }
        namespace Tokens
        {
            namespace Int
            {
                public sealed class Constant : IToken<int, Data>
                {
                    public readonly int Value;
                    public Constant(int value) => Value = value;
                    public IPacket<int> Evaluate(Data _) => new Static<int>(this, Value);

                }
            }
        }
    }
    namespace Global
    {
        public class Data : Any.Data
        {
            public HashSet<Unit> AllUnits;
        }
        namespace Tokens
        {
            public sealed class AllUnits : IToken<IEnumerable<Unit>, Data> { public IPacket<IEnumerable<Unit>> Evaluate(Data context) => new Static<IEnumerable<Unit>>(this, context.AllUnits); }
        }
    }
    namespace Ability
    {
        public class Data : Global.Data
        {
            public Unit Source;
        }
        namespace Tokens
        {
            public sealed class Source : IToken<Unit, Data> { public IPacket<Unit> Evaluate(Data context) => new Static<Unit>(this, context.Source); }
        }
    }
    namespace Generic
    {
        namespace Tokens
        {
            namespace Int
            {
                public sealed class BinaryOperation<C> : IToken<int, C> where C : IContextData
                {
                    public enum EOperation { Add, Subtract, Multiply, Divide }
                    public readonly IToken<int, C> Left;
                    public readonly IToken<int, C> Right;
                    public readonly EOperation Operation;
                    public BinaryOperation(IToken<int, C> left, IToken<int, C> right, EOperation operation)
                    {
                        Left = left;
                        Right = right;
                        Operation = operation;
                    }
                    public IPacket<int> Evaluate(C context)
                    {
                        Func<int, int, int> function = Operation switch
                        {
                            EOperation.Add => (a, b) => a + b,
                            EOperation.Subtract => (a, b) => a - b,
                            EOperation.Multiply => (a, b) => a * b,
                            EOperation.Divide => (a, b) => a / b,
                        };
                        return new Packet.Function.Combine<int, int, int>(this, Left.Evaluate(context), Right.Evaluate(context), function);
                    }
                }
                public sealed class UnaryOperation<C> : IToken<int, C> where C : IContextData
                {
                    public enum EOperation { Negate }
                    public readonly IToken<int, C> Value;
                    public readonly EOperation Operation;
                    public UnaryOperation(IToken<int, C> value, EOperation operation)
                    {
                        Value = value;
                        Operation = operation;
                    }
                    public IPacket<int> Evaluate(C context)
                    {
                        Func<int, int> function = Operation switch
                        {
                            EOperation.Negate => x => -x
                        };
                        return new Packet.Function.Transform<int, int>(this, Value.Evaluate(context), function);
                    }
                }
            }
            namespace Select
            {
                public sealed class One<T, C> : IToken<T, C> where C : IContextData
                {
                    public readonly IToken<IEnumerable<T>, C> From;
                    public One(IToken<IEnumerable<T>, C> from) => From = from;
                    public IPacket<T> Evaluate(C context) => new Packet.Select.One<T>(this, From.Evaluate(context));
                }
                public sealed class Multiple<T, C> : IToken<IEnumerable<T>, C> where C : IContextData
                {
                    public readonly IToken<IEnumerable<T>, C> From;
                    public Multiple(IToken<IEnumerable<T>, C> from) => From = from;
                    public IPacket<IEnumerable<T>> Evaluate(C context) => new Packet.Select.Multiple<T>(this, From.Evaluate(context));
                }
            }
            namespace Merge
            {
                public sealed class Collect<T, C> : IToken<IEnumerable<T>, C> where C : IContextData
                {
                    public readonly IEnumerable<IToken<T, C>> Elements;
                    public Collect(IEnumerable<IToken<T, C>> elements) => Elements = elements;
                    public IPacket<IEnumerable<T>> Evaluate(C context) => new Packet.Merge.Collect<T>(this, Elements.Map(token => token.Evaluate(context)));
                }
                public sealed class Union<T, C> : IToken<IEnumerable<T>, C> where C : IContextData
                {
                    public readonly IEnumerable<IToken<IEnumerable<T>, C>> Elements;
                    public Union(IEnumerable<IToken<IEnumerable<T>, C>> elements) => Elements = elements;
                    public IPacket<IEnumerable<T>> Evaluate(C context) => new Packet.Merge.Union<T>(this, Elements.Map(token => token.Evaluate(context)));
                }
            }
            // our special friends
            public sealed class Referable<T, C> : IToken<T, C> where C : IContextData
            {
                public readonly IToken<T, C> Value;
                public readonly string Label;
                private Option<Packet.Referable<T>> _evaluation;
                public Referable(IToken<T, C> value, string label)
                {
                    Label = label;
                    Value = value;
                    _evaluation = new Option<Packet.Referable<T>>.None();
                }
                public IPacket<T> Evaluate(C context)
                {
                    _evaluation = _evaluation switch
                    {
                        Option<Packet.Referable<T>>.Some v => v,
                        _ => new Option<Packet.Referable<T>>.Some(new Packet.Referable<T>(this, Value.Evaluate(context)))
                    };
                    return _evaluation.Unwrap();
                }
            }
            // i bet a foreach token is possible, but lets not for now yea.
            public sealed class Reference<T, C> : IToken<T, C> where C : IContextData
            {
                public readonly Referable<T, C> RefersTo;
                public Reference(Referable<T, C> refersTo) => RefersTo = refersTo;
                public IPacket<T> Evaluate(C context) => RefersTo.Evaluate(context);
            }
        }
    }
}