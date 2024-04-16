using System.Collections;
using System.Collections.Generic;
using MorseCode.ITask;
using UnityEngine;
using Perfection;
using Res = Resolutions;

public record GameState
{
    public PIndexedSet<int, Res.Unit> Units { get; init; }
    public Updater<PIndexedSet<int, Res.Unit>> dUnits { init => Units = value(Units); }
    public PIndexedSet<Res.Coordinates, Res.Hex.Hex> Hexes { get; init; } 
    public Updater<PIndexedSet<Res.Coordinates, Res.Hex.Hex>> dHexes { init => Hexes = value(Hexes); }
}
