using System;
using System.Collections;
using System.Collections.Generic;

// nothing in this namespace is validated; it assumes you use it perfectly.
#nullable enable
namespace Perfection
{
    public static class Iter
    {
        public static IEnumerable<TResult> Map<TIn, TResult>(this IEnumerable<TIn> enumerable, Func<TIn, TResult> mapFunction)
        {
            foreach (var e in enumerable) yield return mapFunction(e);
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
        public static IEnumerable<T> Sequence<T>(this T startingValue, Func<T, T> function)
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
                yield return v;
                if (breakCondition(v)) yield break;
            }
        }
        public static IEnumerable<T> After<T>(this IEnumerable<T> enumerable, Predicate<T> startCondition)
        {
            var iter = enumerable.GetEnumerator();
            while (iter.MoveNext())
            {
                if (startCondition(iter.Current))
                {
                    yield return iter.Current;
                    break;
                }
            }
            while (iter.MoveNext()) yield return iter.Current;
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
        public static IEnumerable<T> Yield<T>(this T value, int amount = 1)
        {
            for (int i = 0; i < amount; i++) yield return value;
        }
        public static IEnumerable<T> Take<T>(this IEnumerable<T> enumerable, int amount)
        {
            if (amount == 0) yield break;
            int i = 0;
            foreach (var v in enumerable)
            {
                yield return v;
                i++;
                if (i >= amount) yield break;
            }
        }
        public static IEnumerable<(T1 a, T2 b)> ShortZip<T1, T2>(this IEnumerable<T1> enumerableA, IEnumerable<T2> enumerableB)
        {
            var iter1 = enumerableA.GetEnumerator();
            var iter2 = enumerableB.GetEnumerator();
            while (iter1.MoveNext() && iter2.MoveNext()) yield return (iter1.Current, iter2.Current);
        }
        public static IEnumerable<(T1? a, T2? b)> LongZip<T1, T2>(this IEnumerable<T1> enumerableA, IEnumerable<T2> enumerableB)
        {
            var iter1 = enumerableA.GetEnumerator();
            var iter2 = enumerableB.GetEnumerator();
            while (iter1.MoveNext() && iter2.MoveNext()) yield return (iter1.Current, iter2.Current);
            while (iter1.MoveNext()) yield return (iter1.Current, default);
            while (iter2.MoveNext()) yield return (default, iter2.Current);
        }
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> enumerable, int amount)
        {
            var iter = enumerable.GetEnumerator();
            for (int i = -1; i < amount; i++) iter.MoveNext();
            while (iter.MoveNext()) yield return iter.Current;
        }
        public static IEnumerable<T> ContinueAfter<T>(this IEnumerable<T> enumerable, Func<IEnumerable<T>, IEnumerable<T>> consumer)
        {
            var iter = enumerable.GetEnumerator();
            foreach (var v in consumer(iter.AsEnumerable())) yield return v;
            foreach (var v in iter.AsEnumerable()) yield return v;
        }
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }
        public static T? First<T>(this IEnumerable<T> enumerable) where T : class
        {
            var iter = enumerable.GetEnumerator();
            return iter.MoveNext() ? iter.Current : null;
        }
        public static List<T> ToMutList<T>(this IEnumerable<T> enumerable) { return new List<T>(enumerable); }
        public static PList<T> ToList<T>(this IEnumerable<T> enumerable) { return new() { Elements = enumerable }; }
        public static PSet<T> ToSet<T>(this IEnumerable<T> enumerable, int modulo) { return new(modulo) { Elements = enumerable }; }
        public static PIndexedSet<I, T> ToIndexedSet<I, T>(this IEnumerable<T> enumerable, Func<T, I> indexGenerator, int modulo) { return new(indexGenerator, modulo) { Elements = enumerable }; }
        public static PMap<K, T> ToMap<K, T>(this IEnumerable<(K, T)> mapPairs, int modulo) { return new(modulo) { Elements = mapPairs }; }

        public static IEnumerable<T> Over<T>(params T[] arr)
        {
            foreach (var v in arr) yield return v;
        }
    }
    
}