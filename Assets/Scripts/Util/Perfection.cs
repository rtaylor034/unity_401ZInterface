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
    public record PSet<T> : PIndexedSet<T, T>
    {
        public PSet(int modulo) : base(x => x, modulo) { }
        public override string ToString() => _storage
            .AccumulateInto($"PSet:\n", (msg1, x) => msg1 +
        $"{x.AccumulateInto(">", (msg2, y) => msg2 + $" [{y}]\n  ")}\n");
    }
    public record PIndexedSet<I, T> : IEnumerable<T>
    {
        protected readonly List<List<T>> _storage;
        public IEnumerable<T> Elements { get => _storage.Flatten(); init
            {
                Count = 0;
                _storage = new List<List<T>>(Modulo);
                _storage.AddRange(new List<T>(2).GenerateSequence((_) => new(2)).Take(Modulo));
                foreach (var v in value)
                {
                    var bindex = IndexGenerator(v).GetHashCode().Abs() % Modulo;
                    var foundAt = _storage[bindex].FindIndex(x => IndexGenerator(v).Equals(IndexGenerator(x)));
                    if (foundAt == -1) _storage[bindex].Add(v);
                    else _storage[bindex][foundAt] = v;
                }
            }
        } 
        public Updater<IEnumerable<T>> dElements { init => Elements = value(Elements); }
        public readonly int Modulo;
        public readonly int Count;
        public readonly Func<T, I> IndexGenerator;
        public PIndexedSet(Func<T, I> indexGenerator, int modulo)
        {
            Modulo = modulo;
            IndexGenerator = indexGenerator;
            Count = 0;
            _storage = new(0);
        }
        public bool Contains(I index) => GetBucket(index).HasMatch(x => IndexGenerator(x).Equals(index));
        public T this[I index] => GetBucket(index).Find(x => IndexGenerator(x).Equals(index));
        private List<T> GetBucket(I index) => _storage[index.GetHashCode().Abs() % Modulo];
        public IEnumerator<T> GetEnumerator() => _storage.Flatten().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _storage.Flatten().GetEnumerator();
        public override string ToString() => _storage.AccumulateInto("PIndexedSet:\n", (msg1, x) => msg1 +
        $"{x.AccumulateInto(">", (msg2, y) => msg2 + $" [{IndexGenerator(y)} : {y}]\n  ")}\n");
    }
    // Shitty ass Dictionary
    public record PMap<K, T>
    {
        private readonly List<List<(K key, T val)>> _storage;
        public IEnumerable<(K key, T val)> Elements
        {
            get => _storage.Flatten(); init
            {
                Count = 0;
                _storage = new List<List<(K key, T val)>>(Modulo);
                _storage.AddRange(new List<(K key, T val)>(2).GenerateSequence((_) => new(2)).Take(Modulo));
                foreach (var v in value)
                {
                    var bindex = v.key.GetHashCode().Abs() % Modulo;
                    var foundAt = _storage[bindex].FindIndex(x => v.key.Equals(x.key));
                    if (foundAt == -1) _storage[bindex].Add(v);
                    else _storage[bindex][foundAt] = v;
                }
            }
        } 
        public Updater<IEnumerable<(K key, T val)>> dElements { init => Elements = value(Elements); }
        public readonly int Modulo;
        public readonly int Count;
        public PMap(int modulo)
        {
            Modulo = modulo;
            Count = 0;
            _storage = new(0);
        }
        public T this[K indexer] => GetBucket(indexer).Find(x => indexer.Equals(x.key)).val;
        private List<(K key, T val)> GetBucket(K element) => _storage[element.GetHashCode().Abs() % Modulo];
        public override string ToString() => Elements.AccumulateInto("PMap:\n", (msg, x) => msg + $"- [{x.key} : {x.val}]\n");
    }
    public record PList<T>
    {
        private readonly List<T> _list;
        public IEnumerable<T> Elements { get => _list; init { _list = new(value); Count = _list.Count; } } 
        public Updater<IEnumerable<T>> dElements { init => Elements = value(Elements); }
        public readonly int Count;
        public PList()
        {
            _list = new(0);
            Count = 0;
        }
        public T this[int i] => _list[i];
        public override string ToString() => Elements.AccumulateInto("PList:\n", (msg, x) => msg + $"- {x}\n");
    }
    // me when i rewrite System.Linq but worse and with rust names
#nullable enable
    public static class IEnumerable_ext
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
    }
    public static class Misc_ext
    {
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
    public static class Math_ext
    {
        public static int Abs(this int value) => Math.Abs(value);
    }
}