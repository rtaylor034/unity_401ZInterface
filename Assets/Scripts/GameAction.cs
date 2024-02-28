using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable
public abstract class GameAction
{
    public ActionRequest EvaluatedFrom;
    public GameAction(ActionRequest evaluatedFrom)
    {
        EvaluatedFrom = evaluatedFrom;
    }
    public abstract void Forward(GameState state);
    public abstract void Backward(GameState state);

    //use when unevaluated GameAction is cancelled.
    public class None : GameAction
    {
        public None(ActionRequest evaluatedFrom) : base(evaluatedFrom) { }

        public override void Forward(GameState _) { }
        public override void Backward(GameState _) { }
    }
}
