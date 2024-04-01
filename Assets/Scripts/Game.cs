using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable disable
public class Game
{
    //These 2 exist becuase i cant find a better way for Token.DataTokens to access gamedata.
    //A bit suicidal this singleton makes me.
    public static Game CurrentGame { get; private set; }
    public GameState CurrentState => _state;

    private GameWorld _world;
    private GameSettings _settings;
    private GameState _state;
    
    
}
