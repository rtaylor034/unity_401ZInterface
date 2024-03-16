using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Specifications.Implementation
{
    public class ConsequenceRule
    {
        public ConditionGroup<GameAction> Condition;
        public List<ConstructionTemplate<UnresolvedAction>> Consequences;
    }
    public class ActionModifier
    {
        public ConditionGroup<UnresolvedAction> Condition;
        public ConstructionTemplate<UnresolvedAction> Modification;
    }
}
