using System.Collections;
using System.Collections.Generic;
using System;

namespace Perfection
{
    public interface IOption<out T> { }
    public interface ISome<out T> : IOption<T>
    {
        public T Value { get; }
    }
    public record Some<T> : ISome<T>
    {
        public T Value => _value;
        public Some(T value) => _value = value;
        private readonly T _value;
    }
    public record None<T> : IOption<T> { }
    public static class Option
    {
        public static T Unwrap<T>(this IOption<T> option)
        {
            return option switch
            {
                ISome<T> ok => ok.Value,
                _ => throw new System.Exception("Unwrapped non-Some option.")
            };
        }
        public static ISome<T> AsSome<T>(this T value)
        {
            return new Some<T>(value);
        }
        public static bool Check<T>(this IOption<T> option, out T val)
        {
            if (option is ISome<T> ok)
            {
                val = ok.Value;
                return true;
            }
            val = default;
            return false;
        }
        public static bool CheckNone<T>(this IOption<T> option, out T val) => !Check(option, out val);
        public static IOption<TOut> Map<TIn, TOut>(this IOption<TIn> option, Func<TIn, TOut> func)
        {
            return CheckNone(option, out var o) ? new None<TOut>() : func(o).AsSome();
        }
        public static IOption<T> None<T>(this T _)
        {
            return new None<T>();
        }
    }
    
}