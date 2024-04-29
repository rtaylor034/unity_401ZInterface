using System.Collections;
using System.Collections.Generic;
using Token;
using Perfection;
using FourZeroOne;

#nullable enable
namespace Resolution
{

    /// <summary>
    /// all inherits must be by a record class.
    /// </summary>
    public interface IResolution
    {
        public State ChangeState(State context);
        public bool ResEqual(IResolution? other);
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
}
namespace Resolution.Unsafe
{
    //not actually unsafe, just here because you should either extend 'Operation' or 'NoOp'.
    public abstract record Resolution : IResolution
    {
        public virtual bool ResEqual(IResolution? other) => Equals(other);
        public State ChangeState(State before) => ChangeStateInternal(before);
        protected abstract State ChangeStateInternal(State context);
        /// <summary>
        /// <i>Use <see cref="State.WithResolution(Resolution)"/> instead.</i>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
    }
}