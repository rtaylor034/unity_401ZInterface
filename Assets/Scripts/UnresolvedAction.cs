using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPacket
{
    public class Move : ActionPacket
    {
        public int Spaces;
        public HashSet<Unit> Units;

    }
}
