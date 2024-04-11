using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace Resolution
{
    using Token;
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
namespace Resolutions
{
    using Resolution;
    public sealed record Number : NonMutating
    {
        public int Value { get; init; }
    }
}