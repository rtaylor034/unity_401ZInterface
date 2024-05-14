using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r = Resolutions;
using rb = Resolutions.Board;
using FourZeroOne;
using MorseCode.ITask;

namespace Tokens.Board.Unit
{
    public sealed record AllUnits : Infallible<r.Multi<rb.Unit>>
    {
        protected override IOption<r.Multi<rb.Unit>> InfallibleResolve(IProgram program)
        {
            return new r.Multi<rb.Unit>() { Values = program.State.Board.Units }.AsSome();
        }
    }
    namespace Get
    {
        public sealed record HP : PureFunction<rb.Unit, r.Number>
        {
            public HP(IToken<rb.Unit> of) : base(of) { }
            protected override r.Number EvaluatePure(rb.Unit unit)
            {
                return unit.HP;
            }
        }
        public sealed record Effects : PureFunction<rb.Unit, r.Multi<rb.Unit.Effect>>
        {
            public Effects(IToken<rb.Unit> of) : base(of) { }
            protected override r.Multi<rb.Unit.Effect> EvaluatePure(rb.Unit unit)
            {
                return unit.Effects;
            }
        }
    }
}