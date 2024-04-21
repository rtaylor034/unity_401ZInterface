using System.Collections;
using System.Collections.Generic;
using ResObj = Resolution.IResolution;
using Token;
using Perfection;
using r = Resolutions;
using Proxy.Creator;
using Proxy;
namespace Tokens.Alias
{
    public record MultTwo : Token.Alias.OneArg<r.Number, r.Number>
    {
        private readonly static IProxy<MultTwo, r.Number> PROXY = Create.For<MultTwo, r.Number>(P =>
        {
            return P.Construct<Number.Multiply>()
                .WithArgs(P.OriginalArg1(), P.AsIs(new Fixed<r.Number>(2)));
        });
        public MultTwo(IToken<r.Number> to) : base(to, PROXY) { }
    }
}