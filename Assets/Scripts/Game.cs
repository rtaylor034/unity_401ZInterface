using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
public class Game
{
    private GameWorld _world;
    private GameSettings _settings;
    

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

    //-- CLASS LOCATION TO BE DETERMINED --
    
    public HashSet<Hex> DoPathing(Unit mover, Player player, IEnumerable<EPathingSpecification> specifications)
    {
        List<PathingFunction> pathingFunctions = new();
        foreach (var spec in specifications) pathingFunctions.Add(_settings.PathingImplementations.Invoke(spec));
        throw new NotImplementedException();
    }

    public class PathingState
    {
        public int Cost;
        public bool Blocked;
    }

    public delegate void PathingFunction(Unit mover, Player player, PathingState state, Hex from, Hex to);
    //classes like these act as typed enums.
    public abstract class EPathingSpecification
    {
        public class Standard : EPathingSpecification { }
        public class IgnoreWall : EPathingSpecification { }
    }

    public abstract class ETargetingSpecification
    {

    }

}
 