using Resolution;
using Perfection;
using ResObj = Resolution.IResolution;
using System.Collections.Generic;
using Token;

#nullable enable
namespace Resolutions
{
    public sealed record Number : NoOp
    {
        public int Value { get; init; }
        public Updater<int> dValue { init => Value = value(Value); }
    }

    public sealed record Multi<R> : Operation, IMulti<R> where R : ResObj
    {
        public IEnumerable<R> Values { get => _list.Elements; init => _list = new() { Elements = value }; }
        public Updater<IEnumerable<R>> dValues { init => Values = value(Values); }

        protected override Context UpdateContext(Context context) => Values.AccumulateInto(context, (p, x) => p.WithResolution(x));

        private readonly PList<R> _list;
    }

    public sealed record DeclareVariable : Operation
    {
        public string Label { get; init; }
        public Updater<string> dLabel { init => Label = value(Label); }
        public ResObj Object { get; init; }
        public Updater<ResObj> dObject { init => Object = value(Object); }

        protected override Context UpdateContext(Context context) => context with
        {
            dVariables = Q => Q with { dElements = Q => Q.Also((Label, Object).Yield()) }
        };
    }

    public sealed record DeclareRule : Operation
    {
        public Rule.IRule Rule { get; init; } 
        public Updater<Rule.IRule> dRule { init => Rule = value(Rule); }

        protected override Context UpdateContext(Context context) => context with
        {
            dRules = Q => Q with { dElements = Q => Q.Also(Rule.Yield()) }
        };
    }


}