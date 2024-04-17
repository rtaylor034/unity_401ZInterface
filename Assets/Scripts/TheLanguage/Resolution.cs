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
        public Context ChangeContext(Context context);
    }
    public abstract record Operation : Unsafe.Resolution
    {
        protected override sealed Context ChangeContextInternal(Context before) => UpdateContext(before);
        protected abstract Context UpdateContext(Context context);
    }
    public abstract record NoOp : Unsafe.Resolution
    {
        protected override sealed Context ChangeContextInternal(Context context) => context;
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
        /// <summary>
        /// <i>Use <see cref="Context.WithResolution(Resolution)"/> instead.</i>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Context ChangeContext(Context before) => ChangeContextInternal(before);
        protected abstract Context ChangeContextInternal(Context context);
    }
}