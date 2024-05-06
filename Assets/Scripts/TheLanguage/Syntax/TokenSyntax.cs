using Tokens;
using Token;
using Resolutions;
using Perfection;
using r = Resolutions;
using ResObj = Resolution.IResolution;
namespace TokenSyntax
{


    namespace TokenStructure
    {
        public sealed record IfElse<R> where R : class, ResObj
        {
            public IToken<R> Then { get; init; }
            public IToken<R> Else { get; init; }
        }
        public sealed record SubEnvironment<R> where R : class, ResObj
        {
            public Token.Unsafe.IToken[] Environment { get; init; }
            public IToken<R> SubToken { get; init; }
        }
        public sealed record Arguements<RArg1>
            where RArg1 : class, ResObj
        {
            public IToken<RArg1> A { get; init; }
        }
        public sealed record Arguements<RArg1, RArg2>
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
        {
            public IToken<RArg1> A { get; init; }
            public IToken<RArg2> B { get; init; }
        }
        public sealed record Arguements<RArg1, RArg2, RArg3>
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where RArg3 : class, ResObj
        {
            public IToken<RArg1> A { get; init; }
            public IToken<RArg2> B { get; init; }
            public IToken<RArg3> C { get; init; }
        }
     }
    public static class MakeToken
    {
        public static Tokens.Board.Unit.AllUnits AllUnits()
        { return new(); }
        public static Tokens.Board.Hex.AllHexes AllHexes()
        { return new(); }

        public static SubEnvironment<R> tSubEnvironment<R>(TokenStructure.SubEnvironment<R> block) where R : class, ResObj
        { return new(block.Environment) { SubToken = block.SubToken }; }

        public static Recursive<RArg1, ROut> tRecursive<RArg1, ROut>(IToken<RArg1> a, Proxies.RecursiveCall<RArg1, ROut> recursiveProxy)
            where RArg1 : class, ResObj
            where ROut : class, ResObj
        { return new(a, recursiveProxy); }
        public static Recursive<RArg1, RArg2, ROut> tRecursive<RArg1, RArg2, ROut>(IToken<RArg1> a, IToken<RArg2> b, Proxies.RecursiveCall<RArg1, RArg2, ROut> recursiveProxy)
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where ROut : class, ResObj
        { return new(a, b, recursiveProxy); }
        public static Recursive<RArg1, RArg2, RArg3, ROut> tRecursive<RArg1, RArg2, RArg3, ROut>(IToken<RArg1> a, IToken<RArg2> b, IToken<RArg3> c, Proxies.RecursiveCall<RArg1, RArg2, RArg3, ROut> recursiveProxy)
            where RArg1 : class, ResObj
            where RArg2 : class, ResObj
            where RArg3 : class, ResObj
            where ROut : class, ResObj
        { return new(a, b, c, recursiveProxy); }

        public static Nolla<R> Nolla<R>() where R : class, ResObj
        { return new(); }

        public static Tokens.Multi.Union<R> tArrayOf<R>(IToken<R>[] tokens) where R : class, ResObj
        { return new(tokens.Map(x => x.tYield())); }
        public static Tokens.Multi.Union<R> tUnion<R>(IToken<Multi<R>>[] arrays) where R : class, ResObj
        { return new(arrays); }
        public static Tokens.Multi.Intersection<R> tIntersection<R>(IToken<Multi<R>>[] arrays) where R : class, ResObj
        { return new(arrays); }
    }
    public static class _Extensions
    {
        public static Tokens.Select.One<R> tIO_SelectOne<R>(this IToken<Multi<R>> source) where R : class, ResObj
        { return new(source); }
        public static Tokens.Select.Multiple<R> tIO_SelectMany<R>(this IToken<Multi<R>> source, IToken<Number> count) where R : class, ResObj
        { return new(source, count); }

        public static Variable<R> tAs<R>(this IToken<R> token, out VariableIdentifier<R> ident) where R : class, ResObj
        {
            ident = new();
            return new Variable<R>(ident, token);
        }
        public static Reference<R> tRef<R>(this VariableIdentifier<R> ident) where R : class, ResObj
        { return new(ident); }
        
        public static IfElse<R> tIfTrue<R>(this IToken<Bool> condition, TokenStructure.IfElse<R> block) where R : class, ResObj
        {
            return new(condition)
            {
                Pass = block.Then,
                Fail = block.Else
            };
        }

        public static Tokens.Multi.Exclusion<R> tWithout<R>(this IToken<Multi<R>> source, IToken<Multi<R>> exclude) where R : class, ResObj
        { return new(source, exclude); }
        public static Tokens.Multi.Filtered<R> tDynamicFilter<R>(this IToken<Multi<R>> source, out VariableIdentifier<R> elementIdentifier, IToken<Bool> condition) where R : class, ResObj
        {
            elementIdentifier = new();
            return new(source, elementIdentifier, condition);
        }
        public static Tokens.Multi.Count tCount(this IToken<Resolution.IMulti<ResObj>> source)
        { return new(source); }
        public static Tokens.Multi.Yield<R> tYield<R>(this IToken<R> token) where R : class, ResObj
        { return new(token); }

        public static Tokens.Number.Add tAddTo(this IToken<Number> a, IToken<Number> b) 
        { return new(a, b); }
        public static Tokens.Number.Subtract tSubtractFrom(this IToken<Number> a, IToken<Number> b) 
        { return new(a, b); }
        public static Tokens.Number.Multiply tMultiplyBy(this IToken<Number> a, IToken<Number> b) 
        { return new(a, b); }
        public static Tokens.Number.Negate tNegative(this IToken<Number> a)
        { return new(a); }
        public static Tokens.Number.Compare.GreaterThan tIsGreaterThan(this IToken<Number> a, IToken<Number> b)
        { return new(a, b); }
        public static Fixed<Number> tConst(this int value)
        { return new(value); }

        public static Tokens.Board.Unit.Get.HP tGetHP(this IToken<r.Board.Unit> unit)
        { return new(unit); }
        
        
    }
}