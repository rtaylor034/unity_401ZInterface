using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MorseCode.ITask;
using System.Threading.Tasks;

namespace Token
{
    //instead of 
    public interface IToken<out T>
    {
        public ITask<T> Resolve(Expressions.References.IProvider scope);
    }
    
    //horrible
    
}