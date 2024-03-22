using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

// yes, I will shamelessly halfass implement rust features because im just special like that.
namespace GStructures
{
    public abstract class Option<T>
    {
        public sealed class None : Option<T> { }
        public sealed class Some : Option<T>
        {
            public T Value;
            public Some(T value)
            {
                Value = value;
            }
        }
        public T Unwrap()
        {
            return this switch
            {
                Some value => value.Value,
                None => throw new Exception("Unwrapped a 'None' Option."),
                _ => throw new Exception("Unwrapped a class that extends 'Option', but isnt 'None' or 'Some' (why the fuck did you extend 'Option'?)")
            };
        }
    }
}