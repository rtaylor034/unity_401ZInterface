using Perfection;
using Resolution;
namespace Resolution.Board
{
    public interface IPositioned : IResolution
    {
        public Resolutions.Board.Coordinates Position { get; }
    }
}
namespace Resolutions.Board
{
    using Resolution.Board;
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
        public Coordinates Add(Coordinates b) => new()
        {
            R = R + b.R,
            U = U + b.U,
            D = D + b.D,
        };
        public Coordinates ScalarManipulate(System.Func<int, int> scalarFunction) => new()
        {
            R = scalarFunction(R),
            U = scalarFunction(U),
            D = scalarFunction(D)
        };
    }

    public abstract record Hex : NoOp, IPositioned
    {
        public Coordinates Position { get; init; }
        public Updater<Coordinates> dPosition { init => Position = value(Position); }
        public override bool ResEqual(IResolution other)
        {
            return (other is Hex h && Position.ResEqual(h.Position));
        }
    }

    public sealed record Unit : NoOp, IPositioned
    {
        public readonly int UUID;
        public Number HP { get; init; }
        public Updater<Number> dHP { init => HP = value(HP); }
        public Coordinates Position { get; init; }
        public Updater<Coordinates> dPosition { init => Position = value(Position); }
        public Multi<Effect> Effects { get; init; } 
        public Updater<Multi<Effect>> dEffects { init => Effects = value(Effects); }
        public Unit(int id)
        {
            UUID = id;
        }
        public override bool ResEqual(IResolution other)
        {
            return (other is Unit u && UUID == u.UUID);
        }
        public sealed record Effect : NoOp
        {
            public readonly string Identifier;
        }
    }
}