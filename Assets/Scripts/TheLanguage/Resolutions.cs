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
    public sealed record Multi<R> : Operation where R : ResObj
    {
        public PList<R> Elements { get; init; }
        public Updater<PList<R>> dElements { init => Elements = value(Elements); }
        protected override Context UpdateContext(Context before) => Elements.AccumulateInto(before, (p, x) => p.WithResolution(x));
    }
    public sealed record DeclareVariable : Operation
    {
        public string Label { get; init; }
        public ResObj Object { get; init; }
        protected override Context UpdateContext(Context before) => before with
        {
            dVariables = p => new(p.Modulo, p.Also((Label, Object).Yield()))
        };
    }
    public sealed record Unit : NoOp
    {
        public readonly int UUID;
        public int HP { get; init; }
        public Coordinates Position { get; init; }
        public Unit(int id)
        {
            UUID = id;
        }
        public bool Equals(Unit? other) => other is not null && UUID == other.UUID;
        public override int GetHashCode() => UUID;
    }
    public sealed record Coordinates : NoOp
    {
        public int G { get; init; }
        public int U { get; init; }
        public int D { get; init; }
        public int this[int i] => i switch
        {
            0 => G, 1 => U, 2 => D,
            _ => throw new System.IndexOutOfRangeException("Attempted to index Coordinates out of 0..2 range.")
        };
    }
}