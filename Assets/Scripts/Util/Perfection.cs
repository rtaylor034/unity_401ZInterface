using System;
using System.Collections;
using System.Collections.Generic;

// nothing in this namespace is validated; it assumes you use it perfectly.
namespace Perfection
{
    public interface IUnique
    {
        public int UniqueId { get; }
    }
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
        public static IEnumerable<E> Yield<E>()
        {
            yield break;
        }
    }
    public record PSet<U> : IEnumerable<U> where U : IUnique
    {
        private List<List<U>> _storage;
        private int _capacity;
        public IEnumerable<U> Insertions { init { foreach (var v in value) { Insert(v); } } }
        public PSet(int capacity)
        {
            _capacity = capacity;
            _storage = new List<List<U>>(capacity).FillEmpty(new());
        }
        public PSet(PSet<U> original)
        {
            _capacity = original._capacity;
            _storage = new(original._storage.Map(x => new List<U>(x)));
        }
        private void Insert(U element)
        {
            var i = GetCell(element).FindIndex(x => element.UniqueId == x.UniqueId);
            if (i == -1) GetCell(element).Add(element);
            else GetCell(element)[i] = element;
        }
        private List<U> GetCell(U element) => _storage[element.UniqueId % _capacity];

        public IEnumerator<U> GetEnumerator() => _storage.Flatten().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
#nullable enable
    // me when i rewrite System.Linq but worse and with rust names
    public static class Extensions
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
        public static IEnumerable<T> After<T>(this IEnumerable<T> enumerable, Predicate<T> startCondition)
        {
            var iter = enumerable.GetEnumerator();
            while (iter.MoveNext() && !startCondition(iter.Current)) { }
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
        public static IEnumerable<T> Yield<T>(this T value) { yield return value; }
        public static IEnumerable<T> Take<T>(this IEnumerable<T> enumerable, int amount)
        {
            int i = 0;
            foreach (var v in enumerable)
            {
                if (i >= amount) yield break;
                yield return v;
                i++;
            } 
        }
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> enumerable, int amount)
        {
            var iter = enumerable.GetEnumerator();
            for (int i = -1; i < amount; i++) iter.MoveNext();
            while (iter.MoveNext()) yield return iter.Current;
        }
        public static T? First<T>(this IEnumerable<T> enumerable) where T : class
        {
            var iter = enumerable.GetEnumerator();
            return iter.MoveNext() ? iter.Current : null;
        }
        public static List<T> Reversed<T>(this List<T> list)
        {
            List<T> o = new(list);
            o.Reverse();
            return o;
        }
        public static List<T?> FillEmpty<T>(this List<T?> list, T item)
        {
            for (int i = list.Count; i < list.Capacity; i++) list.Add(item);
            return list;
        }
    }
}