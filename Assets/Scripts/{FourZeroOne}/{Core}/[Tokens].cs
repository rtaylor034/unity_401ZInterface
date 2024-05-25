using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Perfection;
using MorseCode.ITask;
using FourZeroOne;

#nullable enable
namespace FourZeroOne.Core.Tokens
{
    using Token;
    using ResObj = Resolution.IResolution;
    using r = Resolutions;
    using FourZeroOne.Core.Resolutions.Board;
    using Program;

    namespace Board
    {
        using rb = Resolutions.Board;
        namespace Coordinates
        {
            public sealed record Of : PureFunction<Resolution.Board.IPositioned, rb.Coordinates>
            {
                public Of(IToken<Resolution.Board.IPositioned> of) : base(of) { }
                protected override rb.Coordinates EvaluatePure(Resolution.Board.IPositioned in1)
                {
                    return in1.Position;
                }
            }
            public sealed record OffsetArea : PureFunction<rb.Coordinates, Resolution.IMulti<rb.Coordinates>, r.Multi<rb.Coordinates>>
            {
                public OffsetArea(IToken<rb.Coordinates> offset, IToken<Resolution.IMulti<rb.Coordinates>> area) : base(offset, area) { }
                protected override r.Multi<rb.Coordinates> EvaluatePure(rb.Coordinates in1, Resolution.IMulti<rb.Coordinates> in2)
                {
                    return new() { Values = in2.Values.Map(x => x.Add(in1)) };
                }
            }
        }
        namespace Hex
        {
            public sealed record AllHexes : Infallible<r.Multi<rb.Hex>>
            {
                protected override IOption<r.Multi<rb.Hex>> InfallibleResolve(IProgram program)
                {
                    return new r.Multi<rb.Hex>() { Values = program.GetState().Board.Hexes }.AsSome();
                }
            }
            public sealed record AtPresent : PresentStateGetter<rb.Hex>
            {
                public AtPresent(IToken<rb.Hex> source) : base(source) { }
                protected override PIndexedSet<int, rb.Hex> GetStatePSet(IProgram program) { return program.GetState().Board.Hexes; }
            }
            namespace Get
            {

            }
        }
        namespace Unit
        {
            public sealed record AllUnits : Infallible<r.Multi<rb.Unit>>
            {
                protected override IOption<r.Multi<rb.Unit>> InfallibleResolve(IProgram program)
                {
                    return new r.Multi<rb.Unit>() { Values = program.GetState().Board.Units }.AsSome();
                }
            }
            public sealed record AtPresent : PresentStateGetter<rb.Unit>
            {
                public AtPresent(IToken<rb.Unit> source) : base(source) { }
                protected override PIndexedSet<int, rb.Unit> GetStatePSet(IProgram program) { return program.GetState().Board.Units; }
            }
            namespace Get
            {
                public sealed record HP : PureFunction<rb.Unit, r.Number>
                {
                    public HP(IToken<rb.Unit> of) : base(of) { }
                    protected override r.Number EvaluatePure(rb.Unit in1) { return in1.HP; }
                }
                public sealed record Effects : PureFunction<rb.Unit, r.Multi<rb.Unit.Effect>>
                {
                    public Effects(IToken<rb.Unit> of) : base(of) { }
                    protected override r.Multi<rb.Unit.Effect> EvaluatePure(rb.Unit in1) { return in1.Effects; }

                }
                public sealed record Owner : PureFunction<rb.Unit, rb.Player>
                {
                    public Owner(IToken<rb.Unit> source) : base(source) { }
                    protected override rb.Player EvaluatePure(rb.Unit in1) { return in1.Owner; }
                }
            }
        }
        namespace Player
        {
            public sealed record AllPlayers : Infallible<r.Multi<rb.Player>>
            {
                protected override IOption<r.Multi<rb.Player>> InfallibleResolve(IProgram program)
                {
                    return new r.Multi<rb.Player>() { Values = program.GetState().Board.Players }.AsSome();
                }
            }
            public sealed record AtPresent : PresentStateGetter<rb.Player>
            {
                public AtPresent(IToken<rb.Player> source) : base(source) { }
                protected override PIndexedSet<int, rb.Player> GetStatePSet(IProgram program) { return program.GetState().Board.Players; }
            }
        }
    }
    namespace IO
    {
        namespace Select
        {
            public sealed record One<R> : Function<Resolution.IMulti<R>, R> where R : class, ResObj
            {
                public override bool IsFallibleFunction => true;
                public One(IToken<Resolution.IMulti<R>> from) : base(from) { }

