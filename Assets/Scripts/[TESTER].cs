
using System;
using System.Collections;
using System.Collections.Generic;
using Token;
using Proxy;
using UnityEngine;
using System.Threading.Tasks;
using MorseCode.ITask;
using r = Resolutions;
using Proxy.Creator;
using Perfection;
using Tokens;
using Tokens.Number;
using Tokens.Alias;
using Tokens.Multi;
public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        // its important to rememeber that P.AsIs() will work just fine unless a proxies' arguements will have OriginalArgs somewhere.
        var token_monster = new Scope<r.Number>(new Variable<r.Number>("scope1", new Multiply(new Fixed<r.Number>(4), new Fixed<r.Number>(8))))
        {
            SubToken = new Add(new MultTwo(new Subtract(new Reference<r.Number>("scope1"), new Fixed<r.Number>(5))), new MultTwo(new Reference<r.Number>("scope1")))
        };
        var token_accu =
            new Tokens.Multi.Union<r.Number>(
                Iter.Over(1, 6, 4, 3, 8, 7, 2, 9, 5).Map(x => new Fixed<r.Number>(x).YieldToken()))
            .FilterToken("x", new Tokens.Number.Compare.GreaterThan(new Reference<r.Number>("x"), new Fixed<r.Number>(4)));
        var rule_co = Rule.Create.For<Fixed<r.Number>, r.Number>(P =>
        {
            return P.AsIs(new Fixed<r.Number>(2));
        });
        var rule_alias = Rule.Create.For<MultTwo, r.Number>(P =>
        {
            return P.Construct<Scope<r.Number>>().WithEnvironment(P.AsIs(new Variable<r.Number>("scope2", new Fixed<r.Number>(8)))) with
            {
                SubTokenProxy = P.Construct<Add>()
                .WithArgs(P.AsIs(new Reference<r.Number>("scope2")), P.AsIs(new Reference<r.Number>("scope1")))
            };
        });

        var context = new Context()
        {
            InputProvider = null,
            State = null,
            Rules = new(),
            Variables = new(7)
        };
        var my_rules = Iter.Over(rule_alias).Take(0);
        Debug.Log(await token_accu.ResolveWithRules(context with { Rules = new() { Elements = my_rules } } ));
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