using Resolution;
using Perfection;
using ResObj = Resolution.Resolution;

using System.Collections.Generic;
using Token;

namespace Resolutions
{
    public sealed record Number : NonMutating
    {
        public int Value { get; init; }
    }
    public sealed record Multi<R> : ResObj where R : IResolution
    {
        private List<R> _elements { get; init; }
        public IEnumerable<R> Values { get => _elements; init { _elements = new(value); } }
        public override Context ChangeContext(Context before)
        {
            return _elements.AccumulateInto(before, (p, x) => p.WithResolution(x));
        }
    }
}