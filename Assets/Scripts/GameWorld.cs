using System.Collections;
using System.Collections.Generic;
using Token;
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using Program;

#nullable enable
public class GameWorld : MonoBehaviour, Program.IInputProvider, Program.IOutputProvider
{
    ITask<IEnumerable<R>?> IInputProvider.ReadMultiSelection<R>(IEnumerable<R> outOf, int count)
    {
        throw new System.NotImplementedException();
    }

    ITask<R?> IInputProvider.ReadSelection<R>(IEnumerable<R> outOf) where R : class
    {
        throw new System.NotImplementedException();
    }

    ITask IOutputProvider.WriteState(State state)
    {
        return Task.CompletedTask.AsITask();
    }
}
