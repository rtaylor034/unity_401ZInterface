using System;
using System.Collections;
using System.Collections.Generic;

// nothing in this namespace is validated; it assumes you use it perfectly.
namespace Perfection
{
    public delegate T Updater<T>(T original);
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
        public static IEnumerable<T> Yield()
        {
            yield break;
        }
    }
    // Shitty ass HashSet
    public record PSet<T> : PQuickIndexSet<T, T>
    {
        public PSet(int modulo, IEnumerable<T> elements) : base(modulo, x => x, elements) { }
        public PSet(int modulo) : base(modulo, x => x) { }
    }
    public record PQuickIndexSet<I, T> : IEnumerable<T>
    {
        private readonly List<List<T>> _storage;
        public readonly int Modulo;
        public readonly int Count;
        public readonly Func<T, I> IndexGenerator;
        public PQuickIndexSet(int modulo, Func<T, I> indexGenerator, IEnumerable<T> elements)
        {
            Modulo = modulo;
            IndexGenerator = indexGenerator;
            (Count, _storage) = elements
                .AccumulateInto((0, new List<List<T>>(modulo).FillEmpty(new(2))),
                   (data, x) =>
                   {
                       var (count, store) = data;
                       var bucket = store[indexGenerator(x).GetHashCode() % modulo];
                       var index = bucket.FindIndex(y => x.Equals(y));
                       if (index == -1) bucket.Add(x);
                       bucket[index] = x;
                       return (++count, store);
                   });
        }
        public PQuickIndexSet(int modulo, Func<T, I> indexGenerator)
        {
            Modulo = modulo;
            IndexGenerator = indexGenerator;
            Count = 0;
            _storage = new(0);
        }
        public bool Contains(T other) => GetBucket(IndexGenerator(other)).Contains(other);
        public T this[T element] => GetBucket(IndexGenerator(element)).Find(x => element.Equals(x));
        public IEnumerable<T> AreaOfIndex(I indexer) => GetBucket(indexer);
        private List<T> GetBucket(I index) => _storage[index.GetHashCode() % Modulo];
        public IEnumerator<T> GetEnumerator() => _storage.Flatten().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _storage.Flatten().GetEnumerator();
    }
    // Shitty ass Dictionary
    public record PMap<K, T> : IEnumerable<(K key, T val)>
    {
        private readonly List<List<(K key, T val)>> _storage;
        public readonly int Modulo;
        public readonly int Count;
        public PMap(int modulo, IEnumerable<(K key, T val)> elements)
        {
            Modulo = modulo;
            (Count, _storage) = elements
                .AccumulateInto((0, new List<List<(K key, T val)>>(modulo).FillEmpty(new(2))),
                   (data, x) =>
                   {
                       var (count, store) = data;
                       var bucket = store[x.key.GetHashCode() % modulo];
                       var index = bucket.FindIndex(y => x.key.Equals(y.key));
                       if (index == -1) bucket.Add(x);
                       bucket[index] = x;
                       return (++count, store);
                   });
        }
        public PMap(int modulo)
        {
            Modulo = modulo;
            Count = 0;
            _storage = new(0);
        }
        public T this[K indexer] => GetBucket(indexer).Find(x => indexer.Equals(x.key)).val;
        private List<(K key, T val)> GetBucket(K element) => _storage[element.GetHashCode() % Modulo];
        public IEnumerator<(K key, T val)> GetEnumerator() => _storage.Flatten().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _storage.Flatten().GetEnumerator();
    }
    public record PList<T> : IEnumerable<T>
    {
        private readonly List<T> _list;
        public readonly int Count;
        public PList(IEnumerable<T> elements)
        {
            _list = new(elements);
            Count = _list.Count;
        }
        public PList()
        {
            _list = new(0);
            Count = 0;
        }
        public T this[int i] => _list[i];
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_list).GetEnumerator();
    }


    // me when i rewrite System.Linq but worse and with rust names
#nullable enable
    public static class Enumerable_ext
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
        public static IEnumerable<T> Yield<T>(this T value, int amount)
        {
            for (int i = 0; i < amount; i++) yield return value;
        }
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
        public static List<T> FillEmpty<T>(this List<T> list, T item)
        {
            for (int i = list.Count; i < list.Capacity; i++) list.Add(item);
            return list;
        }
    }
}