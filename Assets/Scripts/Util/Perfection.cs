using System;
using System.Collections;
using System.Collections.Generic;

// nothing in this namespace is validated; it assumes you use it perfectly.
namespace Perfection
{
    public struct Empty<T> : IEnumerable<T>
    {
        public readonly IEnumerator<T> GetEnumerator()
        {
            yield break;
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }
    }
    public class PBiMap<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
    {
        private Dictionary<T1, T2> _forward;
        private Dictionary<T2, T1> _backward;

        public PBiMap()
        {
            _forward = new();
            _backward = new();
        }
        public PBiMap(IEnumerable<KeyValuePair<T1, T2>> values)
        {
            _forward = new(values);
            _backward = new(InvertKeys(values));
        }
        public PBiMap(PBiMap<T1, T2> original)
        {
            _forward = original._forward;
            _backward = original._backward;
        }
        public PBiMap<T1, T2> With(IEnumerable<KeyValuePair<T1, T2>> values)
        {
            Dictionary<T1, T2> nforward = new(_forward.Also(values));
            Dictionary<T2, T1> nbackward = new(_backward.Also(InvertKeys(values)));
            return new()
            {
                _forward = nforward,
                _backward = nbackward
            };
        }
        private static IEnumerable<KeyValuePair<V, K>> InvertKeys<K, V>(IEnumerable<KeyValuePair<K, V>> values)
        {
            foreach (var val in values) { yield return KeyValuePair.Create(val.Value, val.Key); }
        }
        public T2 Get(T1 key) => _forward[key];
        public T1 Get(T2 key) => _backward[key];

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<T1, T2>>)_forward).GetEnumerator();
        }
        public IEnumerator<KeyValuePair<T2, T1>> InvertedEnumerator()
        {
            return (_backward).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_forward).GetEnumerator();
        }
    }
    public class PList<T> : IEnumerable<T>
    {
        private List<T> _list;
        public PList()
        {
            _list = new();
        }
        public PList(IEnumerable<T> values)
        {
            _list = new(values);
        }
        public PList<T> With(IEnumerable<T> values)
        {
            return new(_list.Also(values));
        }
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
    // me when i rewrite System.Linq but worse and with rust names
    public static class Extensions
    {
        public static IEnumerable<TResult> Map<TIn, TResult>(this IEnumerable<TIn> enumerable, Func<TIn, TResult> mapFunction)
        {
            foreach (var e in enumerable)
            {
                yield return mapFunction(e);
            }
        }
        public static IEnumerable<T> Also<T>(this IEnumerable<T> enumerable, IEnumerable<T> also)
        {
            foreach (var v in enumerable) yield return v;
            foreach (var v in also) yield return v;
        }
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            foreach (var v in enumerable) if (predicate(v)) yield return v;
        }
        public static IEnumerable<(int index, T value)> Enumerate<T>(this IEnumerable<T> enumerable)
        {
            int i = 0;
            foreach (var v in enumerable) yield return (i++, v);
        }
        /// <summary>
        /// WARNING: generates INFINITE iterator. meant to be used with <see cref="Until{T}(IEnumerable{T}, Predicate{T})"/>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> GenerateSequence<T>(this T startingValue, Func<T, T> function)
        {
            T o = startingValue;
            while (true)
            {
                yield return o;
                o = function(o);
            }
        }
        public static TResult AccumulateInto<TIn, TResult>(this IEnumerable<TIn> enumerable, TResult startingValue, Func<TResult, TIn, TResult> function)
        {
            TResult o = startingValue;
            foreach (var v in enumerable)
            {
                o = function(o, v);
            }
            return o;
        }
        public static IEnumerable<T> Until<T>(this IEnumerable<T> enumerable, Predicate<T> breakCondition)
        {
            foreach (var v in enumerable)
            {
                if (breakCondition(v)) yield break;
                yield return v;
            }
        }
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        {
            foreach (var list in enumerable)
                foreach (var e in list) yield return e;
        }
        public static bool HasMatch<T>(this IEnumerable<T> enumerable, Predicate<T> matchCondition)
        {
            foreach (var v in enumerable) if (matchCondition(v)) return true;
            return false;
        }
    }
}