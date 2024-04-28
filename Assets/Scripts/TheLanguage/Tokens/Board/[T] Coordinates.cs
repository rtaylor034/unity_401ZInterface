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
        protected override rb.Coordinates EvaluatePure(Resolution.Board.IPositioned in1)
        {
            return in1.Position;
        }
    }
}