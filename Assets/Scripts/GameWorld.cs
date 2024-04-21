using System.Collections;
using System.Collections.Generic;
using Token;
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;

#nullable enable
public class GameWorld : MonoBehaviour, Token.IInputProvider
{
    ITask<IEnumerable<R>?> IInputProvider.GetMultiSelection<R>(IEnumerable<R> outOf, int count)
    {
        throw new System.NotImplementedException();
    }

    ITask<R?> IInputProvider.GetSelection<R>(IEnumerable<R> outOf) where R : class
    {
        throw new System.NotImplementedException();
    }
}
