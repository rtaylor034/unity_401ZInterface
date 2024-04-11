using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Token;

#nullable enable
namespace Resolution
{

    /// <summary>
    /// all inherits must be by a record class.
    /// </summary>
    public interface IResolution
    {
        public Context ChangeContext(Context before);
    }
    public abstract record Resolution : IResolution
    {
        /// <summary>
        /// <i>Use <see cref="Context.WithResolution(Resolution)"/> instead.</i>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Context ChangeContext(Context before);
    }
    public abstract record NonMutating : Resolution
    {
        public override Context ChangeContext(Context context) => context;
    }
}
#nullable disable
namespace Resolutions
{
    using Perfection;
    using Resolution;
    public sealed record Number : NonMutating
    {
        public int Value { get; init; }
    }
    public sealed record Multi<R> : Resolution where R : IResolution
    {
        private List<R> _elements { get; init; }
        public IEnumerable<R> Values { get => _elements; init { _elements = new(value); } }
        public override Context ChangeContext(Context before)
        {
            return before.GenerateValueOver(_elements, (prev, e) => prev.WithResolution(e));
        }
    }
}