using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r_ = Resolutions;
using Resolutions;
namespace Tokens.Multi
{
    public sealed record Union<R> : PureCombiner<Resolution.IMulti<R>, r_.Multi<R>> where R : class, ResObj
    {
        protected override r_.Multi<R> EvaluatePure(IEnumerable<Resolution.IMulti<R>> inputs)
        {
            return new() { Values = inputs.Map(x => x.Values).Flatten() };
        }
    }
    
    public sealed record Yield<R> : PureFunction<R, r_.Multi<R>> where R : class, ResObj
    {
        public Yield(IToken<R> value) : base(value) { }
        protected override Multi<R> EvaluatePure(R in1)
        {
            return new() { Values = in1.Yield() };
        }
    }
}
