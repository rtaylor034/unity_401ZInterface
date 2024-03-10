using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable
public abstract class GameAction
{
    public abstract void Forward(GameState state);
    public abstract void Backward(GameState state);

    //use when unevaluated GameAction is cancelled.
    public class None : GameAction
    {
        public override void Forward(GameState _) { }
        public override void Backward(GameState _) { }
    }
}
