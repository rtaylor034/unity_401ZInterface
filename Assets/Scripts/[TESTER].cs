
using System;
using System.Collections;
using System.Collections.Generic;
using Token;
using GameActions;
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using GStructures;

public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        var task = new ControlledTask<int>();
        Perform(task);
        await Print(task);
    }
    async Task Perform(ControlledTask<int> task)
    {
        await Task.Delay(2000);
        task.Resolve(7);
    }
    async Task Print(ITask<int> task)
    {
        int o = await task;
        Debug.Log(o);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

public class AClass
{
    public int A;
    public int B;
}
public class BClass : AClass
{

}
public interface In<in T> { }
public interface Out<out T> { }