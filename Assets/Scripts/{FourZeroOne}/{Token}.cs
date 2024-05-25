
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
#nullable enable
namespace FourZeroOne.Token
{
    using ResObj = Resolution.IResolution;
    using Program;
    public interface IToken<out R> : Unsafe.IToken where R : class, ResObj
    {
        public ITask<IOption<R>> ResolveWithRules(IProgram program);
        public ITask<IOption<R>> Resolve(IProgram program);
    }
    
    public sealed record VariableIdentifier<R> : Unsafe.VariableIdentifier where R : class, ResObj
    {
        public VariableIdentifier() : base() { }
        public override string ToString()
        {
            return $"[&{typeof(R).Name}:{_value}]";
        }
    }
    public abstract record Token<R> : IToken<R> where R : class, ResObj
    {
        public abstract bool IsFallible { get; }
        public async ITask<IOption<R>> Resolve(IProgram program)
        {
            program.ObserveToken(this);
            var o = await ResolveInternal(program);
            program.ObserveResolution(o);
            return o;
        }
        public async ITask<IOption<R>> ResolveWithRules(IProgram program)
        {
            if (program.GetState().Rules.Count == 0) return await Resolve(program);
            var resolvingToken = this.ApplyRules(program.GetState().Rules.Elements, out var applied);
            program.ObserveRuleSteps(applied.Map(x => ((Unsafe.IToken)x.fromToken, x.rule)));
            return await resolvingToken.Resolve(program);
            
            //put in 'ObserveRuleSteps' implementation
            /*
            return await resolvingToken.Resolve(program with
            {
                dState = Q => Q with
                {
                    dRules = Q => Q with { dElements = Q => Q.Filter(x => !applied.HasMatch(y => ReferenceEquals(x, y.rule))) }
                }
            });
            */
        }
        public async ITask<IOption<ResObj>> ResolveUnsafe(IProgram program) { return await ResolveInternal(program); }
        public async ITask<IOption<ResObj>> ResolveWithRulesUnsafe(IProgram program) { return await ResolveWithRules(program); }

        protected abstract ITask<IOption<R>> ResolveInternal(IProgram program);
    }

    public abstract record Infallible<R> : Token<R> where R : class, ResObj
    {
        public sealed override bool IsFallible => false;
        protected sealed override ITask<IOption<R>> ResolveInternal(IProgram program) { return Task.FromResult(InfallibleResolve(program)).AsITask(); }

        protected abstract IOption<R> InfallibleResolve(IProgram program);
    }

    // IMPORTANT: tokens now MUST be absolutely pure stateless in order for this to work.
    // 'Lambda' is resolved multiple times as the same object.
    public abstract record Accumulator<RElement, RGen, RInto> : Unsafe.TokenFunction<RInto>
        where RElement : class, ResObj
        where RGen : class, ResObj
        where RInto : class, ResObj
    {
        public override bool IsFallibleFunction => _lambda.IsFallible;

        protected abstract ITask<IOption<RInto>?> Accumulate(IProgram program, IEnumerable<(RElement element, RGen output)> outputs);
        protected Accumulator(IToken<Resolution.IMulti<RElement>> iterator, VariableIdentifier<RElement> elementVariable, IToken<RGen> lambda) : base(iterator)
        {
            _elementIdentifier = elementVariable;
            _lambda = lambda;
        }
        protected override async ITask<IOption<RInto>?> TransformTokens(IProgram program, IOption<ResObj>[] resolutions)
        {
            if (resolutions[0].CheckNone(out var multi)) return new None<RInto>();
            var iterValues = ((Resolution.IMulti<RElement>)multi).Values;
            var generatorTokens = iterValues
                .Map(x => new Core.Tokens.SubEnvironment<Core.Resolutions.Multi<RGen>>(new Core.Tokens.Variable<RElement>(_elementIdentifier, new Core.Tokens.Fixed<RElement>(x)))
                {
                    SubToken = new Core.Tokens.Multi.Yield<RGen>(_lambda)
                });
            var union = new Core.Tokens.Multi.Union<RGen>(generatorTokens);
            var tryGenerate = await union.Resolve(program);
            while (tryGenerate is IOption<Resolution.IMulti<RGen>> generatorOutOption)
            {
                if (generatorOutOption.CheckNone(out var generatorOutputs)) return new None<RInto>();
                if (await Accumulate(program, iterValues.ZipShort(generatorOutputs.Values)) is IOption<RInto> o) return o;
                tryGenerate = await union.Resolve(program);
            }
            return null;
        }
        private readonly VariableIdentifier<RElement> _elementIdentifier;
        private readonly IToken<RGen> _lambda;
    }
    public abstract record PureAccumulator<RElement, RGen, RInto> : Accumulator<RElement, RGen, RInto>
        where RElement : class, ResObj
        where RGen : class, ResObj
        where RInto : class, ResObj
    {
        protected abstract RInto PureAccumulate(IEnumerable<(RElement element, RGen output)> outputs);
        protected PureAccumulator(IToken<Resolution.IMulti<RElement>> iterator, VariableIdentifier<RElement> elementVariable, IToken<RGen> lambda) : base(iterator, elementVariable, lambda) { }
        protected override ITask<IOption<RInto>?> Accumulate(IProgram _, IEnumerable<(RElement element, RGen output)> outputs) { return Task.FromResult(PureAccumulate(outputs).AsSome()).AsITask(); }
        
    }

