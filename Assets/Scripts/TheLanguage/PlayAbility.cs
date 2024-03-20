using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Token;
namespace GameActions
{
    namespace PlayAbility
    {
        public class Expression<C> : IExpression<Packet, C> where C : Context.IContextData
        {
            public IToken<Unit, C> Source;

            public Packet Resolve(C context)
            {
                throw new NotImplementedException();
            }
        }
        public class Packet : IPacket
        {
               
        }
        public class Evaluation : IEvaluation
        {
            private Ability _ability;

            public void Forward(GameState game)
            {

            }
        }
    }
}