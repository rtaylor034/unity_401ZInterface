using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perfection;
using System;
using ResObj = Resolution.Resolution;

#nullable enable
namespace Proxy
{
    using Token;
    namespace Unsafe
    {
        using Token.Unsafe;
        public interface IProxy
        {
            public IToken UnsafeRealize(IToken original);
        }
        public interface IProxy<out R> : IProxy where R : ResObj
        {
            public abstract IToken<R> UnsafeTypedRealize(IToken original);
        }
        public interface IArgProxy<in TOrig, out R> : Unsafe.IProxy<R> where TOrig : IToken where R : ResObj
        {
            public IToken<R> Realize(TOrig original);
        }
        public abstract record ArgProxy<TOrig, R> : IArgProxy<TOrig, R> where TOrig : IToken where R : ResObj
        {
            public abstract IToken<R> Realize(TOrig original);
            public IToken<R> UnsafeTypedRealize(IToken original) => Realize((TOrig)original);
            public IToken UnsafeRealize(IToken original) => UnsafeTypedRealize(original);
        }
        public abstract record FunctionProxy<TOrig, R> : Proxy<TOrig, R>
            where TOrig : IToken<R>
            where R : ResObj
        {
            /// <summary>
            /// Expected to be [ : ]<see cref="Token.Unsafe.TokenFunction{T}"/>
            /// </summary>
            protected List<IProxy> ArgProxies { get; init; }
            protected FunctionProxy(params IProxy[] proxies)
            {
                ArgProxies = new(proxies);
            }
            protected FunctionProxy(IEnumerable<IProxy> proxies)
            {
                ArgProxies = new(proxies);
            }
            public FunctionProxy(FunctionProxy<TOrig, R> original) : base(original)
            {
                ArgProxies = new(original.ArgProxies);
            }
            protected abstract TokenFunction<R> ConstructFromArgs(List<IToken> tokens);
            public override IToken<R> Realize(TOrig original) => ConstructFromArgs(MakeSubstitutions(original));
            protected List<IToken> MakeSubstitutions(TOrig original)
            {
                return new(ArgProxies.Map(x => x.UnsafeRealize(original)));
            }
        }

    }
    public interface IProxy<in TOrig, out R> : Unsafe.IArgProxy<TOrig, R> where TOrig : IToken<R> where R : ResObj { }
    public abstract record Proxy<TOrig, R> : Unsafe.ArgProxy<TOrig, R>, IProxy<TOrig, R> where TOrig : IToken<R> where R : ResObj { }
    
    public static class Extensions
    {
        public static Proxies.Direct<IToken<R>, R> AsProxy<R>(this IToken<R> token) where R : ResObj
        {
            return new(token);
        }
    }
}
namespace Proxies
{
    using Proxy;
    using Token;
    using Token.Unsafe;
    using Proxy.Unsafe;

    public record Direct<TOrig, R> : Proxy<TOrig, R> where TOrig : IToken<R> where R : ResObj
    {
        protected IToken<R> Token { get; init; }
        public override IToken<R> Realize(TOrig _) => Token;
        public Direct(IToken<R> token) => Token = token;
        public Direct<TTo, R> Fix<TTo>() where TTo : IToken<R> => new(Token);
    }

    public sealed record CombinerTransform<TNew, RArg, ROut> : ArgProxy<IHasCombineArgs<RArg, ROut>, RArg>
        where TNew : Token.Combiner<RArg, ROut>
        where RArg : ResObj
        where ROut : ResObj
    {
        public override IToken<RArg> Realize(IHasCombineArgs<RArg, ROut> original)
        {
            return (IToken<RArg>)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArg>>) })
                .Invoke(new object[] { original.Args });
        }
    }
    public record Combiner<TNew, TOrig, RArg, ROut> : FunctionProxy<TOrig, ROut>
        where TNew : Token.Combiner<RArg, ROut>
        where TOrig : IToken<ROut>
        where RArg : ResObj
        where ROut : ResObj
    {
        public Combiner(IEnumerable<IArgProxy<TOrig, RArg>> proxies) : base(proxies) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            //SHAKY
            return (TokenFunction<ROut>)typeof(TNew).GetConstructor(new Type[] { typeof(IEnumerable<IToken<RArg>>) })
                .Invoke(new object[] { tokens });
        }
    }
    #region OriginalArgs
    // ---- [ OriginalArgs ] ----
    public sealed record OriginalArg1<RArg, ROut> : ArgProxy<IHasArg1<RArg, ROut>, RArg> where RArg : ResObj where ROut : ResObj
    {
        public override IToken<RArg> Realize(IHasArg1<RArg, ROut> original) => original.Arg1;
    }
    public sealed record OriginalArg2<RArg, ROut> : ArgProxy<IHasArg2<RArg, ROut>, RArg> where RArg : ResObj where ROut : ResObj
    {
        public override IToken<RArg> Realize(IHasArg2<RArg, ROut> original) => original.Arg2;
    }
    public sealed record OriginalArg3<RArg, ROut> : ArgProxy<IHasArg3<RArg, ROut>, RArg> where RArg : ResObj where ROut : ResObj
    {
        public override IToken<RArg> Realize(IHasArg3<RArg, ROut> original) => original.Arg3;
    }
    // --------
    #endregion
    #region Functions
    // ---- [ Functions ] ----
    public record Function<TNew, TOrig, RArg1, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken<ROut>
        where TNew : Token.Function<RArg1, ROut>
        where RArg1 : ResObj
        where ROut : ResObj
    {
        public Function(IArgProxy<TOrig, RArg1> in1) : base(in1) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>) })
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TNew, TOrig, RArg1, RArg2, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken<ROut>
        where TNew : Token.Function<RArg1, RArg2, ROut>
        where RArg1 : ResObj
        where RArg2 : ResObj
        where ROut : ResObj
    {
        public Function(IArgProxy<TOrig, RArg1> in1, IArgProxy<TOrig, RArg2> in2) : base(in1, in2) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>), typeof(IToken<RArg2>)})
                .Invoke(tokens.ToArray());
        }
    }
    public record Function<TNew, TOrig, RArg1, RArg2, RArg3, ROut> : FunctionProxy<TOrig, ROut>
        where TOrig : IToken<ROut>
        where TNew : Token.Function<RArg1, RArg2, RArg3, ROut>
        where RArg1 : ResObj
        where RArg2 : ResObj
        where RArg3 : ResObj
        where ROut : ResObj
    {
        public Function(IArgProxy<TOrig, RArg1> in1, IArgProxy<TOrig, RArg2> in2, IArgProxy<TOrig, RArg2> in3) : base(in1, in2, in3) { }
        protected override TokenFunction<ROut> ConstructFromArgs(List<IToken> tokens)
        {
            return (TokenFunction<ROut>)
                typeof(TNew).GetConstructor(new Type[] { typeof(IToken<RArg1>), typeof(IToken<RArg2>), typeof(IToken<RArg3>) })
                .Invoke(tokens.ToArray());
        }
    }
    // --------
    #endregion

}