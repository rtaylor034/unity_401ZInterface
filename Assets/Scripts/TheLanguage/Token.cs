using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Context;

namespace Token
{
    public interface IToken<out TResolvesTo, in TMinContextData> : IDisplayable where TMinContextData : IContextData
    {
        public Packet.IPacket<TResolvesTo> Evaluate(TMinContextData context);
    }
    public interface IDisplayable { }
}