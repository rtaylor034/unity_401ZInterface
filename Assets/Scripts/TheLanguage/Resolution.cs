using System.Collections;
using System.Collections.Generic;
using Token;
using Perfection;
using ResObj = Resolution.IResolution;

#nullable enable
namespace Resolution
{

    /// <summary>
    /// all inherits must be by a record class.
    /// </summary>
    public interface IResolution
    {
        public GameState ChangeState(GameState context);
    }

    public abstract record Operation : Unsafe.Resolution
    {
        protected abstract GameState UpdateState(GameState context);
        protected override sealed GameState ChangeStateInternal(GameState before) => UpdateState(before);
    }

    public abstract record NoOp : Unsafe.Resolution
    {
        protected override sealed GameState ChangeStateInternal(GameState context) => context;
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
        protected abstract GameState ChangeStateInternal(GameState context);
        /// <summary>
        /// <i>Use <see cref="GameState.WithResolution(Resolution)"/> instead.</i>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public GameState ChangeState(GameState before) => ChangeStateInternal(before);
    }
}