using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r_ = Resolutions;
namespace Tokens.Multi
{
    public sealed record Union<R> : PureCombiner<Resolution.IMulti<R>, r_.Multi<R>> where R : class, ResObj
    {
        protected override r_.Multi<R> EvaluatePure(IEnumerable<Resolution.IMulti<R>> inputs)
        {
            return new() { Values = inputs.Map(x => x.Values).Flatten() };
        }
    }
    public sealed record Yield<R> : Infallible<r_.Multi<R>> where R : class, ResObj
    {
        private readonly R _value;
        public Yield(R value) => _value = value;
        protected override r_.Multi<R> InfallibleResolve(Context context) => new() { Values = _value.Yield() };
    }
}
