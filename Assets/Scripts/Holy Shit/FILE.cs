using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Hands down THE WORST idea ive ever went through with.
namespace HolyShit
{

    public class Unit
    {
        int HP;
    }
    public class UnresolvedMove : SerialInterface
    {
        private List<Unit> _units;
        private int _spaces;

        public interface GetSchema : GET_SCHEMA
        {
            public sealed record Spaces : GetSchema, Gettable<int, UnresolvedMove>
            {
                public int Get(UnresolvedMove self) => self._spaces;
            }
            public sealed record Units : GetSchema { }
            public sealed record SingleUnit : GetSchema
            {
                public int Index { get; set; }
            }
        }
        public interface SetSchema : SET_SCHEMA
        {
            public sealed record Spaces : GetSchema { }
            public sealed record Units : GetSchema { }
            public sealed record SingleUnit : GetSchema
            {
                public int Index { get; set; }
            }
        }
    }

    public interface GET_SCHEMA { }
    public interface Gettable<T, in Self>
    {
        public T Get(Self self);
    }
    public class Expression<T, Obj> where Obj : SerialInterface
    {
        private readonly Gettable<T, Obj> _initial;
        private readonly List<Modifier<T>> _modifiers;

        public (Gettable<T, Obj>, List<Modifier<T>>) Components => (_initial, new(_modifiers));
        public T Evaluate(Obj obj)
        {
            var o = _initial.Get(obj);
            foreach (var mod in _modifiers) o = mod.Apply(o);
            return o;
        }

        public Expression(Gettable<T, Obj> initial)
        {
            _initial = initial;
            _modifiers = new();
        }
        public Expression<T, Obj> Mod(Modifier<T> mod)
        {
            _modifiers.Add(mod);
            return this;
        }

    }
    public interface Modifier<T>
    {
        public T Apply(T to);
    }
    public class IntMod : Modifier<int>
    {
        public EOperation Operation { get; set; }
        public int Operand { get; set; }
        public enum EOperation
        {
            Add,
            Subtract,
        }
        public int Apply(int to) => Operation switch
        {
            EOperation.Add => to + Operand,
            EOperation.Subtract => to - Operand,
            _ => throw new System.NotImplementedException(),
        };
    }
    public interface SET_SCHEMA { }

    public interface SerialInterface { }
}
