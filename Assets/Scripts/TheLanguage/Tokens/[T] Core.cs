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
        protected sealed override ITask<IOption<ROut>?> TransformTokens(IProgram program, List<IOption<ResObj>> _)
        {
            return SubToken.ResolveWithRules(program);
        }
    }

    public sealed record Variable<R> : Token<r.DeclareVariable> where R : class, ResObj
    {
        public Variable(string label, IToken<R> token)
        {
            _objectToken = token;
            _label = label;
        }
        public override bool IsFallible => _objectToken.IsFallible;
        protected override async ITask<IOption<r.DeclareVariable>?> ResolveInternal(IProgram program)
        {
            return (await _objectToken.ResolveWithRules(program) is IOption<R> resOpt) ?
                new r.DeclareVariable() { Label = _label, Object = resOpt }.AsSome() : null;
        }

        private readonly IToken<R> _objectToken;
        private readonly string _label;
    }

    public sealed record Rule<R> : Infallible<r.DeclareRule> where R : class, ResObj
    {
        public Rule(Rule.IRule rule)
        {
            _rule = rule;
        }

        protected override IOption<r.DeclareRule> InfallibleResolve(IProgram program) { return new r.DeclareRule() { Rule = _rule }.AsSome(); }

        private readonly Rule.IRule _rule;
    }
    public sealed record Fixed<R> : Infallible<R> where R : class, ResObj
    {
        public Fixed(R resolution)
        {
            _resolution = resolution;
        }
        // kinda spicy
        public static implicit operator Fixed<R>(R resolution) => new(resolution);
        protected override IOption<R> InfallibleResolve(IProgram _) { return _resolution.AsSome(); }
        private readonly R _resolution;
    }
    public sealed record Reference<R> : Infallible<R> where R : class, ResObj
    {
        public Reference(string toLabel) => _toLabel = toLabel;

        protected override IOption<R> InfallibleResolve(IProgram program)
        {
            return (program.State.Variables[_toLabel] is IOption<R> val) ? val :
                throw new Exception($"Reference token resolved to non-existent or wrongly-typed object.\n" +
                $"Label: '{_toLabel}'\n" +
                $"Expected: {typeof(R).Name}\n" +
                $"Recieved: {program.State.Variables[_toLabel]?.GetType().Name}\n" +
                $"Current Scope:\n" +
                $"{program.State.Variables.Elements.AccumulateInto("", (msg, x) => msg + $"> '{x.key}' : {x.val}")}");
        }

        private readonly string _toLabel;
    }
}