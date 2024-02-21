using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

//would be really cool if system was immutable and pre/post gamestates were stored so Perform and Undo could be inferred, but we must make compromises (i.e. skill issue).
public abstract class GameAction
{
    public UnevaluatedAction EvaluatedFrom;

    public abstract void Perform(Game game);
    public abstract void Undo(Game game);
    //use when unevaluated GameAction is cancelled.
    public class None : GameAction
    {
        public override void Perform(Game _) { }
        
        public override void Undo(Game _) { }
    }
}
