using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace Resolution
{
    using Token;
    public abstract record Resolution
    {
        /// <summary>
        /// <i>Use <see cref="Context.WithResolution(Resolution)"/> instead.</i>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Context _ChangeContext(Context context) => ChangeContext(context);
        protected abstract Context ChangeContext(Context before);
    }
    public abstract record NonMutating : Resolution
    {
        protected override Context ChangeContext(Context context) => context;
    }
}
namespace Resolutions
{
    using Resolution;
    public sealed record Number : NonMutating
    {
        public int Value { get; init; }
    }
}