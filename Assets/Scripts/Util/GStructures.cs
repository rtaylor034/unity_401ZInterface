using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

// yes, I will shamelessly halfass implement rust features because im just special like that.
namespace GStructures
{
    public class NahException : System.NotSupportedException
    {
        public NahException() : base("NahException reached. It's literally over.") { }
    }
    public abstract class Option<T>
    {
        private static Exception _unrecognizedException => new NotSupportedException("Unwrapped a class that extends 'Option', but isnt 'None' or 'Some' (why the fuck are we extending 'Option'?)");
        public sealed class None : Option<T> { }
        public sealed class Some : Option<T>
        {
            public T Value;
            public Some(T value)
            {
                Value = value;
            }
        }
        public T Unwrap() => this switch
        {
            Some value => value.Value,
            None => throw new Exception("Unwrapped a 'None' Option."),
            _ => throw _unrecognizedException
        };
        public T UnwrapOrElse(Func<T> evaluate) => this switch
        {
            Some value => value.Value,
            None => evaluate(),
            _ => throw _unrecognizedException
        };
    }
}