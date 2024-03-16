using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

//im going to blow my brains out
namespace HolyFuck
{
    public class GameContext
    {

    }
    public interface IToken<T> { }
    public interface EActionStatement<out C, out U> where C : GameContext where U : ActionPacket
    {
        public U Resolve();
    }

    public class MoveActionStatement<C> : EActionStatement<C, ActionPacket.Move> where C : GameContext
    {
        public IToken<List<Unit>> MovingUnits;
        public IToken<int> Spaces;

        public ActionPacket.Move Resolve()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AbilityContext : GameContext
    {

    }
    public class Ability
    {
        public List<EActionStatement<AbilityContext, ActionPacket>> OnUse => new()
        {
            new MoveActionStatement<AbilityContext>(),
        };


    }
}