using System;
using System.Collections.Generic;

#nullable enable
public static class Misc
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
public static class Integer
{
    public static int Abs(this int value) => Math.Abs(value);
}