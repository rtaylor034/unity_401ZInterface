using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

//would be really cool if system was immutable and pre/post gamestates were stored so Perform and Undo could be inferred, but we must make compromises (i.e. skill issue).
#nullable enable
public abstract class GameAction
{
    private GameAction? _parent;
    public UnevaluatedAction EvaluatedFrom;
    private List<GameAction> _resultants;
    public int Depth { get; private set; }
    
    public GameAction(UnevaluatedAction evaluatedFrom, GameAction parent)
    {
        _parent = parent;
        EvaluatedFrom = evaluatedFrom;
        _resultants = new();
        Depth = _parent is null ? 0 : _parent.Depth + 1;
    }
    public abstract void Perform(Game game);
    public abstract void Undo(Game game);

    //use when unevaluated GameAction is cancelled.
    public class None : GameAction
    {
        public None(UnevaluatedAction evaluatedFrom, GameAction parent) : base(evaluatedFrom, parent) { }

        public override void Perform(Game _) { }
        
        public override void Undo(Game _) { }
    }
}
