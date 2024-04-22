using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
using ResObj = Resolution.IResolution;
using Token;
using r_ = Resolutions;

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
        protected sealed override ITask<ROut?> TransformTokens(Context context, List<ResObj> _)
        {
            return SubToken.ResolveWithRules(context);
        }
    }

    public sealed record Variable<R> : Token<r_.DeclareVariable> where R : class, ResObj
    {
        public Variable(string label, IToken<R> token)
        {
            _objectToken = token;
            _label = label;
        }
        public override bool IsFallible => _objectToken.IsFallible;
        public override async ITask<r_.DeclareVariable?> Resolve(Context context)
        {
            return (await _objectToken.ResolveWithRules(context) is R res) ?
                new() { Label = _label, Object = res } : null;
        }

        private readonly IToken<R> _objectToken;
        private readonly string _label;
    }

    public sealed record Rule<R> : Infallible<r_.DeclareRule> where R : class, ResObj
    {
        public Rule(Rule.IRule rule)
        {
            _rule = rule;
        }

        protected override r_.DeclareRule InfallibleResolve(Context context) { return new() { Rule = _rule }; }

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
        protected override R InfallibleResolve(Context _) { return _resolution; }
        private readonly R _resolution;
    }
    public sealed record Reference<R> : Infallible<R> where R : class, ResObj
    {
        public Reference(string toLabel) => _toLabel = toLabel;

        protected override R InfallibleResolve(Context context)
        {
            return (context.State.Variables[_toLabel] is R val) ? val :
                throw new Exception($"Reference token resolved to non-existent or wrongly-typed object.\n" +
                $"Label: '{_toLabel}'\n" +
                $"Expected: {typeof(R).Name}\n" +
                $"Recieved: {context.State.Variables[_toLabel]?.GetType().Name}\n" +
                $"Current Scope:\n" +
                $"{context.State.Variables.Elements.AccumulateInto("", (msg, x) => msg + $"> '{x.key}' : {x.val}")}");
        }

        private readonly string _toLabel;
    }
}