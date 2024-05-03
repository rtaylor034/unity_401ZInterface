using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
using ResObj = Resolution.IResolution;
using Token;
using r = Resolutions;
using FourZeroOne;

#nullable enable
namespace Tokens
{
    public record SubEnvironment<ROut> : Token.Unsafe.TokenFunction<ROut>
        where ROut : class, ResObj
    {
        public IToken<ROut> SubToken { get; init; }
        public SubEnvironment(IEnumerable<Token.Unsafe.IToken> envModifiers) : base(envModifiers) { }
        public SubEnvironment(params Token.Unsafe.IToken[] envModifiers) : base(envModifiers) { }
        public sealed override bool IsFallibleFunction => SubToken.IsFallible;
        protected sealed override ITask<IOption<ROut>?> TransformTokens(IProgram program, IOption<ResObj>[] _)
        {
            return SubToken.ResolveWithRules(program);
        }
    }

    public record Recursive<RArg1, ROut> : Token.Alias.OneArg<RArg1, ROut>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    {
        public readonly Proxy.IProxy<Recursive<RArg1, ROut>, ROut> RecursiveProxy;
        public Recursive(IToken<RArg1> arg1, Proxy.IProxy<Recursive<RArg1, ROut>, ROut> recursiveProxy) : base(arg1, recursiveProxy)
        {
            RecursiveProxy = recursiveProxy;
        }
    }
    public record Recursive<RArg1, RArg2, ROut> : Token.Alias.TwoArg<RArg1, RArg2, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where ROut : class, ResObj
    {
        public readonly Proxy.IProxy<Recursive<RArg1, RArg2, ROut>, ROut> RecursiveProxy;
        public Recursive(IToken<RArg1> arg1, IToken<RArg2> arg2, Proxy.IProxy<Recursive<RArg1, RArg2, ROut>, ROut> recursiveProxy) : base(arg1, arg2, recursiveProxy)
        {
            RecursiveProxy = recursiveProxy;
        }
    }
    public record Recursive<RArg1, RArg2, RArg3, ROut> : Token.Alias.ThreeArg<RArg1, RArg2, RArg3, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where RArg3 : class, ResObj
        where ROut : class, ResObj
    {
        public readonly Proxy.IProxy<Recursive<RArg1, RArg2, RArg3, ROut>, ROut> RecursiveProxy;
        public Recursive(IToken<RArg1> arg1, IToken<RArg2> arg2, IToken<RArg3> arg3, Proxy.IProxy<Recursive<RArg1, RArg2, RArg3, ROut>, ROut> recursiveProxy) : base(arg1, arg2, arg3, recursiveProxy)
        {
            RecursiveProxy = recursiveProxy;
        }
    }

    public record IfElse<R> : Function<r.Bool, R> where R : class, ResObj
    {
        // HACK: this should in theory be (condition.Resolve()) ? Pass.IsFallible : Fail.IsFallible, but we obv cant resolve condition here.
        public override bool IsFallibleFunction => false;
        public IToken<R> Pass { get; init; }
        public IToken<R> Fail { get; init; }
        public IfElse(IToken<r.Bool> condition) : base(condition)
        {
            _passedLastResolution = new None<bool>();
        }
        protected override async ITask<IOption<R>?> Evaluate(IProgram program, IOption<r.Bool> in1)
        {
            if (in1.CheckNone(out var condition)) return new None<R>();
            _passedLastResolution = condition.IsTrue.AsSome();
            return await ((condition.IsTrue) ? Pass : Fail).ResolveWithRules(program);
        }
        //unused
        private IOption<bool> _passedLastResolution;
    }
    public sealed record Variable<R> : Token<r.DeclareVariable<R>> where R : class, ResObj
    {
        public Variable(VariableIdentifier<R> identifier, IToken<R> token)
        {
            _objectToken = token;
            _identifier = identifier;
        }
        public override bool IsFallible => _objectToken.IsFallible;
        protected override async ITask<IOption<r.DeclareVariable<R>>?> ResolveInternal(IProgram program)
        {
            return (await _objectToken.ResolveWithRules(program) is IOption<R> resOpt) ?
                new r.DeclareVariable<R>(_identifier) { Object = resOpt }.AsSome() : null;
        }

        private readonly IToken<R> _objectToken;
        private readonly VariableIdentifier<R> _identifier;
    }

    public sealed record Rule<R> : Infallible<r.DeclareRule> where R : class, ResObj
    {
        public Rule(Rule.IRule rule)
        {
            _rule = rule;
        }

        protected override IOption<r.DeclareRule> InfallibleResolve(IProgram program)
        {
            return new r.DeclareRule() { Rule = _rule }.AsSome();
        }

        private readonly Rule.IRule _rule;
    }
    public sealed record Fixed<R> : Infallible<R> where R : class, ResObj
    {
        public Fixed(R resolution)
        {
            _resolution = resolution;
        }
        protected override IOption<R> InfallibleResolve(IProgram _) { return _resolution.AsSome(); }
        private readonly R _resolution;
    }
    public sealed record Nolla<R> : Infallible<R> where R : class, ResObj
    {
        public Nolla() { }
        protected override IOption<R> InfallibleResolve(IProgram _) { return new None<R>(); }
    }
    public sealed record Reference<R> : Infallible<R> where R : class, ResObj
    { 
        public Reference(VariableIdentifier<R> toIdentifier) => _toIdentifier = toIdentifier;

        protected override IOption<R> InfallibleResolve(IProgram program)
        {
            return (program.State.Variables[_toIdentifier] is IOption<R> val) ? val :
                throw new Exception($"Reference token resolved to non-existent or wrongly-typed object.\n" +
                $"Identifier: {_toIdentifier}\n" +
                $"Expected: {typeof(R).Name}\n" +
                $"Recieved: {program.State.Variables[_toIdentifier]}\n" +
                $"Current Scope:\n" +
                $"{program.State.Variables.Elements.AccumulateInto("", (msg, x) => msg + $"> '{x.key}' : {x.val}\n")}");
        }

        private readonly VariableIdentifier<R> _toIdentifier;
    }
}