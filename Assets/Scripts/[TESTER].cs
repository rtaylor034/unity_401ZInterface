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
            () => 6 m
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
