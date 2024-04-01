using System.Collections;
using System.Collections.Generic;
using Expressions.Reference;
using MorseCode.ITask;
using UnityEngine;

public class GameState
{
    private List<Unit> _units;
    public IEnumerable<Unit> Units => _units;

}
