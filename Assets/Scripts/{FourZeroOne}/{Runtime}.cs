
using System.Collections.Generic;
using Perfection;
using ControlledTasks;
using System.Threading.Tasks;
#nullable enable
namespace FourZeroOne.Runtime
{
    using ResObj = Resolution.IResolution;
    using Resolved = IOption<Resolution.IResolution>;
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
            public Resolved Resolution { get; init; }
            public State State { get; init; }
            public IOption<LinkedStack<IToken>> OperationStack { get; init; }
            public IOption<LinkedStack<Resolved>> ResolutionStack { get; init; }
        }
        protected class LinkedStack<T>
        {
            public readonly IOption<LinkedStack<T>> Link;
            public readonly T Value;
            public readonly int Depth;
            public LinkedStack(T value)
            {
                Value = value;
                Link = this.None();
                Depth = 0;
            }
            public static IOption<LinkedStack<T>> Linked(IOption<LinkedStack<T>> parent, int depth, IEnumerable<T> values)
            {
                return values.AccumulateInto(parent, (stack, x) => new LinkedStack<T>(stack, x, depth).AsSome());
            }
            public static IOption<LinkedStack<T>> Linked(IOption<LinkedStack<T>> parent, int depth, params T[] values) { return Linked(parent, depth, values.IEnumerable()); }
            private LinkedStack(IOption<LinkedStack<T>> link, T value, int depth)
            {
                Link = link;
                Value = value;
                Depth = depth;
            }
        }

        private async Task Run()
        {
            while (_operationStack.Check(out var op))
            {
                var argTokens = op.Value.ArgTokens;
                if (argTokens.Length == 0 || (_resolutionStack.CheckNone(out var node) && node.Depth == op.Depth + 1))
                {
                    var argPass = new Resolved[argTokens.Length];
                    for (int i = argPass.Length; i >= 0; i--)
                    { 
                        argPass[i] = PopFromStack(ref _resolutionStack).Value;
                    }
                    _evalThread = op.Value.ResolveUnsafe(this, argPass);
                    var resolution = await _evalThread;
                    if (resolution.Check(out var notNolla)) _currentState = _currentState.WithResolution(notNolla);
                    PushToStack(ref _resolutionStack, op.Depth, resolution);
                    PopFromStack(ref _operationStack);
                    AddFrame(op.Value, resolution);
                    continue;
                }

                PushToStack(ref _operationStack, op.Depth + 1, op.Value.ArgTokens.AsMutList().Reversed());
            }
        }
        private void AddFrame(IToken token, Resolved resolution)
        {
            var frame = new Frame()
            {
                Resolution = resolution,
                Token = token,
                State = _currentState,
                OperationStack = _operationStack,
                ResolutionStack = _resolutionStack,
            };
            throw new System.NotImplementedException();
        }
        private static void PushToStack<T>(ref IOption<LinkedStack<T>> stack, int depth, IEnumerable<T> values)
        {
            stack = LinkedStack<T>.Linked(stack, depth, values);
        }
        private static LinkedStack<T> PopFromStack<T>(ref IOption<LinkedStack<T>> stack)
        {
            var o = stack.Check(out var popped) ? popped : throw new System.Exception("[Runtime] tried to pop from empty LinkedStack.");
            if (stack.Check(out var node)) stack = node.Link;
            return o;
        }
        private static void PushToStack<T>(ref IOption<LinkedStack<T>> stack, int depth, params T[] values) { PushToStack(ref stack, depth, values.IEnumerable()); }
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
        private ICeasableTask<Resolved> _evalThread;
        private IOption<LinkedStack<Frame>> _frameStack;
        private IOption<LinkedStack<IToken>> _operationStack;
        private IOption<LinkedStack<Resolved>> _resolutionStack;

    }
}