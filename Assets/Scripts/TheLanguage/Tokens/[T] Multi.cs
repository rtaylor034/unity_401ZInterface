using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r = Resolutions;
using Res = Resolution;
using MorseCode.ITask;

#nullable enable
namespace Tokens.Multi
{
    public sealed record Union<R> : PureCombiner<Res.IMulti<R>, r.Multi<R>> where R : class, ResObj
    {
        public Union(IEnumerable<IToken<Res.IMulti<R>>> elements) : base(elements) { }
        public Union(params IToken<Res.IMulti<R>>[] elements) : base(elements) { }
        protected override r.Multi<R> EvaluatePure(IEnumerable<Res.IMulti<R>> inputs)
        {
            return new() { Values = inputs.Map(x => x.Values).Flatten() };
        }
    }
    
    public sealed record Yield<R> : PureFunction<R, r.Multi<R>> where R : class, ResObj
    {
        public Yield(IToken<R> value) : base(value) { }
        protected override r.Multi<R> EvaluatePure(R in1)
        {
            return new() { Values = in1.Yield() };
        }
    }
    public sealed record Filter<R> : PureAccumulator<R, r.Bool, r.Multi<R>> where R : class, ResObj
    {
        public Filter(IToken<Res.IMulti<R>> iterator, string elementLabel, IToken<r.Bool> lambda) : base(iterator, elementLabel, lambda) { }
        protected override r.Multi<R> PureAccumulate(IEnumerable<(R element, r.Bool output)> outputs)
        {
            return new() { Values = outputs.Filter(x => x.output.IsTrue).Map(x => x.element) };
        }
    }
}
