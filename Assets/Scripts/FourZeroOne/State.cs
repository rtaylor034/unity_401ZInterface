
using Perfection;
using System;


#nullable enable
namespace FourZeroOne
{
    using Token.Unsafe;
    using ResObj = Resolution.IResolution;
    using r = Core.Resolutions;
    using rb = Core.Resolutions.Board;
    public record State
    {
        public PMap<VariableIdentifier, IOption<ResObj>> Variables { get; init; }
        public Updater<PMap<VariableIdentifier, IOption<ResObj>>> dVariables { init => Variables = value(Variables); }
        public PList<Rule.IRule> Rules { get; init; }
        public Updater<PList<Rule.IRule>> dRules { init => Rules = value(Rules); }
        public BoardState Board { get; init; }
        public Updater<BoardState> dBoard { init => Board = value(Board); }
        public State WithResolution(ResObj resolution) { return resolution.ChangeState(this); }
    }
    public record BoardState
    {
        public readonly PIndexedSet<int, rb.Unit> Units;
        public readonly PIndexedSet<rb.Coordinates, rb.Hex> Hexes;
        public Updater<PIndexedSet<int, rb.Unit>> dUnits { init => Units = value(Units); }
        public Updater<PIndexedSet<rb.Coordinates, rb.Hex>> dHexes { init => Hexes = value(Hexes); }
        public BoardState()
        {
            Units = new(unit => unit.UUID, 13);
            Hexes = new(hex => hex.Position, 133);
        }
    }
}