    #region Functions
        // ---- [ Functions ] ----

    public interface IFunction<RArg1, ROut> : Unsafe.IHasArg1<RArg1>, Unsafe.IFunction<ROut>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    { }
    public interface IFunction<RArg1, RArg2, ROut> : Unsafe.IHasArg1<RArg1>, Unsafe.IHasArg2<RArg2>, Unsafe.IFunction<ROut>
    where RArg1 : class, ResObj
    where RArg2 : class, ResObj
    where ROut : class, ResObj
    { }
    public interface IFunction<RArg1, RArg2, RArg3, ROut> : Unsafe.IHasArg1<RArg1>, Unsafe.IHasArg2<RArg2>, Unsafe.IHasArg3<RArg3>, Unsafe.IFunction<ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where RArg3 : class, ResObj
        where ROut : class, ResObj
    { }
    public interface ICombiner<RArgs, ROut> : Unsafe.IHasCombinerArgs<RArgs>, Unsafe.IFunction<ROut>
    where RArgs : class, ResObj
    where ROut : class, ResObj
    { }
    /// <summary>
    /// Core.Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    public abstract record Function<RArg1, ROut> : Unsafe.TokenFunction<ROut>,
        IFunction<RArg1, ROut>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    {
        public IToken<RArg1> Arg1 => (IToken<RArg1>)ArgTokens[0];

        protected Function(IToken<RArg1> in1) : base(in1) { }
        protected abstract ITask<IOption<ROut>?> Evaluate(IProgram program, IOption<RArg1> in1);
        protected override ITask<IOption<ROut>?> TransformTokens(IProgram program, IOption<ResObj>[] args)
        {
            return Evaluate(program, args[0].RemapAs(x => (RArg1)x));
        }
    }

    /// <summary>
    /// Core.Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;, IToken&lt;<typeparamref name="RArg2"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    /// <typeparam name="RArg2"></typeparam>
    public abstract record Function<RArg1, RArg2, ROut> : Unsafe.TokenFunction<ROut>,
        IFunction<RArg1, RArg2, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where ROut : class, ResObj
    {
        public IToken<RArg1> Arg1 => (IToken<RArg1>)ArgTokens[0];
        public IToken<RArg2> Arg2 => (IToken<RArg2>)ArgTokens[1];

        protected abstract ITask<IOption<ROut>?> Evaluate(IProgram program, IOption<RArg1> in1, IOption<RArg2> in2);
        protected Function(IToken<RArg1> in1, IToken<RArg2> in2) : base(in1, in2) { }
        protected override ITask<IOption<ROut>?> TransformTokens(IProgram program, IOption<ResObj>[] args)
        {
            return Evaluate(program, args[0].RemapAs(x => (RArg1)x), args[1].RemapAs(x => (RArg2)x));
        }
    }

    /// <summary>
    /// Core.Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IToken&lt;<typeparamref name="RArg1"/>&gt;, IToken&lt;<typeparamref name="RArg2"/>&gt;, IToken&lt;<typeparamref name="RArg3"/>&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg1"></typeparam>
    /// <typeparam name="RArg2"></typeparam>
    /// <typeparam name="RArg3"></typeparam>
    public abstract record Function<RArg1, RArg2, RArg3, ROut> : Unsafe.TokenFunction<ROut>,
        IFunction<RArg1, RArg2, RArg3, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where RArg3 : class, ResObj
        where ROut : class, ResObj
    {
        public IToken<RArg1> Arg1 => (IToken<RArg1>)ArgTokens[0];
        public IToken<RArg2> Arg2 => (IToken<RArg2>)ArgTokens[1];
        public IToken<RArg3> Arg3 => (IToken<RArg3>)ArgTokens[2];

        protected abstract ITask<IOption<ROut>?> Evaluate(IProgram program, IOption<RArg1> in1, IOption<RArg2> in2, IOption<RArg3> in3);
        protected Function(IToken<RArg1> in1, IToken<RArg2> in2, IToken<RArg3> in3) : base(in1, in2, in3) { }
        protected override ITask<IOption<ROut>?> TransformTokens(IProgram program, IOption<ResObj>[] args)
        {
            return Evaluate(program, args[0].RemapAs(x => (RArg1)x), args[1].RemapAs(x => (RArg2)x), args[2].RemapAs(x => (RArg3)x));
        }
    }

