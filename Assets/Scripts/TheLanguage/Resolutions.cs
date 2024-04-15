using Resolution;
using Perfection;
using ResObj = Resolution.IResolution;
using System.Collections.Generic;
using Token;

namespace Resolutions
{
    public sealed record Number : NoOp
    {
        public int Value { get; init; }
    }
    public sealed record Multi<R> : Operation where R : ResObj
    {
        private List<R> _elements { get; init; }
        public IEnumerable<R> Values { get => _elements; init { _elements = new(value); } }
        protected override Context UpdateContext(Context before) => _elements.AccumulateInto(before, (p, x) => p.WithResolution(x));
    }
    public sealed record DeclareVariable : Operation
    {
        public string Label { get; init; }
        public ResObj Object { get; init; }
        protected override Context UpdateContext(Context before) => before with
        {
            Scope = before.Scope with
            {
                Variables = before.Scope.Variables.Also(KeyValuePair.Create(Label, Object).Yield())
            }
        };
    }
    public sealed record Unit : NoOp
    {

    }
}