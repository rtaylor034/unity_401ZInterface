using Perfection;
using Resolution;
namespace Resolutions
{
    public sealed record Coordinates : NoOp
    {
        public int R { get; init; }
        public int U { get; init; }
        public int D { get; init; }
        public int this[int i] => i switch
        {
            0 => R,
            1 => U,
            2 => D,
            _ => throw new System.IndexOutOfRangeException("Attempted to index Coordinates out of 0..2 range.")
        };
        public override string ToString() => $"({R}.{U}.{D})";
    }
    public abstract record Hex : NoOp
    {
        public Coordinates Position { get; init; }
        public Updater<Coordinates> dPosition { init => Position = value(Position); }
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
}