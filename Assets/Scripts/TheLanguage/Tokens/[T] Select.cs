using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r = Resolutions;
using MorseCode.ITask;
using FourZeroOne;
#nullable enable
namespace Tokens.Select
{
    public sealed record One<R> : Function<Resolution.IMulti<R>, R> where R : class, ResObj
    {
        public override bool IsFallibleFunction => true;
        public One(IToken<Resolution.IMulti<R>> from) : base(from) { }

        protected async override ITask<IOption<R>?> Evaluate(IProgram program, IOption<Resolution.IMulti<R>> fromOpt)
        {
            if (fromOpt.CheckNone(out var from)) return new None<R>();
            if (await program.Input.ReadSelection(from.Values, 1) is not IOption<IEnumerable<R>> selOpt) return null;
            return (selOpt.Check(out var sel)) ? sel.First()?.AsSome() : new None<R>();
        }
    }

    //make range instead of single int count
    public sealed record Multiple<R> : Function<Resolution.IMulti<R>, r.Number, r.Multi<R>> where R : class, ResObj
    {
        public override bool IsFallibleFunction => true;
        public Multiple(IToken<Resolution.IMulti<R>> from, IToken<r.Number> count) : base(from, count) { }

        protected override async ITask<IOption<r.Multi<R>>?> Evaluate(IProgram program, IOption<Resolution.IMulti<R>> fromOpt, IOption<r.Number> countOpt)
        {
            if (fromOpt.CheckNone(out var from) || countOpt.CheckNone(out var count)) return new None<r.Multi<R>>();
            if (await program.Input.ReadSelection(from.Values, 1) is not IOption<IEnumerable<R>> selOpt) return null;
            return selOpt.RemapAs(v => new r.Multi<R>() { Values = v });
        }
    }
}