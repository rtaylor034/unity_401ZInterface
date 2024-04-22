
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
using FourZeroOne;
public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        // its important to rememeber that P.AsIs() will work just fine unless a proxies' arguements will have OriginalArgs somewhere.
        var token_monster = new SubEnvironment<r.Number>(new Variable<r.Number>("scope1", new Multiply(new Fixed<r.Number>(4), new Fixed<r.Number>(8))))
        {
            SubToken = new Add(new MultTwo(new Subtract(new Reference<r.Number>("scope1"), new Fixed<r.Number>(5))), new MultTwo(new Reference<r.Number>("scope1")))
        };
        var token_accu =
            new Tokens.Multi.Union<r.Number>(
                Iter.Over(1, 6, 4, 3, 8, 7, 2, 9, 5).Map(x => new Fixed<r.Number>(x).YieldToken()))
            .FilterToken("x", new Tokens.Number.Compare.GreaterThan(new Reference<r.Number>("x"), new Fixed<r.Number>(4)));
        var token_union =
            new Tokens.Multi.Union<r.Number>(Iter.Over(1, 6, 4, 3, 8, 7, 2, 9, 5).Map(x => new Fixed<r.Number>(x).YieldToken()));
        var rule_co = Rule.Create.For<Fixed<r.Number>, r.Number>(P =>
        {
            return P.AsIs(new Fixed<r.Number>(2));
        });
        var rule_alias = Rule.Create.For<Union<r.Number>, r.Multi<r.Number>>(P =>
        {
            return P.Construct<SubEnvironment<r.Multi<r.Number>>>().WithEnvironment(P.AsIs(new Variable<r.Number>("y", new Fixed<r.Number>(8)))) with
            {
                SubTokenProxy = P.Construct<Filter<r.Number>>().OverTokens(P.Construct<Union<r.Number>>().WithOriginalArgs(),
                "x", P.AsIs(new Tokens.Number.Compare.GreaterThan(new Reference<r.Number>("x"), new Reference<r.Number>("y"))))
            };
        });
        var g = new GameObject();
        var IO = g.AddComponent<GameWorld>();
        var program = new FourZeroOne.Program(IO, IO)
        {
            State = new()
            {
                Rules = new() { Elements = Iter.Over(rule_alias).Take(0) },
                Variables = new(7),
                Board = new() { }
            }
        };
        Debug.Log(await token_monster.ResolveWithRules(program));
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