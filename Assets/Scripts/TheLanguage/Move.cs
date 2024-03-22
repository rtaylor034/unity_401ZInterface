using System.Collections.Generic;
using UnityEngine;
using Token;
using ResolutionProtocol;
using MorseCode.ITask;

// would be so insanely insane with rust macros
// me when expressions are literally no different than tokens.
namespace GameActions
{
    namespace Move
    {
        public class Token<C> : IToken<Resolution, C> where C : Context.IContextData
        {
            public IToken<IEnumerable<Unit>, C> MovableUnits;
            public IToken<Unit, C> Test;
            public IToken<int, C> TestInt; 
            public IProtocol<Resolution> Evaluate(C context)
            {
                return new Packet(this)
                {
                    MovableUnits = MovableUnits.Evaluate(context),
                };
            }
        }
        public class Packet : TokenSourced<Resolution>
        {
            public IProtocol<IEnumerable<Unit>> MovableUnits;
            public (int Min, int Max) Total;
            public (int Min, int Max) PerUnit;
            public List<string> PathingRules;

            public Packet(IDisplayable source) : base(source) { }
            public override ITask<Resolution> Resolve(GameWorld resolver)
            {
                throw new System.NotImplementedException();
            }
        }
        public class Resolution : IResolution
        {
            private List<PosChange> _posChanges;
            public struct PosChange
            {
                public Unit Unit;
                public Vector3Int From;
                public Vector3Int To;
                public PosChange(Unit unit, Vector3Int from, Vector3Int to)
                {
                    this.Unit = unit;
                    this.From = from;
                    this.To = to;
                }
            }
        }
    }
}