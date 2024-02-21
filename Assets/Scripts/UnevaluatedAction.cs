
using System;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable
public abstract class UnevaluatedAction
{
    public bool CanBeNone { get; private set; }
    public abstract Task<GameAction> TryEvaluate(GameWorld world);
}
