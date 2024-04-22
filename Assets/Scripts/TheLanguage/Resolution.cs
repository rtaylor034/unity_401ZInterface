using System.Collections;
using System.Collections.Generic;
using Token;
using Perfection;
using ResObj = Resolution.IResolution;
using Program;

#nullable enable
namespace Resolution
{

    /// <summary>
    /// all inherits must be by a record class.
    /// </summary>
    public interface IResolution
    {
        public State ChangeState(State context);
    }

    public abstract record Operation : Unsafe.Resolution
    {
        protected abstract State UpdateState(State context);
        protected override sealed State ChangeStateInternal(State before) => UpdateState(before);
    }

    public abstract record NoOp : Unsafe.Resolution
    {
        protected override sealed State ChangeStateInternal(State context) => context;
    }

    public interface IMulti<out R> : ResObj where R : ResObj
    {
        public IEnumerable<R> Values { get; }
    }
}
namespace Resolution.Unsafe
{
    //not actually unsafe, just here because you should either extend 'Operation' or 'NoOp'.
    public abstract record Resolution : IResolution
    {
        protected abstract State ChangeStateInternal(State context);
        /// <summary>
        /// <i>Use <see cref="State.WithResolution(Resolution)"/> instead.</i>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public State ChangeState(State before) => ChangeStateInternal(before);
    }
}