    /// <summary>
    /// Core.Tokens that inherit must have a constructor matching: <br></br>
    /// <code>(IEnumerable&lt;IToken&lt;<typeparamref name="RArg"/>&gt;&gt;)</code>
    /// </summary>
    /// <typeparam name="RArg"></typeparam>
    public abstract record Combiner<RArg, ROut> : Unsafe.TokenFunction<ROut>, ICombiner<RArg, ROut>
        where RArg : class, ResObj
        where ROut : class, ResObj
    {
        public IEnumerable<IToken<RArg>> Args => ArgTokens.Elements.Map(x => (IToken<RArg>)x);

        protected abstract ITask<IOption<ROut>?> Evaluate(IProgram program, IEnumerable<IOption<RArg>> inputs);
        protected Combiner(IEnumerable<IToken<RArg>> tokens) : base(tokens) { }
        protected sealed override ITask<IOption<ROut>?> TransformTokens(IProgram program, IOption<ResObj>[] tokens)
        {
            return Evaluate(program, tokens.Map(x => x.RemapAs(x => (RArg)x)));
        }
    }
    #region Pure Functions
    // -- [ Pure Functions ] --
    public abstract record PureFunction<RArg1, ROut> : Function<RArg1, ROut>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    {
        public sealed override bool IsFallibleFunction => false;

        protected abstract ROut EvaluatePure(RArg1 in1);
        protected PureFunction(IToken<RArg1> in1) : base(in1) { }
        protected sealed override ITask<IOption<ROut>?> Evaluate(IProgram _, IOption<RArg1> in1)
        {
            IOption<ROut> o = (in1.CheckNone(out var a)) ? new None<ROut>() :
                EvaluatePure(a).AsSome();
            return Task.FromResult(o).AsITask();
        }
    }
    public abstract record PureFunction<RArg1, RArg2, ROut> : Function<RArg1, RArg2, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where ROut : class, ResObj
    {
        public sealed override bool IsFallibleFunction => false;

        protected abstract ROut EvaluatePure(RArg1 in1, RArg2 in2);
        protected PureFunction(IToken<RArg1> in1, IToken<RArg2> in2) : base(in1, in2) { }
        protected sealed override ITask<IOption<ROut>?> Evaluate(IProgram _, IOption<RArg1> in1, IOption<RArg2> in2)
        {
            IOption<ROut> o = (in1.CheckNone(out var a) || in2.CheckNone(out var b)) ? new None<ROut>() :
                EvaluatePure(a, b).AsSome();
            return Task.FromResult(o).AsITask();
        }
    }
    public abstract record PureFunction<RArg1, RArg2, RArg3, ROut> : Function<RArg1, RArg2, RArg3, ROut>
        where RArg1 : class, ResObj
        where RArg2 : class, ResObj
        where RArg3 : class, ResObj
        where ROut : class, ResObj
    {
        public sealed override bool IsFallibleFunction => false;

        protected abstract ROut EvaluatePure(RArg1 in1, RArg2 in2, RArg3 in3);
        protected PureFunction(IToken<RArg1> in1, IToken<RArg2> in2, IToken<RArg3> in3) : base(in1, in2, in3) { }
        protected sealed override ITask<IOption<ROut>?> Evaluate(IProgram _, IOption<RArg1> in1, IOption<RArg2> in2, IOption<RArg3> in3)
        {
            IOption<ROut> o = (in1.CheckNone(out var a) || in2.CheckNone(out var b) || in3.CheckNone(out var c)) ? new None<ROut>() :
                EvaluatePure(a, b, c).AsSome();
            return Task.FromResult(o).AsITask();
        }
    }
    public abstract record PureCombiner<RArg, ROut> : Combiner<RArg, ROut>
        where RArg : class, ResObj
        where ROut : class, ResObj
    {
        public sealed override bool IsFallibleFunction => false;

        protected abstract ROut EvaluatePure(IEnumerable<RArg> inputs);
        protected PureCombiner(IEnumerable<IToken<RArg>> tokens) : base(tokens) { }
        protected sealed override ITask<IOption<ROut>?> Evaluate(IProgram _, IEnumerable<IOption<RArg>> inputs) => Task.FromResult(EvaluatePure(inputs.Filter(x => x.IsSome()).Map(x => x.Unwrap())).AsSome()).AsITask();
    }
    // ----
    #endregion
    // --------
    #endregion

