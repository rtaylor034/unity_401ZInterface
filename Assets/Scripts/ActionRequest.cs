
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
public abstract class ActionRequest
{
    public bool CanBeNone { get; private set; }
    public abstract Task<GameAction> Make(GameWorld world);
}
