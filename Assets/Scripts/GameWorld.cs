using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour, Token.IResolver
{
    private Dictionary<Unit, Physical.Unit> _unitMap;
}
