using System;
using System.Collections;
using System.Collections.Generic;
using Context.Token;
using GameActions;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

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
        GameActions.Move.Expression<Context.Ability.Data> expression = new()
        {
            TestInt = new Context.Global.Tokens.TestValue()
        };
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