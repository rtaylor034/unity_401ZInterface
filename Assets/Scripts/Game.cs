using System;
using System.Collections.Generic;
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

    public delegate void PathingFunction(Unit mover, Player player, PathingState state, Hex from, Hex to);
    //classes like these act as typed enums.
    public interface EPathingSpecification
    {
        public class Standard : EPathingSpecification { }
        public class IgnoreWall : EPathingSpecification { }
    }

    public interface ETargetingSpecification
    {

    }
    public interface EConsequenceSpecification
    {
        public class Passive : EConsequenceSpecification
        {
            public List<Func<bool>> PassiveSpecific;
        }
    }
}
