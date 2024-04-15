using Resolution;
using Perfection;
using ResObj = Resolution.IResolution;
using System.Collections.Generic;
using Token;

namespace Resolutions
{
    public sealed record Number : NoOp
    {
        public int Value { get; private init; }
        public Number(int value) => Value = value;
    }
    public sealed record Multi<R> : Operation where R : ResObj
    {
        public List<R> Elements { get; init; }
        public Updater<IEnumerable<R>> dElements { init => Elements = new(value(Elements)); }
        protected override Context UpdateContext(Context before) => Elements.AccumulateInto(before, (p, x) => p.WithResolution(x));
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
    public sealed record Unit : NoOp, IUnique
    {
        public int UniqueId { get; private init; }
        public int HP { get; init; }
        public Coordinates Position { get; init; }
        public Unit(int id)
        {
            UniqueId = id;
        }
    }
    public sealed record Coordinates : NoOp
    {
        public int G { get; init; }
        public int U { get; init; }
        public int D { get; init; }
        public int this[int i] => i switch
        {
            0 => G, 1 => U, 2 => D,
            _ => throw new System.IndexOutOfRangeException("Attempted to index a Position out of 0..2 range.")
        };
    }
}