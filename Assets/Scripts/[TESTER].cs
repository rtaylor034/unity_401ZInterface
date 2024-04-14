
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
        var token = new Subtract(
            new Multiply(
                new Constant(8),
                new Constant(4)),
            new Constant(6));
        var rule = Rule.Create.For<Subtract, Res.Number>(P =>
        {
            return P.TokenFunction<Subtract>().WithArgs(P.OriginalArg1(), P.AsIs(new Constant(8)));
        });
        var rule2 = Rule.Create.For<Multiply, Res.Number>(P =>
        {
            return P.AsIs(new Constant(1));
        });
        Debug.Log(rule);
        Debug.Log(token);
        Debug.Log(await token.ResolveWithRules(new Context { InputProvider = null, Rules = new() { rule }, Scope = null, State = null}));
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