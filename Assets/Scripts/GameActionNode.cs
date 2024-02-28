using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GameActionSequence
{
    public int LeftForward => _resultants.Count - _pointer;
    public int LeftBackward => _pointer + 1;
    private GameAction _head;
    private List<GameActionSequence> _resultants;
    private int _pointer = -1;

    public int DoForward(int count, GameState state)
    {

    }
}
