
using System;
using System.Collections;
using System.Collections.Generic;
using Token;
using TokenSyntax;
using ProxySyntax;
using Proxy;
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using r = Resolutions;
using t = Tokens;
using Proxy.Creator;
using Perfection;
using Tokens.Number;
using FourZeroOne;
public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        var token1 = MakeToken.tRecursive<r.Number, r.Multi<r.Number>, r.Number>(new()
        {
            A = 0.tConst(),
            B = 1.Sequence(x => x + 1).Take(5).Map(x => x.tConst()).tToMulti(),
            RecursiveProxyStatement = P =>
                P.pSubEnvironment(RHint<r.Number>.Specify(), new()
                {
                    EnvironmentProxies = new()
                    {
                        P.pOriginalB().pAs(out var pool),
                        pool.tRef().pAsProxyFor(P).pIO_SelectOne().pAs(out var selection),
                        P.pOriginalA().pAs(out var counter)
                    },
                    SubProxy = P.pOriginalA().pIsGreaterThan(2.tConst().pAsProxyFor(P)).pIfTrue(RHint<r.Number>.Specify(), new()
                    {
                        Then = selection.tRef().pAsProxyFor(P),
                        Else = P.pRecurseWith(new()
                        {
                            A = counter.tRef().tAdd(1.tConst()).pAsProxyFor(P),
                            B = pool.tRef().tWithout(selection.tRef().tYield()).pAsProxyFor(P)
                        }).pAdd(selection.tRef().pAsProxyFor(P))
                    })
                })
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
            Debug.Log("===========[ START ]============");
            Debug.Log(await token1.ResolveWithRules(program));
            Debug.Log("===========[ END ]==============");
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