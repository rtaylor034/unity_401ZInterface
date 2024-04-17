using System.Collections;
using System.Collections.Generic;
using MorseCode.ITask;
using UnityEngine;
using Perfection;
using Res = Resolutions;

public record GameState
{
    public readonly PIndexedSet<int, Res.Unit> Units;
    public readonly PIndexedSet<Res.Coordinates, Res.Hex> Hexes;
    public Updater<PIndexedSet<int, Res.Unit>> dUnits { init => Units = value(Units); }
    public Updater<PIndexedSet<Res.Coordinates, Res.Hex>> dHexes { init => Hexes = value(Hexes); }

    public GameState()
    {
        Units = new(unit => unit.UUID, 13);
        Hexes = new(hex => hex.Position, 133);
    }
}
