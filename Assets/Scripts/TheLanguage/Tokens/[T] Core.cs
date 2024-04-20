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

    public sealed record Scope<R> : SubEnvironment<r_.DeclareVariable, R> where R : class, ResObj
    {
        public Scope(IEnumerable<IToken<r_.DeclareVariable>> variables) : base(variables) { }
        public Scope(params IToken<r_.DeclareVariable>[] variables) : base(variables) { }
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

    public sealed record Reference<R> : Infallible<R> where R : class, ResObj
    {
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

        private readonly string _toLabel;
    }
}