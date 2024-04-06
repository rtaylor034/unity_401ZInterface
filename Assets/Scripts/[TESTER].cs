
using System;
using System.Collections;
using System.Collections.Generic;
using Token;
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using GStructures;

public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        AClass aa = new AClass();
        AClass ab = new BClass();
        BClass bb = new BClass();
        Debug.Log(aa.Test());
        Debug.Log(ab.Test());
        Debug.Log(bb.Test());
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
    public virtual string TestA() => "A";
    public string Test() => TestA();
}
public class BClass : AClass
{
    public override string TestA() => "B";
}
public interface In<in T> { }
public interface Out<out T> { }