using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
public class Game
{
    private GameWorld _world;

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
}
 