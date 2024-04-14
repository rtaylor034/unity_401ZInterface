using Resolution;
using Perfection;
using ResObj = Resolution.IResolution;
using BaseObject = Resolution.Resolution;

using System.Collections.Generic;
using Token;

namespace Resolutions
{
    public sealed record Number : NonMutating
    {
        public int Value { get; init; }
    }
    public sealed record Multi<R> : BaseObject where R : ResObj
    {
        private List<R> _elements { get; init; }
        public IEnumerable<R> Values { get => _elements; init { _elements = new(value); } }
        public override Context ChangeContext(Context before) => _elements.AccumulateInto(before, (p, x) => p.WithResolution(x));
    }
    public sealed record Referable : BaseObject
    {
        public string Label { get; init; }
        public ResObj Object { get; init; }
        public override Context ChangeContext(Context before) => before with
        {
            Scope = before.Scope with
            {
                Variables = before.Scope.Variables.Also(KeyValuePair.Create(Label, Object).Yield())
            }
        };
    }
}