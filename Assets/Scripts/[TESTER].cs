
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
using Tokens;
using Tokens.Number;
public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        //this beats herion
        var token = new Multiply(
            new Add(
                new Constant(10),
                new Constant(5)),
            new Constant(2));
        var rule = Rule.Create.For<Add, Res.Number>(P =>
        {
            return P.TokenFunction<Subtract>().WithArgs(P.OriginalArg1(), P.OriginalArg2());
        });
        var rule2 = Rule.Create.For<Subtract, Res.Number>(P =>
        {
            return P.TokenFunction<Multiply>().WithArgs(P.OriginalArg1(), P.AsIs(new Constant(100)));
        });
        Debug.Log(await token.ResolveWithRules(new Context { InputProvider = null, Rules = new() { rule, rule2 }, Scope = null, State = null}));
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