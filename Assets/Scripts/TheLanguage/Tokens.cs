
using System.Collections.Generic;
using UnityEngine;
using Packets;
using System;
using GStructures;
using System.Net.Sockets;
using MorseCode.ITask;
using System.Threading.Tasks;

#nullable enable

// All tokens are stateless with the exception of 'Referable' and 'Reference'
// functional (non-functioning) programming B)
namespace Tokens
{
    public interface IResolver { }
    public interface IToken<out T>
    {
        public ITask<T> Resolve(IResolver resolver);
    }
    namespace Int
    {
        public sealed class Constant : IToken<int>
        {
            public readonly int Value;
            public Constant(int value) => Value = value;
            public ITask<int> Resolve(IResolver _) => Task.FromResult(Value).AsITask();

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
            public async ITask<int> Resolve(IResolver resolver)
            {
                Func<int, int, int> function = Operation switch
                {
                    EOperation.Add => (a, b) => a + b,
                    EOperation.Subtract => (a, b) => a - b,
                    EOperation.Multiply => (a, b) => a * b,
                    EOperation.Divide => (a, b) => a / b,
                };
                return function(await Left.Resolve(resolver), await Right.Resolve(resolver));
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
            public async ITask<int> Resolve(IResolver resolver)
            {
                Func<int, int> function = Operation switch
                {
                    EOperation.Negate => x => -x
                };
                return function(await Value.Resolve(resolver));
            }
        }
    }
    namespace Select
    {
        public sealed class One<T> : IToken<T>
        {
            public readonly IToken<IEnumerable<T>> From;
            public One(IToken<IEnumerable<T>> from) => From = from;
            public ITask<T> Resolve(IResolver resolver)
            {
                throw new System.NotImplementedException();
            }
        }
        public sealed class Multiple<T> : IToken<IEnumerable<T>>
        {
            public readonly IToken<IEnumerable<T>> From;
            public Multiple(IToken<IEnumerable<T>> from) => From = from;
            public ITask<IEnumerable<T>> Resolve(IResolver resolver)
            {
                throw new System.NotImplementedException();
            }
        }
    }
    namespace Merge
    {
        public sealed class Collect<T> : IToken<IEnumerable<T>>
        {
            public readonly IEnumerable<IToken<T>> Elements;
            public Collect(IEnumerable<IToken<T>> elements) => Elements = elements;
            public async ITask<IEnumerable<T>> Resolve(IResolver resolver)
            {
                List<T> o = new();
                foreach (var element in Elements) o.Add(await element.Resolve(resolver));
                return o;
            }
        }
        public sealed class Union<T> : IToken<IEnumerable<T>>
        {
            public readonly IEnumerable<IToken<IEnumerable<T>>> Elements;
            public Union(IEnumerable<IToken<IEnumerable<T>>> elements) => Elements = elements;
            public async ITask<IEnumerable<T>> Resolve(IResolver resolver)
            {
                List<T> o = new();
                foreach (var element in Elements) o.AddRange(await element.Resolve(resolver));
                return o;
            }
        }
    }
    // our special friends
    public sealed class Referable<T> : IToken<T>
    {
        public readonly IToken<T> Value;
        public readonly string Label;
        private Option<T> _resolution;
        public Referable(IToken<T> value, string label)
        {
            Label = label;
            Value = value;
            _resolution = new Option<T>.None();
        }
        public async ITask<T> Resolve(IResolver resolver)
        {
            _resolution = _resolution switch
            {
                Option<T>.Some v => v,
                _ => new Option<T>.Some(await Value.Resolve(resolver))
            };
            return _resolution.Unwrap();
        }
    }
    // i bet a foreach token is possible, but lets not for now yea.
    public sealed class Reference<T> : IToken<T>
    {
        public readonly Referable<T> RefersTo;
        public Reference(Referable<T> refersTo) => RefersTo = refersTo;
        public ITask<T> Resolve(IResolver resolver) => RefersTo.Resolve(resolver);
    }
    public sealed class Map<TIn, TOut> : IToken<IEnumerable<TOut>>
    {
        public readonly IToken<IEnumerable<TIn>> Over;
        public readonly Func<IToken<TIn>, IToken<TOut>> MapFunction;
        public Map(IToken<IEnumerable<TIn>> over, Func<IToken<TIn>, IToken<TOut>> mapFunction)
        {
            Over = over;
            MapFunction = mapFunction;
        }
        public async ITask<IEnumerable<TOut>> Resolve(IResolver resolver)
        {
            //add back context and packets guys, we gotta go bald.
        }

    }
    
}