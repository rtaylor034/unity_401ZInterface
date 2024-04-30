
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
using Select = Tokens.Select;
using FourZeroOne;
public class TESTER : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        var program = new FourZeroOne.Programs.Standard.Program()
        {
            State = new()
            {
                Rules = new(),
                Variables = new(7),
                Board = new() { }
            }
        };
        while ((await new Select.One<r.Bool>(new Union<r.Bool>(Iter.Over(true, false).Map(x => new Yield<r.Bool>(new Fixed<r.Bool>(x))))).Resolve(program)).Unwrap().IsTrue)
        {
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