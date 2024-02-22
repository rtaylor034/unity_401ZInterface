using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
public class Game
{
    private GameWorld _world;

    // in settings
    // -- kinda just have to use a switch statement for this.
    // private Dictionary<EMoveType, PathfindingFunction> _pathfindingImplementations;

    private struct PathfindState
    {
        //probably contains an int with the amount of steps required/left.
    }

    private delegate PathfindState PathfindingFunction(PathfindState currentState, Hex from, Hex to);
    public delegate IEnumerable<UnevaluatedAction> ResultantAdder(GameAction action);
    public delegate void ActionModifier(UnevaluatedAction action);

    private List<ResultantAdder> _evaluationAdders;
    private List<ActionModifier> _requestModifiers; 

    public GuardedCollectionHandle<ResultantAdder> OnActionEvaluate { get; private set; }
    public GuardedCollectionHandle<ActionModifier> BeforeActionEvaluate { get; private set; }

    public Game()
    {
        _evaluationAdders = new();
        _requestModifiers = new();
        OnActionEvaluate = new(_evaluationAdders);
        BeforeActionEvaluate = new(_requestModifiers);

    }


    //classes like these act as typed enums.
    public abstract class MoveRestriction
    {
        public Unit Mover;

        public class Standard { }
        public class IgnoreWall { }
    }

}
 