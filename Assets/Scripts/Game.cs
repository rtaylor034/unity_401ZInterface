using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

#nullable disable
public class Game
{
    private GameWorld _world;
    private GameSettings _settings;
    private GameState _state;

    //-- CLASS LOCATION TO BE DETERMINED --
    
    public HashSet<Hex> DoPathing(Unit mover, Player player, IEnumerable<EPathingSpecification> specifications)
    {
        List<PathingFunction> pathingFunctions = new();
        foreach (var spec in specifications) pathingFunctions.Add(_settings.PathingImplementations(spec));
        throw new NotImplementedException();
    }

    public class PathingState
    {
        public int Cost;
        public bool Blocked;
    }

    public interface SETTINGS_IMPLEMENTED { }
    public delegate void PathingFunction(Unit mover, Player player, PathingState state, Hex from, Hex to);
    //classes like these act as typed enums.
    public interface EPathingSpecification : SETTINGS_IMPLEMENTED
    {
        public class Standard : EPathingSpecification { }
        public class IgnoreWall : EPathingSpecification { }
    }

    public interface ETargetingSpecification : SETTINGS_IMPLEMENTED
    {

    }
    public interface EConsequenceSpecification : SETTINGS_IMPLEMENTED
    {
        public class Passive : EConsequenceSpecification
        {
            public List<ConsequenceRule> PassiveSpecific;
        }
        public class Slippery : EConsequenceSpecification { }
    }
    public class ConsequenceRule
    {


    }
}
public interface ICondition<T>
{
    public abstract bool Evaluate(T input);
}
public class CombinedCondition<T>
{
    public enum ECombiner
    {
        And,
        Or
    }
    private List<CombinedCondition<T>> _conditions;
    private List<ECombiner> _combiners;
    public List<CombinedCondition<T>> Conditions => new(_conditions);
    public List<ECombiner> Combiners => new(_combiners);

    public CombinedCondition<T> Combine(ECombiner combiner, CombinedCondition<T> condition)
    {
        _conditions.Add(condition);
        _combiners.Add(combiner);
        return this;
    }
    public CombinedCondition<T> Combine(ECombiner combiner, ICondition<T> condition)
    {
        _conditions.Add(new IdentityCondition(condition));
        _combiners.Add(combiner);
        return this;
    }
    public static CombinedCondition<T> Identity(ICondition<T> condition)
    {
        return new()
        {
            _conditions = new() { new IdentityCondition(condition) }
        };
    }
    private CombinedCondition()
    {
        _conditions = new();
        _combiners = new();
    }
    public virtual bool Evaluate(T input)
    {
        bool o = _conditions[0].Evaluate(input);
        for (int i = 1; i < _conditions.Count; i++)
        {
            switch (_combiners[i-1])
            {
                case ECombiner.And:
                    o = (o && _conditions[i].Evaluate(input));
                    break;
                case ECombiner.Or:
                    o = (o || _conditions[i].Evaluate(input));
                    break;
            }
        }
        return o;
    }
    private class IdentityCondition : CombinedCondition<T>
    {
        public ICondition<T> Value { get; private set; }
        public IdentityCondition(ICondition<T> identity) { Value = identity; }
        public override bool Evaluate(T input) => Value.Evaluate(input);
    }
}
