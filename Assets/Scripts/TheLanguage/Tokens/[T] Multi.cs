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
    
    public sealed record Intersection<R> : PureCombiner<Res.IMulti<R>, r.Multi<R>> where R : class, ResObj
    {
        public Intersection(IEnumerable<IToken<Res.IMulti<R>>> sets) : base(sets) { }
        public Intersection(params IToken<Res.IMulti<R>>[] sets) : base(sets) { }
        protected override r.Multi<R> EvaluatePure(IEnumerable<Res.IMulti<R>> inputs)
        {
            var iter = inputs.GetEnumerator();
            if (!iter.MoveNext()) return new();
            var o = iter.Current.Values;
            while (iter.MoveNext())
            {
                o = o.Filter(x => iter.Current.Values.HasMatch(y => x.Equals(y)));
            }
            return new() { Values = o };
        }
    }

    public sealed record Exclusion<R> : PureFunction<Res.IMulti<R>, Res.IMulti<R>, r.Multi<R>> where R : class, ResObj
    {
        public Exclusion(IToken<Res.IMulti<R>> from, IToken<Res.IMulti<R>> exclude) : base(from, exclude) { }
        protected override r.Multi<R> EvaluatePure(Res.IMulti<R> in1, Res.IMulti<R> in2)
        {
            return new() { Values = in1.Values.Filter(x => !in2.Values.HasMatch(y => y.ResEqual(x))) };
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

    public sealed record Filtered<R> : PureAccumulator<R, r.Bool, r.Multi<R>> where R : class, ResObj
    {
        public Filtered(IToken<Res.IMulti<R>> iterator, VariableIdentifier<R> elementVariable, IToken<r.Bool> lambda) : base(iterator, elementVariable, lambda) { }
        protected override r.Multi<R> PureAccumulate(IEnumerable<(R element, r.Bool output)> outputs)
        {
            return new() { Values = outputs.Filter(x => x.output.IsTrue).Map(x => x.element) };
        }
    }

    public sealed record Count : PureFunction<Res.IMulti<ResObj>, r.Number>
    {
        public Count(IToken<Res.IMulti<ResObj>> of) : base(of) { }
        protected override r.Number EvaluatePure(Res.IMulti<ResObj> in1)
        {
            return new() { Value = in1.Count };
        }
    }
}
