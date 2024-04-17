using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r_ = Resolutions;
using MorseCode.ITask;
#nullable enable
namespace Tokens.Select
{
    public sealed record One<R> : Function<Resolution.IMulti<R>, R> where R : class, ResObj
    {
        public override bool IsFallibleFunction => true;
        public One(IToken<Resolution.IMulti<R>> from) : base(from) { }
        protected override ITask<R?> Evaluate(Context context, Resolution.IMulti<R> in1)
        {
            throw new System.NotImplementedException();
        }
    }
    public sealed record Multiple<R> : Function<Resolution.IMulti<R>, r_.Multi<R>> where R : class, ResObj
    {
        public override bool IsFallibleFunction => true;
        public Multiple(IToken<Resolution.IMulti<R>> from) : base(from) { }
        protected override ITask<r_.Multi<R>?> Evaluate(Context context, Resolution.IMulti<R> in1)
        {
            throw new System.NotImplementedException();
        }
    }
}