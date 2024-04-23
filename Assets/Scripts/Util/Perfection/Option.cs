using System.Collections;
using System.Collections.Generic;
using System;

namespace Perfection
{
    using Options;
    public interface IOption<out T> { }
    public interface ISome<T> : IOption<T>
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
    public static class _Option
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
        public static IOption<T> Map<T>(this IOption<T> option, Func<T, T> func)
        {
            return CheckNone(option, out var o) ? option : func(o).AsSome();
        }
        public static IOption<T> AsNone<T>(this T _)
        {
            return new None<T>();
        }
    }
    
}