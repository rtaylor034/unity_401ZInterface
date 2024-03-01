using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
//make it abstract they said. keep it simple they said. ('they' is the voices)
//also, its ITimelineEvaluator is just async becuase fuck you!
#nullable enable
namespace GFunction.ConsequentialTimeline
{
    /// <summary>
    /// The entirety of this class works on the assumption that every implementation of <see cref="ITimelineAction{TState}"/> and <see cref="ITimelineEvaluator{TState, TInput}"/> are 100% clean.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TInput"></typeparam>
    public class ConsequentialTimeline<TState, TInput> where TState : class
    {
        public delegate void EvaluatorModifier(ref ITimelineEvaluator<TState, TInput> evaluator);
        public delegate IEnumerable<ITimelineEvaluator<TState, TInput>> ConsequenceAdder(in ITimelineAction<TState> action);

        /// <summary>
        /// Volitile. Proper clearing/removing of added function is the responsibility of user.
        /// </summary>
        public GuardedCollectionHandle<ConsequenceAdder> ConsequenceListeners => new(_consequenceAdders);
        /// <summary>
        /// Volitile. Proper clearing/removing of added function is the responsibility of user.
        /// </summary>
        public GuardedCollectionHandle<EvaluatorModifier> EvaluatorListeners => new(_evaluatorModifiers);


        private ConsequenceSequence<TState> _head;
        private TState _state;
        private List<ConsequenceAdder> _consequenceAdders;
        private List<EvaluatorModifier> _evaluatorModifiers;
        private List<ConsequentialTimeline<TState, TInput>> _branches;

        public ConsequentialTimeline(TState infoState, ITimelineAction<TState> head)
        {
            _state = infoState;
            _head = new(head, null, 0, _state);
            _consequenceAdders = new();
            _evaluatorModifiers = new();
            _branches = new();
        }
        //branch constructor
        private ConsequentialTimeline(ConsequentialTimeline<TState, TInput> source, ConsequenceSequence<TState> head)
        {
            _state = source._state;
            _head = head;
            _consequenceAdders = new(source._consequenceAdders);
            _evaluatorModifiers = new(source._evaluatorModifiers);
            _branches = new();
        }
    }

    public class TimelineNode<TState, TInput> where TState : class
    {
        private ITimelineEvaluator<TState, TInput> _evaluator;
        private List<ConsequenceSequence<TState>> _branches;
        private TimelineNode<TState, TInput> _from;
    }
    public class ConsequenceSequence<TState> where TState : class
    {
        public int LeftForward => _consequences.Count - _pointer;
        public int LeftBackward => _pointer + 1;
        public ConsequenceSequence<TState>? Parent => _parent;

        internal ITimelineAction<TState> _cause;
        internal TState _state;
        internal int _pointer = -1;
        internal int _depth;
        internal ConsequenceSequence<TState>? _parent;
        internal List<ConsequenceSequence<TState>> _consequences;

        public void AddConsequence(ITimelineAction<TState> consequence)
        {
            _consequences.Add(new(consequence, this, _depth + 1, _state));
        }

        internal bool ForwardAll()
        {
            if (LeftForward <= 0) return false;
            if (_pointer == -1)
            {
                _cause.Forward(ref _state);
                _pointer++;
            }
            for (int i = _pointer; i < LeftForward; i++)
            {
                _consequences[_pointer].ForwardAll();
            }
            if (_parent is not null) _parent._pointer++;
            return true;
        }
        internal bool BackwardAll()
        {
            if (LeftBackward <= 0) return false;
            for (int i = _pointer - 1; i >= 0; i--)
            {
                _consequences[_pointer].BackwardAll();
            }
            Debug.Assert(_pointer == 0);
            _cause.Backward(ref _state);
            _pointer--;
            if (_parent is not null) _parent._pointer--;
            return true;
        }

        internal ConsequenceSequence(ITimelineAction<TState> cause, ConsequenceSequence<TState>? parent, int depth, TState state)
        {
            _cause = cause;
            _parent = parent;
            _depth = depth;
            _consequences = new();
            _state = state;
        }
    }
    public interface ITimelineAction<TState> where TState : class
    {
        public abstract void Forward(ref TState state);
        public abstract void Backward(ref TState state);
        public abstract bool Equivalent(ITimelineAction<TState> other);
    }
    public interface ITimelineEvaluator<TState, TInput> where TState : class
    {
        public abstract Task<ITimelineAction<TState>> Evaluate(TInput input);
    }
}
