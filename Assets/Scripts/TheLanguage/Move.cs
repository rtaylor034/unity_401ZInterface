using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Context.Token;

//would be so insanely insane with macros or rust guys.
namespace GameActions
{
    namespace Move
    {
        public class Expression<C> : IExpression<Packet, C> where C : Context.IContextData
        {
            public IToken<List<Unit>, C> MovableUnits;
  ;          public IToken<Unit, C> Test;
            public IToken<int, C> TestInt;

            public Packet Resolve(C context)
            {
                return new()
                {
                    MovableUnits = MovableUnits.Resolve(context),
                };
            }
        }
        public class Packet : IPacket
        {
            public List<Unit> MovableUnits;
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