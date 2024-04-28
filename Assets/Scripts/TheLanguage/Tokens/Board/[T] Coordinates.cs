using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r = Resolutions;
using rb = Resolutions.Board;
using Res = Resolution;
using FourZeroOne;
using MorseCode.ITask;

namespace Tokens.Board.Coordinates
{
    public sealed record Of : PureFunction<Res.Board.IPositioned, rb.Coordinates>
    {
        public Of(IToken<Res.Board.IPositioned> of) : base(of) { }
        protected override rb.Coordinates EvaluatePure(Res.Board.IPositioned in1)
        {
            return in1.Position;
        }
    }
    public sealed record OffsetArea : PureFunction<rb.Coordinates, Res.IMulti<rb.Coordinates>, r.Multi<rb.Coordinates>>
    {
        public OffsetArea(IToken<rb.Coordinates> offset, IToken<Res.IMulti<rb.Coordinates>> area) : base(offset, area) { }
        protected override r.Multi<rb.Coordinates> EvaluatePure(rb.Coordinates in1, Res.IMulti<rb.Coordinates> in2)
        {
            return new() { Values = in2.Values.Map(x => x.Add(in1)) };
        }
    }
}