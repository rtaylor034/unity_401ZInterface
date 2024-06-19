
using System.Collections.Generic;
using Perfection;
using ControlledTasks;
#nullable enable
namespace FourZeroOne.Runtime
{
    using ResObj = Resolution.IResolution;
    using IToken = Token.Unsafe.IToken;
    using Token;
    using ControlledTasks;
    public interface IRuntime
    {
        public State GetState();
        public ICeasableTask<IOption<R>> PerformAction<R>(IToken<R> action) where R : class, ResObj;
        public ICeasableTask<IOption<IEnumerable<R>>> ReadSelection<R>(IEnumerable<R> from, int count) where R : class, ResObj;
    }

    //garbage collector reliant/heavy implementation
    public abstract class FrameSaving : IRuntime
    {
        public State GetState() => _currentState;
        public ICeasableTask<IOption<R>> PerformAction<R>(IToken<R> action) where R : class, ResObj
        {
            throw new System.NotImplementedException();
        }
        public ICeasableTask<R> EvaluateToken<R>(State startingState, IToken<R> token) where R : class, ResObj
        {
            throw new System.NotImplementedException();
        }
        public ICeasableTask<IOption<IEnumerable<R>>> ReadSelection<R>(IEnumerable<R> from, int count) where R : class, ResObj
        {
            return SelectionImplementation(from, count);
        }

        protected abstract void RecieveToken(IToken token);
        protected abstract void RecieveResolution(IOption<ResObj> resolution);
        protected abstract void RecieveRuleSteps(IEnumerable<(IToken token, Rule.IRule appliedRule)> steps);
        protected abstract ControlledTask<IOption<IEnumerable<R>>> SelectionImplementation<R>(IEnumerable<R> from, int count) where R : class, ResObj;

        protected void GotoFrame(Frame frame)
        {
            _operationStack = frame.OperationStack;
            _resolutionStack = frame.ResolutionStack;
            Run();
        }

        protected record Frame
        {
            public IToken Token { get; init; }
            public ResObj Resolution { get; init; }
            public State PreviousState { get; init; }
            public LinkedStack<IToken> OperationStack { get; init; }
            public LinkedStack<ResObj> ResolutionStack { get; init; }
        }
        protected record LinkedStack<T>
        {
            public readonly IOption<LinkedStack<T>> Link;
            public readonly T Value;
            public LinkedStack(T value)
            {
                Value = value;
                Link = this.None();
            }
            public static IOption<LinkedStack<T>> Linked(IOption<LinkedStack<T>> parent, IEnumerable<T> values)
            {
                return values.AccumulateInto(parent, (stack, x) => new LinkedStack<T>(stack, x).AsSome());
            }
            public static IOption<LinkedStack<T>> Linked(IOption<LinkedStack<T>> parent, params T[] values) { return Linked(parent, values.IEnumerable()); }
            private LinkedStack(IOption<LinkedStack<T>> link, T value)
            {
                Link = link;
                Value = value;
            }
        }

        private ICeasableTask<ResObj> Run()
        {
            var operationLink = _operationStack.Unwrap();
            var token = operationLink.Value;
            _operationStack = operationLink.Link.
            
        }
        private static IToken<R> ApplyRules<R>(IToken<R> token, IEnumerable<Rule.IRule> rules, out List<(IToken<R> fromToken, Rule.IRule rule)> appliedRules) where R : class, ResObj
        {
            var o = token;
            appliedRules = new();
            foreach (var rule in rules)
            {
                if (rule.TryApplyTyped(o) is IToken<R> newToken)
                {
                    appliedRules.Add((o, rule));
                    o = newToken;
                }
            }
            return o;
        }

        private State _currentState;
        private ControlledAwaiter _evalThread;
        private IOption<LinkedStack<IToken>> _operationStack;
        private IOption<LinkedStack<ResObj>> _resolutionStack;

    }
}