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
               //maybe like a "PacketResponse" type and a special "IReferenceToken<_, _, PacketResponse>"
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