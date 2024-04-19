
using System;
using System.Collections;
using System.Collections.Generic;
using Token;
using Proxy;
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using r_ = Resolutions;
using Proxy.Creator;
using Perfection;
using Tokens;
using Tokens.Number;
using INT = Resolutions.Number;
using Tokens.Alias;
public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        //this beats herion
        var token_monster = new SubEnvironment<INT>(new Variable<INT>("scope1", new Multiply(new Constant(4), new Constant(8))))
        {
            SubToken = new Add(new MultTwo(new Subtract(new Reference<INT>("scope1"), new Constant(1))), new MultTwo(new Reference<INT>("scope1")))
        };
        var rule_co = Rule.Create.For<Constant, INT>(P =>
        {
            return P.AsIs(new Constant(2));
        });
        var rule_alias = Rule.Create.For<MultTwo, INT>(P =>
        {
            return P.TokenFunction<MultTwo>()
            .WithArgs(P.TokenFunction<MultTwo>()
                .WithArgs(P.AsIs(new Constant(5))));
        });
        var context = new Context()
        {
            InputProvider = null,
            State = null,
            Rules = new(),
            Variables = new(7)
        };
        var bruh = 1.Sequence(x => x + 1).ContinueAfter(x => x.Take(10).Also(0.Yield(5))).Take(20);
        Debug.Log(new PList<int>() { Elements = bruh });
        Debug.Log(await token_monster.ResolveWithRules(context with { Rules = new() { Elements = Iter.Over(rule_alias) } } ));
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