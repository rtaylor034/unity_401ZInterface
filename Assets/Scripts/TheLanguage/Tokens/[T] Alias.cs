using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r_ = Resolutions;
using Proxy.Creator;
using Proxy;
namespace Tokens.Alias
{
    public record MultTwo : Token.Alias.OneArg<r_.Number, r_.Number>
    {
        private readonly static IProxy<MultTwo, r_.Number> PROXY = Create.For<MultTwo, r_.Number>(P =>
        {
            return P.TokenFunction<Number.Multiply>()
                .WithArgs(P.OriginalArg1(), P.AsIs(new Number.Constant(2)));
        });
        public MultTwo(IToken<r_.Number> to) : base(to, PROXY) { }
    }
}