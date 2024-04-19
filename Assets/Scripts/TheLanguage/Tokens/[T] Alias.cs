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
    public record AddTen : Token.Alias.OneArg<r_.Number, r_.Number>
    {
        private readonly static IProxy<AddTen, r_.Number> PROXY = Create.For<AddTen, r_.Number>(P =>
        {
            return P.TokenFunction<Number.Add>()
                .WithArgs(P.OriginalArg1(), P.AsIs(new Number.Constant(10)));
        });
        public AddTen(IToken<r_.Number> to) : base(to, PROXY) { }
    }
}