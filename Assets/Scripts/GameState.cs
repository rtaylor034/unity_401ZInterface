using System.Collections;
using System.Collections.Generic;
using MorseCode.ITask;
using UnityEngine;
using Perfection;
using Resolutions;

public record GameState
{
    public PIndexedSet<int, Unit> Units { get; init; }
    public Updater<PIndexedSet<int, Unit>> dUnits { init => Units = value(Units); }
    
}
