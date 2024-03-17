using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using any = Context.IContext;
namespace GameActions
{
    namespace PlayAbility
    {
        public class Expression<C> : IExpression<Packet, C> where C : any
        {
            public Packet Resolve(GameState game, C context)
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