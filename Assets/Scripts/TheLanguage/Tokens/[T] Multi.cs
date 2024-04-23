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
        protected override IOption<r.Multi<R>> EvaluatePure(IEnumerable<IOption<Res.IMulti<R>>> inputs)
        {
            return new r.Multi<R>() { Values = inputs.Filter(x => x.Check(out var _)).Map(x => x.Unwrap().Values).Flatten() }.AsOption();
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
    public static class _Extensions
    {
        public static Filter<R> FilterToken<R>(this IToken<Res.IMulti<R>> iterator, string elementLabel, IToken<r.Bool> lambda) where R : class, ResObj
        {
            return new(iterator, elementLabel, lambda);
        }
        public static Yield<R> YieldToken<R>(this IToken<R> token) where R : class, ResObj
        {
            return new(token);
        }
    }
}
