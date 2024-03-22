
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using Resolver = GameWorld;
using System.Collections.Generic;
using System;
using Token;
using GStructures;

#nullable enable
namespace ResolutionProtocol
{
    public interface IProtocol<out T>
    {
        public ITask<T> Resolve(Resolver resolver);
        public IDisplayable DisplaySource { get; }
    }
    public abstract class TokenSourced<T> : IProtocol<T>
    {
        public IDisplayable DisplaySource { get; private set; }
        public abstract ITask<T> Resolve(Resolver resolver);
        public TokenSourced(IDisplayable source) => DisplaySource = source;
    }
    public class Static<T> : TokenSourced<T>
    {
        public readonly T Value;
        public Static(IDisplayable source, T value) : base(source) => Value = value;
        public override ITask<T> Resolve(Resolver _) => Task.FromResult(Value).AsITask();
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
        public override async ITask<TOut> Resolve(Resolver resolver) => Function(await A.Resolve(resolver), await B.Resolve(resolver));
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
        public override async ITask<T> Resolve(Resolver resolver) => Function(await Value.Resolve(resolver));
    }
    namespace Select
    {
        public class One<T> : TokenSourced<T>
        {
            public readonly IProtocol<IEnumerable<T>> From;
            public One(IDisplayable source, IProtocol<IEnumerable<T>> from) : base(source) { From = from; }
            public override ITask<T> Resolve(Resolver resolver)
            {
                throw new System.NotImplementedException();
            }
        }
        public class Multiple<T> : TokenSourced<IEnumerable<T>>
        {
            public readonly IProtocol<IEnumerable<T>> From;
            public Multiple(IDisplayable source, IProtocol<IEnumerable<T>> from) : base(source) { From = from; }
            public override ITask<IEnumerable<T>> Resolve(Resolver resolver)
            {
                throw new System.NotImplementedException();
            }
        }
    }

    // our special friends
    public class Referable<T> : TokenSourced<T> 
    {
        public readonly IProtocol<T> Value;
        private Option<T> _resolution;
        public Referable(Context.Any.Tokens.Referable<T> source, IProtocol<T> value) : base(source)
        {
            Value = value;
            _resolution = new Option<T>.None();
        }
        public override async ITask<T> Resolve(Resolver resolver)
        {
            _resolution = _resolution switch
            {
                Option<T>.Some v => v,
                _ => new Option<T>.Some(await Value.Resolve(resolver)),
            };
            return _resolution.Unwrap();
        }
    }
    public sealed class Reference<T> : TokenSourced<T>
    {
        public readonly Referable<T> RefersTo;
        public Reference(Context.Any.Tokens.Reference<T> source, Referable<T> refersTo) : base(source) => RefersTo = refersTo;
        public override ITask<T> Resolve(Resolver resolver) => RefersTo.Resolve(resolver);
    }
}
