using Resolution;
using Perfection;
using ResObj = Resolution.IResolution;
using System.Collections.Generic;
using Token;
using FourZeroOne;

#nullable enable
namespace Resolutions
{
    public sealed record Number : NoOp
    {
        public int Value { get; init; }
        public Updater<int> dValue { init => Value = value(Value); }
        public static implicit operator Number(int value) => new() { Value = value };
    }
    public sealed record Bool : NoOp
    {
        public bool IsTrue { get; init; }
        public Updater<bool> dIsTrue { init => IsTrue = value(IsTrue); }
        public static implicit operator Bool(bool value) => new() { IsTrue = value };
    }

    public sealed record Multi<R> : Operation, IMulti<R> where R : ResObj
    {
        public IEnumerable<R> Values { get => _list.Elements; init => _list = new() { Elements = value }; }
        public Updater<IEnumerable<R>> dValues { init => Values = value(Values); }
        public IOption<Multi<R>> AsOption()
        {
            return (_list.Count > 0) ? this.AsSome() : new None<Multi<R>>();
        }

        protected override State UpdateState(State state)
        {
            // a *little* fucking retarded that we have to cast here
            // possible codesmell??
            return Values.AccumulateInto(state, (p, x) => p.WithResolution(((ResObj)x).AsSome()));
        }

        private readonly PList<R> _list;
        public override string ToString()
        {
            return $"[Multi<{typeof(R).Name}> : {_list}]";
        }
    }

    public sealed record DeclareVariable : Operation
    {
        public string Label { get; init; }
        public Updater<string> dLabel { init => Label = value(Label); }
        public ResObj Object { get; init; }
        public Updater<ResObj> dObject { init => Object = value(Object); }

        protected override State UpdateState(State state) => state with
        {
            dVariables = Q => Q with { dElements = Q => Q.Also((Label, Object).Yield()) }
        };
    }

    public sealed record DeclareRule : Operation
    {
        public Rule.IRule Rule { get; init; } 
        public Updater<Rule.IRule> dRule { init => Rule = value(Rule); }

        protected override State UpdateState(State state) => state with
        {
            dRules = Q => Q with { dElements = Q => Q.Also(Rule.Yield()) }
        };
    }


}