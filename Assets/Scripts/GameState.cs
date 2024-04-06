using System.Collections;
using System.Collections.Generic;
using MorseCode.ITask;
using UnityEngine;

public record GameState
{
    private List<Unit> _units;
    public IEnumerable<Unit> Units => _units;

}
