
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using Evaluator = GameWorld;
using System.Collections.Generic;
using System;
using Token;

namespace ResolutionProtocol
{
    
    public interface IProtocol<out T>
    {
        public ITask<T> Resolve(Evaluator evaluator);
        public IDisplayable DisplaySource { get; }
    }
    public abstract class TokenSourced<T> : IProtocol<T>
    {
        public IDisplayable DisplaySource { get; private set; }
        public abstract ITask<T> Resolve(Evaluator evaluator);
        public TokenSourced(IDisplayable source) => DisplaySource = source;
    }
    public class Static<T> : TokenSourced<T>
    {
        public readonly T Value;
        public Static(IDisplayable source, T value) : base(source) => Value = value;
        public override ITask<T> Resolve(Evaluator _) => Task.FromResult(Value).AsITask();
    }
    public class Combine<TIn1, TIn2, TOut> : TokenSourced<TOut>
    {
        public readonly IProtocol<TIn1> A;
        public readonly IProtocol<TIn2> B;
        public readonly Func<TIn1, TIn2, TOut> Function;
        public Combine(IDisplayable source, IProtocol<TIn1> a, IProtocol<TIn2> b, Func<TIn1, TIn2, TOut> function) : base(source)
        {
            A = a;
            B = b;
            Function = function;
        }
        public override async ITask<TOut> Resolve(Evaluator evaluator) => Function(await A.Resolve(evaluator), await B.Resolve(evaluator));
    }
    public class Transform<T> : TokenSourced<T>
    {
        public readonly IProtocol<T> Value;
        public readonly Func<T, T> Function;
        public Transform(IDisplayable source, IProtocol<T> value, Func<T, T> function) : base(source)
        {
            Value = value;
            Function = function;
        }
        public override async ITask<T> Resolve(Evaluator evaluator) => Function(await Value.Resolve(evaluator));
    }
    namespace Select
    {
        //may split into multiple enums, will probably use a switch statement for now.
        public class One<T> : TokenSourced<T>
        {
            public readonly IEnumerable<T> From;
            public One(IDisplayable source, IEnumerable<T> from) : base(source) { From = from; }
            public override ITask<T> Resolve(Evaluator evaluator)
            {
                throw new System.NotImplementedException();
            }
        }
        public class Multiple<T> : TokenSourced<T>
        {
            public readonly IEnumerable<T> From;
            public Multiple(IDisplayable source, IEnumerable<T> from) : base(source) { From = from; }
            public override ITask<T> Resolve(Evaluator evaluator)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
