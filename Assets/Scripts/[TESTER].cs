
using System;
using System.Collections;
using System.Collections.Generic;
using Token;
using GameActions;
using UnityEngine;

public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<Func<int>> bruh = new()
        {
            () => 6
        };
        Debug.Log(bruh.Count);
        List<Func<int>> ok = new()
        {
            bruh[0]
        };
        ok.Remove(bruh[0]);
        Debug.Log(ok.Count);
        GameActions.Move.Expression<Context.Ability.Data> expression1 = new()
        {
            MovableUnits = new Context.Global.Tokens.AllUnits(),
            TestInt = new Context.Any.Tokens.Int.Constant(5),
            Test = new Context.Ability.Tokens.Source()
        };
        GameActions.Move.Expression<Context.Global.Data> expression2 = null;
        ExpressionSet <Context.Ability.Data> o = null;
        o._expressions.Add(expression1);
        o._expressions.Add(expression2);

        In<AClass> ina = null;
        In<BClass> inb = null;
        Out<AClass> outa = null;
        Out<BClass> outb = null;
        outa = outb;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class AClass
{

}
public class BClass : AClass
{

}
public interface In<in T> { }
public interface Out<out T> { }