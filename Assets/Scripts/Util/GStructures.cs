using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        public sealed class Some : Option<T>
        {
            public T Value { get; }
            public Some(T value)
            {
                Value = value;
            }
        }
        public sealed class None : Option<T> { }
        public T Expect(string msg) => this switch
        {
            Some value => value.Value,
            _ => throw new Exception(msg)
        };
        public T Unwrap() => Expect($"Unwrapped a {GetType().Name} {GetType().BaseType.Name}.");
        public T UnwrapOrElse(Func<T> evaluate) => this switch
        {
            Some value => value.Value,
            _ => evaluate()
        };
        public Option<T> Map(Func<T, T> func) => this switch
        {
            Some some => new Some(func(some.Value)),
            _ => this
        };
    }
    public abstract class Result<T, E>
    {
        public sealed class Ok : Result<T, E>
        {
            public T Value { get; }
            public Ok(T value) => Value = value;
        }
        public sealed class Err : Result<T, E>
        {
            public E Value { get; }
            public Err(E value) => Value = value;
        }
        public Result<E, T> Invert() => this switch
        {
            Ok ok => new Result<E, T>.Err(ok.Value),
            Err err => new Result<E, T>.Ok(err.Value),
            _ => throw new System.NotSupportedException()
        };
        public T Expect(string msg) => this switch
        {
            Ok value => value.Value,
            _ => throw new Exception(msg)
        };
        public T Unwrap() => Expect($"Unwrapped a {GetType().Name} {GetType().BaseType.Name}.");
        public T UnwrapOrElse(Func<T> evaluate) => this switch
        {
            Ok value => value.Value,
            _ => evaluate()
        };
        public Result<T, E> Map(Func<T, T> func) => this switch
        {
            Ok ok => new Ok(func(ok.Value)),
            _ => this
        };
    }
}