                protected async override ITask<IOption<R>?> Evaluate(IProgram program, IOption<Resolution.IMulti<R>> fromOpt)
                {
                    if (fromOpt.CheckNone(out var from)) return new None<R>();
                    if (await program.Input.ReadSelection(from.Values, 1) is not IOption<IEnumerable<R>> selOpt) return null;
                    return (selOpt.Check(out var sel)) ? sel.First()?.AsSome() : new None<R>();
                }
            }

            //make range instead of single int count
            public sealed record Multiple<R> : Function<Resolution.IMulti<R>, r.Number, r.Multi<R>> where R : class, ResObj
            {
                public override bool IsFallibleFunction => true;
                public Multiple(IToken<Resolution.IMulti<R>> from, IToken<r.Number> count) : base(from, count) { }

                protected override async ITask<IOption<r.Multi<R>>?> Evaluate(IProgram program, IOption<Resolution.IMulti<R>> fromOpt, IOption<r.Number> countOpt)
                {
                    if (fromOpt.CheckNone(out var from) || countOpt.CheckNone(out var count)) return new None<r.Multi<R>>();
                    if (await program.Input.ReadSelection(from.Values, count.Value) is not IOption<IEnumerable<R>> selOpt) return null;
                    return selOpt.RemapAs(v => new r.Multi<R>() { Values = v });
                }
            }
        }
    }
    namespace Number
    {
        public sealed record Add : PureFunction<r.Number, r.Number, r.Number>
        {
            public Add(IToken<r.Number> operand1, IToken<r.Number> operand2) : base(operand1, operand2) { }
            protected override r.Number EvaluatePure(r.Number a, r.Number b) { return new() { Value = a.Value + b.Value }; }
        }

        public sealed record Subtract : PureFunction<r.Number, r.Number, r.Number>
        {
            public Subtract(IToken<r.Number> operand1, IToken<r.Number> operand2) : base(operand1, operand2) { }
            protected override r.Number EvaluatePure(r.Number a, r.Number b) { return new() { Value = a.Value - b.Value }; }
        }

        public sealed record Multiply : PureFunction<r.Number, r.Number, r.Number>
        {
            public Multiply(IToken<r.Number> operand1, IToken<r.Number> operand2) : base(operand1, operand2) { }
            protected override r.Number EvaluatePure(r.Number a, r.Number b) { return new() { Value = a.Value * b.Value }; }
        }

        public sealed record Negate : PureFunction<r.Number, r.Number>
        {
            public Negate(IToken<r.Number> operand) : base(operand) { }
            protected override r.Number EvaluatePure(r.Number operand) { return new() { Value = -operand.Value }; }
        }
        namespace Compare
        {
            public sealed record GreaterThan : PureFunction<r.Number, r.Number, r.Bool>
            {
                public GreaterThan(IToken<r.Number> a, IToken<r.Number> b) : base(a, b) { }
                protected override r.Bool EvaluatePure(r.Number in1, r.Number in2)
                {
                    return new() { IsTrue = in1.Value > in2.Value };
                }
            }
        }
    }
    namespace Multi
    {
        
        public sealed record Union<R> : PureCombiner<Resolution.IMulti<R>, r.Multi<R>> where R : class, ResObj
        {
            public Union(IEnumerable<IToken<Resolution.IMulti<R>>> elements) : base(elements) { }
            public Union(params IToken<Resolution.IMulti<R>>[] elements) : base(elements) { }
            protected override r.Multi<R> EvaluatePure(IEnumerable<Resolution.IMulti<R>> inputs)
            {
                return new() { Values = inputs.Map(x => x.Values).Flatten() };
            }
        }

