using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//make it abstract they said. keep it simple they said. ('they' is the voices)
//also, Resolve() returns a Task because fuck you!
#nullable enable
namespace GFunction.ConsequentialTimeline
{
    public class ConsequentialTimeline<TState, TInput>
    {
        public GuardedCollectionHandle<ConsequenceListener> ConsequenceListeners;
        public GuardedCollectionHandle<UnresolvedModifier> UnresolvedActionModifiers;

        public delegate IEnumerable<ITimelineUnresolved<TState, TInput>> ConsequenceListener(ITimelineAction<TState> action);

        /// <summary>
        /// DO NOT mutate <paramref name="unresolved"/>, create a new unresolved action and return it.
        /// </summary>
        /// <param name="unresolved"></param>
        /// <returns></returns>
        public delegate ITimelineUnresolved<TState, TInput>? UnresolvedModifier(ITimelineUnresolved<TState, TInput> unresolved);

        private List<ConsequenceListener> _consequenceListeners;
        private List<UnresolvedModifier> _unresolvedModifiers;
        private TState _state;
        private TInput _input;
        private int _maxDepth;
        private Branch _currentBranch;

        public async Task ResolveAtPresent(ITimelineUnresolved<TState, TInput> unresolvedAction)
        {
            if (_currentBranch.TimeIsPresent) await ResolveInternal(unresolvedAction, 0);
            else await ResolveInternal(unresolvedAction, _currentBranch.NextAction.depth);
            
        }

        private async Task ResolveInternal(ITimelineUnresolved<TState, TInput> unresolvedAction, int depth)
        {
            if (depth > _maxDepth) return;
            List<ITimelineUnresolved<TState, TInput>> unresolvedStates = new() { unresolvedAction };
            foreach (var modifier in _unresolvedModifiers)
            {
                var newState = modifier.Invoke(unresolvedAction);
                if (newState is not null)
                {
                    unresolvedStates.Add(newState);
                    unresolvedAction = newState;
                }
            }
            var newNode = new ActionNode(await unresolvedAction.Resolve(_input), depth, unresolvedStates);
        }
        private class Branch
        {
            private Branch _parent;
            private List<ActionNode> _timeline;
            private Dictionary<ActionNode, List<Branch>> _childBranches;
            int _pointer;
            public ActionNode CurrentAction => _timeline[_pointer];
            public ActionNode NextAction => _timeline[_pointer + 1];
            public bool TimeIsPresent => (_timeline.Count - 1 == _pointer);
            public IEnumerable<Branch>? BranchesFromCurrent => _childBranches.TryGetValue(CurrentAction, out var o) ? o : null;

            private Branch(Branch parent, ActionNode head)
            {
                _parent = parent;
                _timeline = new();
                _childBranches = new();
                _pointer = -1;
                AddNode(head);
            }

            private Branch AddNode(ActionNode action)
            {
                if (TimeIsPresent)
                {
                    _timeline.Add(action);
                    _pointer++;
                    return this;
                }
                if (action.Equals(NextAction))
                {
                    _pointer++;
                    return this;
                }
                //-- otherwise, create new branch
                Branch newBranch = new(this, action);
                if (!_childBranches.TryAdd(CurrentAction, new() { newBranch }))
                {
                    _childBranches[CurrentAction].Add(newBranch);
                }
                return newBranch;
            }
        }

        private class ActionNode
        {
            public List<ITimelineUnresolved<TState, TInput>> unresolvedStates;
            public int depth;
            public ITimelineAction<TState> action;

            public ActionNode(ITimelineAction<TState> action, int depth, List<ITimelineUnresolved<TState, TInput>> unresolvedStates)
            {
                this.depth = depth;
                this.action = action;
                this.unresolvedStates = unresolvedStates;
            }
        }

    }

    /// <summary>
    /// Equivalent actions should return 'true' for <see cref="object.Equals(object)"/>
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public interface ITimelineAction<TState>
    {
        public abstract void Forward(ref TState state);
        public abstract void Backward(ref TState state);
    }

    public interface ITimelineUnresolved<TState, TInput>
    {
        public abstract Task<ITimelineAction<TState>> Resolve(TInput input);
    }
}
