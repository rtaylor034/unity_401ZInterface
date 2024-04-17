using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
using ResObj = Resolution.IResolution;
using Token;
using r_ = Resolutions;
using Resolutions;

#nullable enable
namespace Tokens
{

    public sealed record SubEnvironment<R> : Token.Unsafe.TokenFunction<R> where R : class, ResObj
    {
        public IToken<R> SubToken { get; init; }
        public sealed override bool IsFallibleFunction => SubToken.IsFallible;
        public SubEnvironment(params IToken<Resolution.Operation>[] envModifiers) : base(envModifiers) { }
        public SubEnvironment(IEnumerable<Token.Unsafe.IToken> envModifiers) : base(envModifiers) { }
        protected sealed override async ITask<R?> TransformTokens(Context context, List<ResObj> _) => await SubToken.ResolveWithRules(context);
    }
    public sealed record Variable<R> : Token<r_.DeclareVariable> where R : class, ResObj
    {
        public override bool IsFallible => _objectToken.IsFallible;
        private readonly IToken<R> _objectToken;
        private readonly string _label;
        public Variable(string label, IToken<R> token)
        {
            _objectToken = token;
            _label = label;
        }
        public override async ITask<r_.DeclareVariable?> Resolve(Context context)
        {
            return (await _objectToken.ResolveWithRules(context) is r_.DeclareVariable res) ?
                new() { Label = _label, Object = res } : null;
        }
    }
    public sealed record Rule<R> : Infallible<r_.DeclareRule> where R : class, ResObj
    {
        private readonly Rule.IRule _rule;
        public Rule(Rule.IRule rule) => _rule = rule;
        protected override DeclareRule InfallibleResolve(Context context) => new() { Rule = _rule };
    }
    public sealed record Reference<R> : Infallible<R> where R : class, ResObj
    {
        private readonly string _toLabel;
        public Reference(string toLabel) => _toLabel = toLabel;
        protected override R InfallibleResolve(Context context)
        {
            return (context.Variables[_toLabel] is R val) ? val :
                throw new Exception($"Reference token resolved to non-existent or wrongly-typed object.\n" +
                $"Label: '{_toLabel}'\n" +
                $"Expected: {typeof(R).Name}\n" +
                $"Recieved: {context.Variables[_toLabel]?.GetType().Name}\n" +
                $"Current Scope:\n" +
                $"{context.Variables.Elements.AccumulateInto("", (msg, x) => msg + $"> '{x.key}' : {x.val}")}");
        }
    }
}