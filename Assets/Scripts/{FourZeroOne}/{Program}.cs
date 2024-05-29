
using System.Collections.Generic;
using Perfection;
using MorseCode.ITask;
#nullable enable
namespace FourZeroOne.Program
{
    using ResObj = Resolution.IResolution;
    using IToken = Token.Unsafe.IToken;
    using Token;
    using ControlledTask;
    public interface IProgram
    {
        public State GetState();
        public IOption<ResObj>[] GetArgs();
        public ITask<IOption<R>> PerformAction<R>(IToken<R> action) where R : class, ResObj;
        public void ObserveToken(IToken token);
        public void ObserveResolution(IOption<ResObj> resolution);
        public void ObserveRuleSteps(IEnumerable<(IToken fromToken, Rule.IRule appliedRule)> steps);
        public ITask<IOption<IEnumerable<R>>> ReadSelection<R>(IEnumerable<R> from, int count) where R : class, ResObj;
    }
    public abstract class Program : IProgram
    {
        public State GetState() => _currentState;
        public IOption<ResObj>[] GetArgs()
        {
            throw new System.NotImplementedException();
        }

        public void ObserveToken(IToken token)
        {
            _evalStack.Push((_currentState, token));
            RecieveToken(token);
        }
        public void ObserveResolution(IOption<ResObj> resolution)
        {
            var evalState = _evalStack.Pop().state;
            if (resolution.Check(out var some)) _currentState = evalState.WithResolution(some);
            RecieveResolution(resolution);
        }
        public void ObserveRuleSteps(IEnumerable<(IToken fromToken, Rule.IRule appliedRule)> steps)
        {
            _currentState = _currentState with
            {
                dRules = Q => Q with
                {
                    dElements = Q => Q.Filter(x => !steps.HasMatch(y => ReferenceEquals(x, y.appliedRule)))
                }
            };
            RecieveRuleSteps(steps);
        }
        
        public ITask<IOption<IEnumerable<R>>> ReadSelection<R>(IEnumerable<R> from, int count) where R : class, ResObj
        {
            return SetTokenThread(SelectionImplementation(from, count));
        }
        protected abstract void RecieveToken(IToken token);
        protected abstract void RecieveResolution(IOption<ResObj> resolution);
        protected abstract void RecieveRuleSteps(IEnumerable<(IToken token, Rule.IRule appliedRule)> steps);
        protected abstract ControlledTask<IOption<IEnumerable<R>>> SelectionImplementation<R>(IEnumerable<R> from, int count) where R : class, ResObj;
        protected void SwitchToNode()
        {

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

        private ControlledTask<T> SetTokenThread<T>(ControlledTask<T> task)
        {
            _tokenThread = task.Awaiter;
            return task;
        }
        private Stack<(State state, IToken token)> _evalStack;
        private State _currentState;
        private ControlledAwaiter _tokenThread;
    }
}