using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MorseCode.ITask;
using System.Threading.Tasks;

namespace Token
{
    public interface IToken<out T>
    {
        public ITask<T> Resolve(Expressions.References.IProvider scope);
    }
    public abstract class GameDataToken<T> : IToken<T>
    {
        public abstract T GetValue(GameState state);
        public ITask<T> Resolve(Expressions.References.IProvider _) => GetValue()
    }
    //special tokens that are especially silly.
    namespace GameData
    {
        
        public sealed class AllUnits : IToken<IEnumerable<Unit>>
        {

        }
    }
}