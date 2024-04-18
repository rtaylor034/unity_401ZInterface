using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
using ResObj = Resolution.IResolution;
using r_ = Resolutions;
using Token;

#nullable enable
namespace Token.Alias
{
    public record Alias<R> : Token<R> where R : class, ResObj
    {
        public override bool IsFallible => _isFallible;
        public override ITask<R?> Resolve(Context context)
        {
            return _realizedToken.Resolve(context);
        }
        protected Alias(Proxy.Unsafe.IProxy<R> proxy)
        {
            _proxy = proxy;
            _realizedToken = proxy.UnsafeTypedRealize(this, null);
            _isFallible = _realizedToken.IsFallible;
        }
        private readonly Proxy.Unsafe.IProxy<R> _proxy;
        private readonly IToken<R> _realizedToken;
        private readonly bool _isFallible;
    }

    public abstract record OneArg<RArg1, ROut> : Alias<ROut>, IHasArg1<RArg1>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    {
        public IToken<RArg1> Arg1 => _arg1;
        protected OneArg(IToken<RArg1> in1, Proxy.Unsafe.IProxy<ROut> proxy) : base(proxy)
        {
            _arg1 = in1;
        }
        private readonly IToken<RArg1> _arg1;
    }

    public abstract record TwoArg<RArg1, RArg2, ROut> : Alias<ROut>, IHasArg1<RArg1>, IHasArg2<RArg2>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where ROut : class, ResObj
    {
        public IToken<RArg1> Arg1 => _arg1;
        public IToken<RArg2> Arg2 => _arg2;
        protected TwoArg(IToken<RArg1> in1, IToken<RArg2> in2, Proxy.Unsafe.IProxy<ROut> proxy) : base(proxy)
        {
            _arg1 = in1;
            _arg2 = in2;
        }
        private readonly IToken<RArg1> _arg1;
        private readonly IToken<RArg2> _arg2;
    }

    public abstract record ThreeArg<RArg1, RArg2, RArg3, ROut> : Alias<ROut>, IHasArg1<RArg1>, IHasArg2<RArg2>, IHasArg3<RArg3>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where RArg3 : class, ResObj
        where ROut : class, ResObj
    {
        public IToken<RArg1> Arg1 => _arg1;
        public IToken<RArg2> Arg2 => _arg2;
        public IToken<RArg3> Arg3 => _arg3;
        protected ThreeArg(IToken<RArg1> in1, IToken<RArg2> in2, IToken<RArg3> in3, Proxy.Unsafe.IProxy<ROut> proxy) : base(proxy)
        {
            _arg1 = in1;
            _arg2 = in2;
            _arg3 = in3;
        }
        private readonly IToken<RArg1> _arg1;
        private readonly IToken<RArg2> _arg2;
        private readonly IToken<RArg3> _arg3;
    }

}