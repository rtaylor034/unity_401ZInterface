
using System.Collections.Generic;
using Perfection;
using MorseCode.ITask;
#nullable enable
namespace FourZeroOne.Runtime
{
    using ResObj = Resolution.IResolution;
    using IToken = Token.Unsafe.IToken;
    using Token;
    using ControlledTask;
    public interface IRuntime
    {
        public State GetState();
        public ITask<IOption<R>> PerformAction<R>(IToken<R> action) where R : class, ResObj;
        public ITask<IOption<IEnumerable<R>>> ReadSelection<R>(IEnumerable<R> from, int count) where R : class, ResObj;
    }
    public abstract class Runtime : IRuntime
    {
        public State GetState() => _currentState;
        public ITask<IOption<R>> PerformAction<R>(IToken<R> action) where R : class, ResObj
        {
            throw new System.NotImplementedException();
        }
        public ITask<R> EvaluateToken<R>(State startingState, IToken<R> token) where R : class, ResObj
        {
            throw new System.NotImplementedException();
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