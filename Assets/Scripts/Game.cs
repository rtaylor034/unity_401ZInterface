using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable disable
public class Game
{
    private GameWorld _world;
    private GameSettings _settings;
    private GameState _state;
    //used by Token.GameData to grab gamedata
    public GameState CurrentState => _state;

    //-- CLASS LOCATION TO BE DETERMINED --
    
    public HashSet<Hex> DoPathing(Unit mover, Player player, IEnumerable<EPathingSpecification> specifications)
    {
        List<Specifications.PathingFunction> pathingFunctions = new();
        foreach (var spec in specifications) pathingFunctions.Add(_settings.PathingImplementations(spec));
        throw new NotImplementedException();
    }

    
}
