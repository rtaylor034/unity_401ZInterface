
using System;
using System.Collections;
using System.Collections.Generic;
using Token;
using Proxy;
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using GStructures;
using Res = Resolutions;
using Rule.Creator;
using Perfection;
public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        foreach (var e in (0, 1).GenerateSequence(x => (x.Item2, x.Item1 + x.Item2)).Map(x => x.Item2).Until(x => x > 100))
        {
            Debug.Log(e);
        }
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

public record ValT1 : Resolution.NonMutating { }
public record ValT2 : Resolution.NonMutating { }
public record TokenFunc : Token.PureFunction<ValT1, ValT2, ValT2>
{
    protected override ValT2 EvaluatePure(ValT1 in1, ValT2 in2)
    {
        throw new NotImplementedException();
    }
    public TokenFunc(Token<ValT1> in1, Token<ValT2> in2) : base(in1, in2) { }
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