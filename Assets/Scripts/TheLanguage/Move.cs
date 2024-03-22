using System.Collections.Generic;
using UnityEngine;
using Token;
using EvaluationProtocol;

// would be so insanely insane with rust macros
// me when expressions are literally no different than tokens.
namespace GameActions
{
    namespace Move
    {
        public class Expression<C> : IExpression<Packet, C> where C : Context.IContextData
        {
            public IToken<IEnumerable<Unit>, C> MovableUnits;
            public IToken<Unit, C> Test;
            public IToken<int, C> TestInt;

            public IProtocol<Packet> Resolve(C context)
            {
                return new Static<Packet>(this, new()
                {
                    MovableUnits = MovableUnits.Resolve(context),
                });
            }
        }
        public class Packet : IPacket
        {
            public IProtocol<IEnumerable<Unit>> MovableUnits;
            public (int Min, int Max) Total;
            public (int Min, int Max) PerUnit;
            public List<string> PathingRules;
        }
        public class Evaluation : IEvaluation
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