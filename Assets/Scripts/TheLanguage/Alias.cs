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
        public override bool IsFallible => Expand().IsFallible;
        public override ITask<R?> Resolve(Context context)
        {
            return Expand().ResolveWithRules(context);
        }
        protected Alias(Proxy.Unsafe.IProxy<R> proxy)
        {
            _proxy = proxy;
            _cachedRealization = null;
        }
        private IToken<R> Expand()
        {
            return (_cachedRealization is null) ? _proxy.UnsafeTypedRealize(this, null) : _cachedRealization;
        }
        private readonly Proxy.Unsafe.IProxy<R> _proxy;
        //NOTE: this only works under the assumption that proxies are perfect pure (stateless immutable).
        private readonly IToken<R>? _cachedRealization;
        
    }
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    public abstract record OneArg<RArg1, ROut> : Alias<ROut>, IFunction<RArg1, ROut>
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
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;, IToken&lt;<typeparamref name="RArg2"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    /// <typeparam name="RArg2"></typeparam>
    public abstract record TwoArg<RArg1, RArg2, ROut> : Alias<ROut>, IFunction<RArg1, RArg2, ROut>
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
    /// <summary>
    /// Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;, IToken&lt;<typeparamref name="RArg2"/>&gt;, IToken&lt;<typeparamref name="RArg3"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    /// <typeparam name="RArg2"></typeparam>
    /// <typeparam name="RArg3"></typeparam>
    public abstract record ThreeArg<RArg1, RArg2, RArg3, ROut> : Alias<ROut>, IFunction<RArg1, RArg2, RArg3, ROut>
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