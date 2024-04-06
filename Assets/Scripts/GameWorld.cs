using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour, Token.IInputProvider
{
    private Dictionary<Unit, Physical.Unit> _unitMap;
}
