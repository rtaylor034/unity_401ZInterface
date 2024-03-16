using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Specifications;
using UnityEditor.ShaderKeywordFilter;
using Unity.VisualScripting;
namespace Specifications.Schema
{
    public class PathingState
    {
        public int Cost;
        public bool Blocked;
    }

    public delegate void PathingFunction(Unit mover, Player player, PathingState state, Hex from, Hex to);
    //classes like these act as typed enums.

    //dont worry too hard on these, they are *meant* to *arbitrarily* categorize/generalize things based on the games needs.
    public interface EPathingRule
    {
        public sealed class Standard : EPathingRule { }
        public sealed class IgnoreWall : EPathingRule { }
    }

    public interface ETargetingRule
    {

    }
    public interface EConsequenceRule
    {
        public sealed class Passive : EConsequenceRule
        {
            public List<Implementation.ConsequenceRule> PassiveSpecific;
        }
        public interface Effect : EConsequenceRule
        {
            public sealed class Slow : Effect { }
            public sealed class Silence : Effect { }
        }
    }
    public interface EActionModifier
    {
        public sealed class Passive : EActionModifier
        {
            public 
        }
    }
}
