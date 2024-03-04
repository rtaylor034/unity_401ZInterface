using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
//make it abstract they said. keep it simple they said. ('they' is the voices)
//also, Resolve() returns a Task because fuck you!
#nullable disable
namespace GFunction.ConsequentialTimeline
{
    public delegate IEnumerable<ITimelineUnresolved<TState, TInput>> ConsequenceListener<TState, TInput>(ITimelineAction<TState> action);
    public delegate ITimelineUnresolved<TState, TInput> UnresolvedModifier<TState, TInput>(ITimelineUnresolved<TState, TInput> unresolved);
    public class ConsequentialTimeline<TState, TInput>
    {
        public GuardedCollectionHandle<ConsequenceListener<TState, TInput>> ConsequenceListeners;
        public GuardedCollectionHandle<UnresolvedModifier<TState, TInput>> UnresolvedActionModifiers;

        private List<ActionWrapper<TState>> _timeline;
        private Dictionary<ActionWrapper<TState>, List<ConsequentialTimeline<TState, TInput>>> _branches;
        private int _location;
        private List<ConsequenceListener<TState, TInput>> _consequenceListeners;
        private List<UnresolvedModifier<TState, TInput>> _unresolvedModifiers;
        private TState _state;
        private TInput _input;
        private int _maxDepth;
        private ConsequentialTimeline<TState, TInput> _currentBranch;
        
        private ActionWrapper<TState> CurrentAction => _timeline[_location];
        private ActionWrapper<TState> NextAction => _timeline[_location + 1];
        private bool TimeIsPresent => (_timeline.Count - 1 == _location);

        public void AdvanceWith(ITimelineUnresolved<TState, TInput> unresolvedAction)
        {n
            if (TimeIsPresent) AdvanceInternal(unresolvedAction, 0);
            else AdvanceInternal(unresolvedAction, CurrentAction._depth);
        }
        public void Rewind(int toDepth)
        {
            // :: NEEDS BOUNDS CHECK ::

        }

        private void AdvanceInternal(ITimelineUnresolved<TState, TInput> unresolvedAction, int depth)
        {
            foreach (var modifier in _unresolvedModifiers) unresolvedAction = modifier.Invoke(unresolvedAction);
            var action = unresolvedAction.Resolve(_input);

        }
        private void AddNode(ActionWrapper<TState> action)
        {
            ref var c = ref _currentBranch;
            if (c.TimeIsPresent)
            {
                c._timeline.Add(action);
                c._location++;
                return;
            }
            if (action.EquivalentTo(NextAction))
            {
                c._location++;
                return;
            }
            //-- otherwise, create new branch
            ConsequentialTimeline<TState, TInput> newBranch = new();
            newBranch.AddNode(action);
            if (!c._branches.TryAdd(c.CurrentAction, new(newBranch.Wrapped())))
            {
                c._branches[CurrentAction].Add(newBranch);
            }
            c._currentBranch = newBranch;
        }
        private ConsequentialTimeline()
        {
            _timeline = new();
            _branches = new();
            _location = -1;
        }
    }

    internal class ActionWrapper<TSTate>
    {
        internal int _depth;
        internal ITimelineAction<TSTate> _action;

        internal ActionWrapper(ITimelineAction<TSTate> action, int depth)
        {
            _depth = depth;
            _action = action;
        }
        internal bool EquivalentTo(ActionWrapper<TSTate> action)
        {
            return (action._depth == _depth && action._action.EquivalentTo(_action));
        }
    }
    public interface ITimelineAction<TState>
    {
        public abstract void Forward(ref TState state);
        public abstract void Backward(ref TState state);
        public abstract bool EquivalentTo(ITimelineAction<TState> other);
    }

    public interface ITimelineUnresolved<TState, TInput>
    {
        public abstract Task<ITimelineAction<TState>> Resolve(TInput input);
    }
}
