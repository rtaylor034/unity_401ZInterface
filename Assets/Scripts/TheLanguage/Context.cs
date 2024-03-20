using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Context.Token;

//TOKENSTREAM :D
namespace Context
{
    public interface IContextData { }

    namespace Any
    {
        public class Data : IContextData
        {
            n
        }
        namespace Tokens
        {
            namespace Int
            {
                public sealed class Constant : IToken<int, Data>
                {
                    public readonly int Value;
                    public Constant(int value) => Value = value;
                    public int Resolve(Data _) => Value;

                }
                public sealed class BinaryOperation : IToken<int, Data>
                {
                    public enum EOperation
                    {
                        Add,
                        Subtract,
                        Divide,
                        Multiply
                    }
                    public readonly EOperation Operation;
                    public readonly IToken<int, Data> Left;
                    public readonly IToken<int, Data> Right;
                    public BinaryOperation(EOperation operation, IToken<int, Data> left, IToken<int, Data> right)
                    {
                        Operation = operation;
                        Left = left;
                        Right = right;
                    }
                    public int Resolve(Data data)
                    {
                        return Operation switch
                        {
                            EOperation.Add => Left.Resolve(data) + Right.Resolve(data),
                            EOperation.Subtract => Left.Resolve(data) - Right.Resolve(data),
                            EOperation.Divide => Left.Resolve(data) / Right.Resolve(data),
                            EOperation.Multiply => Left.Resolve(data) * Right.Resolve(data),
                            _ => throw new System.ArgumentOutOfRangeException("Why are we casting enums?")
                        };
                    }
                }
                public sealed class UnaryOperation : IToken<int, Data>
                {
                    public enum EOperation
                    {
                        Negate
                    }
                    public readonly EOperation Operation;
                    public readonly IToken<int, Data> Operand;
                    public UnaryOperation(EOperation operation, IToken<int, Data> operand)
                    {
                        Operation = operation;
                        Operand = operand;
                    }
                    public int Resolve(Data data)
                    {
                        return Operation switch
                        {
                            EOperation.Negate => -Operand.Resolve(data),
                            _ => throw new System.ArgumentOutOfRangeException("Why are we casting enums?")
                        };
                    }
                }
            }
            namespace Set
            {
                //Reference behavior TBI
                public sealed class SelectOne<T> : IToken<IEnumerable<T>, Data>
                {
                    public readonly IToken<IEnumerable<T>, Data> From;
                    public SelectOne(IToken<IEnumerable<T>, Data> from) { From = from; }
                    public IEnumerable<T> Resolve(Data context) => From.Resolve(context);
                }
            }
        }
    }
    namespace Global
    {
        public class Data : Any.Data
        {
            public int TestValue;
            public HashSet<Unit> AllUnits;
        }
        namespace Tokens
        {
            public sealed class TestValue : IToken<int, Data> { public int Resolve(Data context) => context.TestValue; }
            public sealed class AllUnits : IToken<IEnumerable<Unit>, Data>
            {
                public IEnumerable<Unit> Resolve(Data context)
                {
                    return context.AllUnits;
                }
            }
        }
    }
    namespace Ability
    {
        public class Data : Global.Data
        {
            public Unit Source;
        }
        namespace Tokens
        {
            public sealed class Source : IToken<Unit, Data> { public Unit Resolve(Data context) => context.Source; }
        }
    }

    namespace Token
    {
        public interface IToken<out T, in C> where C : IContextData
        {
            public T Resolve(C context);
        }
        public class Reference<T, C> : IToken<T, C> where C : IContextData
        {
            private IToken<T, C> _refersTo;
            public T Resolve(C context) => _refersTo.Resolve(context);
            public Reference(IToken<T, C> refersTo)
            {
                _refersTo = refersTo;
            }
        }
    }
}