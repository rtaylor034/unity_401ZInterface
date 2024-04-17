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
        public override bool IsFallible => ObjectToken.IsFallible;
        public readonly IToken<R> ObjectToken;
        public readonly string Label;
        public Variable(string label, IToken<R> token)
        {
            ObjectToken = token;
            Label = label;
        }
        public override async ITask<r_.DeclareVariable?> Resolve(Context context)
        {
            return (await ObjectToken.ResolveWithRules(context) is r_.DeclareVariable res) ?
                new() { Label = Label, Object = res } : null;
        }
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