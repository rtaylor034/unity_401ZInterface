using System;
using MorseCode.ITask;
using System.Collections.Generic;
using UnityEngine;
using Token;
namespace GameActions
{
    namespace PlayAbility
    {
        public class Token<C> : IToken<Resolution, C> where C : Context.IContextData
        {
            public IToken<Unit, C> Source;

            public ResolutionProtocol.IProtocol<Resolution> Evaluate(C context)
            {
                throw new NotImplementedException();
            }
        }
        public class Packet : ResolutionProtocol.TokenSourced<Resolution>
        {
            public Packet(IDisplayable source) : base(source) { }
            public override ITask<Resolution> Resolve(GameWorld resolver)
            {
                throw new System.NotImplementedException();
            }
        }
        public class Resolution : IResolution
        {
            private Ability _ability;

            public void Forward(GameState game)
            {

            }
        }
    }
}