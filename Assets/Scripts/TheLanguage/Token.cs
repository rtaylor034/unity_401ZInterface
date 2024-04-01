using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MorseCode.ITask;
using System.Threading.Tasks;
namespace Token
{
    // GameWorld should really be the only IResolver.
    // I just like making interfaces because im tarted.
    public interface IResolver { }
    public interface IToken<out T>
    {
        public ITask<T> Resolve(ResolutionContext context);
    }
    public struct ResolutionContext
    {
        public IResolver Resolver { get; set; }
        public Expressions.Reference.IProvider Scope { get; set; }
    }
    public abstract class Reference<T> : IToken<T>
    {
        public Expressions.Reference.Identifier.IIdentifier Identifier;
        protected Reference(Expressions.Reference.Identifier.IIdentifier identifier) => Identifier = identifier;
        public async ITask<T> Resolve(ResolutionContext context)
        {
            // assumes you DONT mess up the type.
            // this make me want to commit suicide but i cannot be bothered to find a better way.
            return (T)(await context.Scope.GetReference(Identifier).Resolve(context));
        }
    }
    public abstract class DataToken<T> : IToken<T>
    {
        public ITask<T> Resolve(ResolutionContext context) => Task.FromResult(GetData(Game.CurrentGame.CurrentState)).AsITask();
        protected abstract T GetData(GameState state);
    }
}
namespace Tokens
{
    using Token;
    public sealed class DynamicReference<T> : Reference<T>
    {
        public DynamicReference(string key) : base(new Expressions.Reference.Identifier.Dynamic(key)) { }
    }
    
    // special tokens that are especially silly.
    namespace GameData
    {
        public sealed class AllUnits : DataToken<IEnumerable<Unit>>
        {
            protected override IEnumerable<Unit> GetData(GameState state) => state.Units;
        }
    }
    namespace Gen
    {
        namespace Select
        {
            public sealed class One<T> : IToken<T>
            {
                public IToken<IEnumerable<T>> From;
                public One(IToken<IEnumerable<T>> from)
                {
                    From = from;
                }
                public ITask<T> Resolve(ResolutionContext context)
                {
                    throw new System.NotImplementedException();
                }
            }
            public sealed class Multiple<T> : IToken<IEnumerable<T>>
            {
                public IToken<IEnumerable<T>> From;
                // simple 'count' based system for now.
                // would be cool if it was condition based. (like select until <x>)
                public IToken<int> Count;
                public Multiple(IToken<IEnumerable<T>> from, IToken<int> count)
                {
                    From = from;
                    Count = count;
                }
                public ITask<IEnumerable<T>> Resolve(ResolutionContext context)
                {
                    throw new System.NotImplementedException();
                }
            }
        }
    }
}