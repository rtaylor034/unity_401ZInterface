using System.Collections;
using System.Collections.Generic;
using MorseCode.ITask;
using UnityEngine;
using Perfection;
using Res = Resolutions;

public record GameState
{
    public readonly PIndexedSet<int, Res.Unit> Units;
    public readonly PIndexedSet<Res.Coordinates, Res.Hex.Hex> Hexes;
    public Updater<PIndexedSet<int, Res.Unit>> dUnits { init => Units = value(Units); }
    public Updater<PIndexedSet<Res.Coordinates, Res.Hex.Hex>> dHexes { init => Hexes = value(Hexes); }


    public GameState()
    {
        Units = new(13, x => x.UUID);
        Hexes = new(77, x => x.Position);
    }
}
