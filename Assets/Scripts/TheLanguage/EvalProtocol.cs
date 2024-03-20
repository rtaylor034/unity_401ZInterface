
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using Evaluator = GameWorld;
using System.Collections.Generic;
using System;
using System.Threading.Tasks.Sources;

namespace EvaluationProtocol
{
    public interface IProtocol<out T>
    {
        public ITask<T> Evaluate(Evaluator evaluator);
    }
    public class Static<T> : IProtocol<T>
    {
        public readonly T Value;
        public Static(T value) => Value = value;
        public ITask<T> Evaluate(Evaluator _) => Task.FromResult(Value).AsITask();
    }
    public class Combine<TIn1, TIn2, TOut> : IProtocol<TOut>
    {
        public readonly IProtocol<TIn1> A;
        public readonly IProtocol<TIn2> B;
        public readonly Func<TIn1, TIn2, TOut> Function;
        public Combine(IProtocol<TIn1> a, IProtocol<TIn2> b, Func<TIn1, TIn2, TOut> function)
        {
            A = a;
            B = b;
            Function = function;
        }
        public async ITask<TOut> Evaluate(Evaluator evaluator) => Function(await A.Evaluate(evaluator), await B.Evaluate(evaluator));
    }
    public class Transform<T> : IProtocol<T>
    {
        public readonly IProtocol<T> Value;
        public readonly Func<T, T> Function;
        public Transform(IProtocol<T> value, Func<T, T> function)
        {
            Value = value;
            Function = function;
        }
        public async ITask<T> Evaluate(Evaluator evaluator) => Function(await Value.Evaluate(evaluator));
    }
    namespace Select
    {
        //may split into multiple enums, will probably use a switch statement for now.
        public class One<T> : IProtocol<T>
        {
            public readonly IEnumerable<T> From;
            public One(IEnumerable<T> from) { From = from; }
            public ITask<T> Evaluate(Evaluator evaluator)
            {
                throw new System.NotImplementedException();
            }
        }
        public class Multiple<T> : IProtocol<T>
        {
            public readonly IEnumerable<T> From;
            public Multiple(IEnumerable<T> from) { From = from; }
            public ITask<T> Evaluate(Evaluator evaluator)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
