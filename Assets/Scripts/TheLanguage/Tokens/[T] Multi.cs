using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r = Resolutions;
using Res = Resolution;
using MorseCode.ITask;
namespace Tokens.Multi
{
    public sealed record Union<R> : PureCombiner<Res.IMulti<R>, r.Multi<R>> where R : class, ResObj
    {
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

    //make abstract version
    public sealed record Where<R> : Token<r.Multi<R>> where R : ResObj
    {
        public Where(IToken<r.Multi<R>> from, IToken<r.Bool> condition)
        {
            _from = from;
            _condition = condition;
        }
        public override ITask<r.Multi<R>> Resolve(Context context)
        {
            
        }
        private readonly IToken<r.Multi<R>> _from;
        private readonly IToken<r.Bool> _condition;
    }
}
