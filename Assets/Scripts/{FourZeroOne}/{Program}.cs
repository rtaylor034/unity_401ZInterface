
using System.Collections.Generic;
using Perfection;
using MorseCode.ITask;
#nullable enable
namespace FourZeroOne.Program
{
    using ResObj = Resolution.IResolution;
    using Token.Unsafe;
    public interface IProgram
    {
        public State GetState();
        public void ObserveToken(IToken token);
        public void ObserveResolution(IOption<ResObj> resolution);
        public void ObserveRuleSteps(IEnumerable<(IToken, Rule.IRule)> steps);
        public ITask<IOption<IEnumerable<R>>> ReadSelection<R>(IEnumerable<R> from, int count) where R : class, ResObj;
    }
    public abstract class Program : IProgram
    {
        public State GetState() => CurrentState;
        
        public void ObserveToken(IToken token)
        {
            RecieveToken(token);
        }
        public void ObserveResolution(IOption<ResObj> resolution)
        {
            RecieveResolution(resolution);
        }
        public void ObserveRuleSteps(IEnumerable<(IToken, Rule.IRule)> steps)
        {
            RecieveRuleSteps(steps);
        }
        
        public ITask<IOption<IEnumerable<R>>> ReadSelection<R>(IEnumerable<R> from, int count) where R : class, ResObj
        {
            return SelectionImplementation(from, count);
        }
        protected abstract void RecieveToken(IToken token);
        protected abstract void RecieveResolution(IOption<ResObj> resolution);
        protected abstract void RecieveRuleSteps(IEnumerable<(IToken, Rule.IRule)> steps);
        
        protected abstract ControlledTask.ControlledTask<IOption<IEnumerable<R>>> SelectionImplementation<R>(IEnumerable<R> from, int count) where R : class, ResObj;

        protected State CurrentState;
    }
}