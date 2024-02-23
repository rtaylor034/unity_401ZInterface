using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings
{
    public delegate Game.PathingFunction ImplementPathing(Game.EPathingSpecification pathingSpec);

    public ImplementPathing PathingImplementations;

    public GameSettings (
        ImplementPathing PATHING_IMPLEMENTATIONS

        )
    {
        PathingImplementations = PATHING_IMPLEMENTATIONS;
    }

    //placeholder constant
    public static GameSettings STANDARD = new(
        PATHING_IMPLEMENTATIONS: (pathingSpec) =>
            {
                throw new NotImplementedException();
                switch (pathingSpec)
                {
                    case Game.EPathingSpecification.Standard info:
                        break;
                    case Game.EPathingSpecification.IgnoreWall info:
                        break;
                    default:
                        break;
                }
            }
        );
}