    public abstract record PresentStateGetter<RSource> : Function<RSource, RSource>
        where RSource : class, Resolution.IStateTracked
    {
        public sealed override bool IsFallibleFunction => false;
        public PresentStateGetter(IToken<RSource> source) : base(source) { }
        protected abstract PIndexedSet<int, RSource> GetStatePSet(IProgram program);
        protected sealed override ITask<IOption<RSource>?> Evaluate(IProgram program, IOption<RSource> in1) { return Task.FromResult(in1.RemapAs(x => GetStatePSet(program)[x.UUID])).AsITask(); }
    }

    public static class _Extensions
    {
        public static IToken<R> ApplyRules<R>(this IToken<R> token, IEnumerable<Rule.IRule> rules, out List<(IToken<R> fromToken, Rule.IRule rule)> appliedRules) where R : class, ResObj
        {
            var o = token;
            appliedRules = new();
            foreach (var rule in rules)
            {
                if (rule.TryApplyTyped(o) is IToken<R> newToken)
                {
                    appliedRules.Add((o, rule));
                    o = newToken;
                }
            }
            return o;
        }
        public static IToken<R> ApplyRule<R>(this IToken<R> token, Rule.IRule? rule) where R : class, ResObj
        {
            return (rule is not null) ? token.ApplyRules(rule.Yield(), out var _) : token;
        }

    }
}
namespace FourZeroOne.Token.Unsafe
{
    using ResObj = Resolution.IResolution;
    using Token;
    using Program;
    public interface IToken
    {
        public bool IsFallible { get; }
        public ITask<IOption<ResObj>> ResolveWithRulesUnsafe(IProgram program);
        public ITask<IOption<ResObj>> ResolveUnsafe(IProgram program);
    }

    public interface IHasArg1<RArg> : Unsafe.IHasArg1 where RArg : class, ResObj
    { public IToken<RArg> Arg1 { get; } }
    public interface IHasArg2<RArg> : Unsafe.IHasArg2 where RArg : class, ResObj
    { public IToken<RArg> Arg2 { get; } }
    public interface IHasArg3<RArg> : Unsafe.IHasArg3 where RArg : class, ResObj
    { public IToken<RArg> Arg3 { get; } }
    public interface IHasCombinerArgs<RArgs> : IToken where RArgs : class, ResObj
    {
        public IEnumerable<IToken<RArgs>> Args { get; }
    }

    // shitty name for this interface-set. consider something like 'FromArgs', 'ProductOfArgs', 'ArgTransformer', or something.
    public interface IFunction<ROut> : IFunction, IToken<ROut> where ROut : class, ResObj { }
    public interface IFunction : IToken { }

    public interface IHasArg1 : IToken { }
    public interface IHasArg2 : IHasArg1 { }
    public interface IHasArg3 : IHasArg2 { }

    public abstract record VariableIdentifier
    {
        public VariableIdentifier()
        {
            _value = _assigner;
            _assigner++;
        }
        public virtual bool Equals(VariableIdentifier? other) => other is not null && _value == other._value;
        public override int GetHashCode() => _value.GetHashCode();
        protected static int _assigner = 0;
        protected readonly int _value;
    }
    public abstract record TokenFunction<R> : Token<R> where R : class, ResObj
    {
        public abstract bool IsFallibleFunction { get; }
        public sealed override bool IsFallible => IsFallibleFunction || ArgTokens.Elements.Map(x => x.IsFallible).HasMatch(x => x == true);
        protected sealed override async ITask<IOption<R>> ResolveInternal(IProgram program)
        {
            throw new System.NotImplementedException();
        }

        protected readonly PList<IToken> ArgTokens;
        protected abstract ITask<IOption<R>?> TransformTokens(IProgram program, IOption<ResObj>[] resolutions);
        protected TokenFunction(IEnumerable<IToken> tokens)
        {
            ArgTokens = new() { Elements = tokens };
        }
        protected TokenFunction(params IToken[] tokens) : this(tokens as IEnumerable<IToken>) { }
    }

}