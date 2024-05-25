
using System.Collections.Generic;
using Perfection;

#nullable enable
namespace FourZeroOne
{
    using ResObj = Resolution.IResolution;
    using Token.Unsafe;
    public interface IOutputInterface
    {
        /// <summary>
        /// Called when a token is transformed via rules, <paramref name="pairs"/> holding the transformation steps. <br></br>
        /// <i><see cref="WriteToken(IToken)"/> is only called on the final resulting token.</i>
        /// </summary>
        /// <param name="pairs"></param>
        public void WriteRuleSteps(IEnumerable<(IToken fromToken, Rule.IRule rule)> pairs);
        /// <summary>
        /// Called when <paramref name="token"/> *starts* resolving.<br></br>
        /// <i>Only called on final realization after all rules are applied.</i>
        /// </summary>
        /// <param name="token"></param>
        public void WriteToken(State startingState, IToken token);
        /// <summary>
        /// Called when a token resolves to <paramref name="resolution"/>.
        /// </summary>
        /// <param name="resolution"></param>
        public void WriteResolution(IOption<ResObj> resolution);
    }
}