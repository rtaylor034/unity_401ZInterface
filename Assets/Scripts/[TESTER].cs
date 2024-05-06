
using System;
using System.Collections;
using System.Collections.Generic;
using Token;
using TokenSyntax;
using Proxy;
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using r = Resolutions;
using t = Tokens;
using Proxy.Creator;
using Perfection;
using Tokens.Number;
using Token.Creator;
using FourZeroOne;
public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        o.tBoolTo<r.Number>(new()
        {
            True = 5.tFixed(),
            False = 8.tFixed()
        });
        var program = new FourZeroOne.Programs.Standard.Program()
        {
            State = new()
            {
                Rules = new() { Elements = Iter.Over<Rule.IRule>() },
                Variables = new(7),
                Board = new() { }
            }
        };
        while ((await new t.Select.One<r.Bool>(new t.Multi.Union<r.Bool>(Iter.Over(true, false).Map(x => new t.Multi.Yield<r.Bool>(new t.Fixed<r.Bool>(x))))).Resolve(program)).Unwrap().IsTrue)
        {
            Debug.Log("===============================");
            Debug.Log(await token2.ResolveWithRules(program));
            Debug.Log("===============================");
        }
        UnityEditor.EditorApplication.ExitPlaymode();
        In<AClass> a = null;
        In<BClass> b = null;
        b = a;
        MakeToken.tRecursive(new()
        {
            A = 5.tFixed()
        }, ))
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