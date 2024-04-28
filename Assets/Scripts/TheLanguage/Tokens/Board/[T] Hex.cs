using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r = Resolutions;
using rb = Resolutions.Board;
using FourZeroOne;
using MorseCode.ITask;

namespace Tokens.Board.Hex
{
    public sealed record AllHexes : Infallible<r.Multi<rb.Hex>>
    {
        protected override IOption<r.Multi<rb.Hex>> InfallibleResolve(IProgram program)
        {
            return new r.Multi<rb.Hex>() { Values = program.State.Board.Hexes }.AsSome();
        }
    }
    namespace Get
    {

    }
}