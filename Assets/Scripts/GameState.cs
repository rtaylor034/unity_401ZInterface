using System.Collections;
using System.Collections.Generic;
using MorseCode.ITask;
using UnityEngine;
using Perfection;
using Resolutions;

public record GameState
{
    public PSet<Unit> Units { get; init; }
}
