
using System;
using System.Collections;
using System.Collections.Generic;
using Token;
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
        var token1 = new t.SubEnvironment<r.Number>(new t.Fixed<r.Number>(10).AsVariable(out var x), new t.Fixed<r.Number>(5).AsVariable(out var y))
        {
            SubToken = new Add(new t.Reference<r.Number>(x), new t.Reference<r.Number>(y))
        };
        var token2 = new t.IfElse<r.Number>(new t.Number.Compare.GreaterThan(
            new t.Select.One<r.Number>(0.Sequence(x => x + 1).Take(10).Map(x => new t.Fixed<r.Number>(x).YieldToken()).UnionToken()),
            new t.Fixed<r.Number>(5)))
        {
            Pass = new t.Fixed<r.Number>(100),
            Fail = new t.Fixed<r.Number>(0)
        };
        var token3 = new Token.Creator.Create.
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