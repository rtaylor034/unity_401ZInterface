using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r = Resolutions;
using MorseCode.ITask;
using Program;
#nullable enable
namespace Tokens.Select
{
    public sealed record One<R> : Function<Resolution.IMulti<R>, R> where R : class, ResObj
    {
        public override bool IsFallibleFunction => true;
        public One(IToken<Resolution.IMulti<R>> from) : base(from) { }

        protected override ITask<R?> Evaluate(IProgram program, Resolution.IMulti<R> from)
        {
            return program.Input.ReadSelection(from.Values);
        }
    }

    public sealed record Multiple<R> : Function<Resolution.IMulti<R>, r.Number, r.Multi<R>> where R : class, ResObj
    {
        public override bool IsFallibleFunction => true;
        public Multiple(IToken<Resolution.IMulti<R>> from, IToken<r.Number> count) : base(from, count) { }

        protected override async ITask<r.Multi<R>?> Evaluate(IProgram program, Resolution.IMulti<R> from, r.Number count)
        {
            return (await program.Input.ReadMultiSelection(from.Values, count.Value) is IEnumerable<R> selections) ?
                new() { Values = selections } :
                null;
        }
    }
}