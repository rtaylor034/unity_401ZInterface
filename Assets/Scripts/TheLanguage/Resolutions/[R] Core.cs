using Resolution;
using Perfection;
using ResObj = Resolution.IResolution;
using System.Collections.Generic;
using Token;
using FourZeroOne;

#nullable enable
namespace Resolution
{
    public interface IMulti<out R> : ResObj where R : ResObj
    {
        public IEnumerable<R> Values { get; }
    }
}
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
        public override bool ResEqual(ResObj? other)
        {
            if (other is not IMulti<R> othermulti) return false;
            foreach (var (a, b) in Values.ZipLong(othermulti.Values)) if (a is not null && a.ResEqual(b)) return false;
            return true;
        }
        protected override State UpdateState(State state)
        {
            return Values.AccumulateInto(state, (p, x) => p.WithResolution(x));
        }

        private readonly PList<R> _list;
        public override string ToString()
        {
            return $"[Multi<{typeof(R).Name}> : {_list}]";
        }
    }

    public sealed record DeclareVariable<R> : Operation where R : class, ResObj
    {
        public readonly VariableIdentifier<R> Identifier;
        public IOption<R> Object { get; init; }
        public Updater<IOption<R>> dObject { init => Object = value(Object); }
        public DeclareVariable(VariableIdentifier<R> identifier)
        {
            Identifier = identifier;
        }
        protected override State UpdateState(State state) => state with
        {
            dVariables = Q => Q with
            {
                dElements = Q => Q.Also(((Token.Unsafe.VariableIdentifier)Identifier, (IOption<ResObj>)Object).Yield())
            }
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