        public sealed record Intersection<R> : PureCombiner<Resolution.IMulti<R>, r.Multi<R>> where R : class, ResObj
        {
            public Intersection(IEnumerable<IToken<Resolution.IMulti<R>>> sets) : base(sets) { }
            public Intersection(params IToken<Resolution.IMulti<R>>[] sets) : base(sets) { }
            protected override r.Multi<R> EvaluatePure(IEnumerable<Resolution.IMulti<R>> inputs)
            {
                var iter = inputs.GetEnumerator();
                if (!iter.MoveNext()) return new();
                var o = iter.Current.Values;
                while (iter.MoveNext())
                {
                    o = o.Filter(x => iter.Current.Values.HasMatch(y => x.Equals(y)));
                }
                return new() { Values = o };
            }
        }

        public sealed record Exclusion<R> : PureFunction<Resolution.IMulti<R>, Resolution.IMulti<R>, r.Multi<R>> where R : class, ResObj
        {
            public Exclusion(IToken<Resolution.IMulti<R>> from, IToken<Resolution.IMulti<R>> exclude) : base(from, exclude) { }
            protected override r.Multi<R> EvaluatePure(Resolution.IMulti<R> in1, Resolution.IMulti<R> in2)
            {
                return new() { Values = in1.Values.Filter(x => !in2.Values.HasMatch(y => y.ResEqual(x))) };
            }
        }

        public sealed record Yield<R> : PureFunction<R, r.Multi<R>> where R : class, ResObj
        {
            public Yield(IToken<R> value) : base(value) { }
            protected override r.Multi<R> EvaluatePure(R in1)
            {
                return new() { Values = in1.Yield() };
            }
        }

        public sealed record Filtered<R> : PureAccumulator<R, r.Bool, r.Multi<R>> where R : class, ResObj
        {
            public Filtered(IToken<Resolution.IMulti<R>> iterator, VariableIdentifier<R> elementVariable, IToken<r.Bool> lambda) : base(iterator, elementVariable, lambda) { }
            protected override r.Multi<R> PureAccumulate(IEnumerable<(R element, r.Bool output)> outputs)
            {
                return new() { Values = outputs.Filter(x => x.output.IsTrue).Map(x => x.element) };
            }
        }

        public sealed record Count : PureFunction<Resolution.IMulti<ResObj>, r.Number>
        {
            public Count(IToken<Resolution.IMulti<ResObj>> of) : base(of) { }
            protected override r.Number EvaluatePure(Resolution.IMulti<ResObj> in1)
            {
                return new() { Value = in1.Count };
            }
        }
    }
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

    public record Recursive<RArg1, ROut> : Macro.OneArg<RArg1, ROut>
        where RArg1 : class, ResObj
        where ROut : class, ResObj
    {
        public readonly Proxy.IProxy<Recursive<RArg1, ROut>, ROut> RecursiveProxy;
        public Recursive(IToken<RArg1> arg1, Proxy.IProxy<Recursive<RArg1, ROut>, ROut> recursiveProxy) : base(arg1, recursiveProxy)
        {
            RecursiveProxy = recursiveProxy;
        }
    }
    public record Recursive<RArg1, RArg2, ROut> : Macro.TwoArg<RArg1, RArg2, ROut>
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
    public record Recursive<RArg1, RArg2, RArg3, ROut> : Macro.ThreeArg<RArg1, RArg2, RArg3, ROut>
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
            return (program.GetState().Variables[_toIdentifier] is IOption<R> val) ? val :
                throw new Exception($"Reference token resolved to non-existent or wrongly-typed object.\n" +
                $"Identifier: {_toIdentifier}\n" +
                $"Expected: {typeof(R).Name}\n" +
                $"Recieved: {program.GetState().Variables[_toIdentifier]}\n" +
                $"Current Scope:\n" +
                $"{program.GetState().Variables.Elements.AccumulateInto("", (msg, x) => msg + $"> '{x.key}' : {x.val}\n")}");
        }

        private readonly VariableIdentifier<R> _toIdentifier;
    }
}