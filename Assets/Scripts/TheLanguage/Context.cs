using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Context.Token;
using EvaluationProtocol;
using System;
//TOKENSTREAM :D
namespace Context
{
    public interface IContextData { }

    namespace Any
    {
        public class Data : IContextData
        {
            
        }
        namespace Tokens
        {
            namespace Int
            {
                public sealed class Constant : IToken<int, Data>
                {
                    public readonly int Value;
                    public Constant(int value) => Value = value;
                    public IProtocol<int> Resolve(Data _) => new Static<int>(Value);

                }
                public sealed class BinaryOperation : IToken<int, Data>
                {
                    public enum EOperation { Add, Subtract, Multiply, Divide }
                    public readonly IToken<int, Data> Left;
                    public readonly IToken<int, Data> Right;
                    public readonly EOperation Operation;
                    public BinaryOperation(IToken<int, Data> left, IToken<int, Data> right, EOperation operation)
                    {
                        Left = left;
                        Right = right;
                        Operation = operation;
                    }
                    public IProtocol<int> Resolve(Data context)
                    {
                        Func<int, int, int> function = Operation switch
                        {
                            EOperation.Add => (a, b) => a + b,
                            EOperation.Subtract => (a, b) => a - b,
                            EOperation.Multiply => (a, b) => a * b,
                            EOperation.Divide => (a, b) => a / b,
                        };
                        return new Combine<int, int, int>(Left.Resolve(context), Right.Resolve(context), function);
                    }
                }
            }
            namespace Set
            {
                namespace Select
                {
                    public sealed class One<T> : IToken<T, Data>
                    {
                        public readonly IEnumerable<T> From;
                        public One(IEnumerable<T> from) => From = from;
                        public IProtocol<T> Resolve(Data _) => new EvaluationProtocol.Select.One<T>(From);
                    }
                }
                
            }
        }
    }
    namespace Global
    {
        public class Data : Any.Data
        {
            public HashSet<Unit> AllUnits;
        }
        namespace Tokens
        {
            public sealed class AllUnits : IToken<IEnumerable<Unit>, Data> { public IProtocol<IEnumerable<Unit>> Resolve(Data context) => new Static<IEnumerable<Unit>>(context.AllUnits); }
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
            public sealed class Source : IToken<Unit, Data> { public IProtocol<Unit> Resolve(Data context) => new Static<Unit>(context.Source); }
        }
    }

    namespace Token
    {
        // genuinely on suicide watch due to https://github.com/dotnet/roslyn/issues/2981.
        // UPDATE : its ok guys, 3rd party library is here :D
        public interface IToken<out T, in C> : IDisplayComponent where C : IContextData
        {
            public IProtocol<T> Resolve(C context);
        }
        public interface IDisplayComponent { }
    }
}