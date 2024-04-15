
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
using Proxy.Creator;
using Perfection;
using Tokens;
using Tokens.Number;
public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        //this beats herion
        var token_3 = new Subtract(new Constant(10), new Constant(44));
        var rule_1 = Rule.Create.For<Subtract, Res.Number>(P =>
        {
            return P.SubEnvironment(P.OriginalArg1().AsVariable("y")) with
            {
                SubTokenProxy = P.TokenFunction<Add>()
                  .WithArgs(P.AsIs(new Reference<Res.Number>("y")), P.AsIs(new Reference<Res.Number>("y")))
            };
        });
        var context = new Context()
        {
            InputProvider = null,
            State = null,
            Rules = new() { rule_1 },
            Scope = new() { }
        };
        Debug.Log(await token_3.ResolveWithRules(context));
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