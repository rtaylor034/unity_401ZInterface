using System.Collections;
using System.Collections.Generic;
using MorseCode.ITask;
using UnityEngine;
using Perfection;
using r = Resolutions;
using ResObj = Resolution.IResolution;

public record GameState
{
    public PMap<string, ResObj> Variables { get; init; }
    public Updater<PMap<string, ResObj>> dVariables { init => Variables = value(Variables); }
    public PList<Rule.IRule> Rules { get; init; }
    public Updater<PList<Rule.IRule>> dRules { init => Rules = value(Rules); }
    public BoardState Board { get; init; } 
    public Updater<BoardState> dBoard { init => Board = value(Board); }
    
    public record BoardState
    {
        public readonly PIndexedSet<int, r.Unit> Units;
        public readonly PIndexedSet<r.Coordinates, r.Hex> Hexes;
        public Updater<PIndexedSet<int, r.Unit>> dUnits { init => Units = value(Units); }
        public Updater<PIndexedSet<r.Coordinates, r.Hex>> dHexes { init => Hexes = value(Hexes); }
        public BoardState()
        {
            Units = new(unit => unit.UUID, 13);
            Hexes = new(hex => hex.Position, 133);
        }
    }
    public GameState WithResolution(ResObj resolution) { return resolution.ChangeState(this); }
}
