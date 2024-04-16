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
        public PList<R> Values { get; init; }
        public Updater<PList<R>> dElements { init => Values = value(Values); }
        protected override Context UpdateContext(Context before) => Values.Elements.AccumulateInto(before, (p, x) => p.WithResolution(x));
        public IEnumerable<R> GetElements() => Values.Elements;
    }
    public sealed record DeclareVariable : Operation
    {
        public string Label { get; init; }
        public Updater<string> dLabel { init => Label = value(Label); }
        public ResObj Object { get; init; }
        public Updater<ResObj> dObject { init => Object = value(Object); }
        protected override Context UpdateContext(Context before) => before with
        {
            dVariables = p => p with { dElements = x => x.Also((Label, Object).Yield()) }
        };
    }
    public sealed record Unit : NoOp
    {
        public readonly int UUID;
        public int HP { get; init; } 
        public Updater<int> dHP { init => HP = value(HP); }
        public Coordinates Position { get; init; }
        public Updater<Coordinates> dPosition { init => Position = value(Position); }
        public Unit(int id)
        {
            UUID = id;
        }
    }
    public sealed record Coordinates : NoOp
    {
        public int R { get; init; }
        public int U { get; init; }
        public int D { get; init; }
        public int this[int i] => i switch
        {
            0 => R, 1 => U, 2 => D,
            _ => throw new System.IndexOutOfRangeException("Attempted to index Coordinates out of 0..2 range.")
        };
    }
    namespace Hex
    {
        public abstract record Hex : NoOp
        {
            public Coordinates Position { get; init; }
            public Updater<Coordinates> dPosition { init => Position = value(Position); }
        }
    }
}