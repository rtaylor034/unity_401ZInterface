using System;
using System.Collections;
using System.Collections.Generic;
using Context.Token;
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
        IToken<int, Context.Global> o = null;
        IToken<int, Context.Ability> a = o;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
