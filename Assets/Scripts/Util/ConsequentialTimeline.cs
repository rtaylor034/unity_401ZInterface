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
    public class ConsequentialTimeline<TState, TAction, TEval, TInput>
        where TState : class
        where TAction : ITimelineAction<TState>
        where TEval : ITimelineEvaluator<TState, TInput>
    {
        private ConsequentialTimelineNode<TState, TAction> _head;
        private TState _state;
        private bool _consequenceListeners;


        public ConsequentialTimeline(TState infoState, TAction head)
        {
            _state = infoState;
            _head = new(head, null, 0, _state);
        }
    }
    public class ConsequentialTimelineNode<TState, TAction>
        where TState : class
        where TAction : ITimelineAction<TState>
    {
        public int LeftForward => _consequences.Count - _pointer;
        public int LeftBackward => _pointer + 1;
        public ConsequentialTimelineNode<TState, TAction>? Parent => _parent;

        internal T _value;
        internal TState _state;
        internal ConsequentialTimelineNode<TState, TAction>? _parent;
        internal int _pointer = -1;
        internal int _depth;
        internal List<ConsequentialTimelineNode<TState, TAction>> _consequences;

        public void AddConsequence(TAction consequence)
        {
            _consequences.Add(new(consequence, this, _depth + 1, _state));
        }

        internal bool ForwardAll()
        {
            if (LeftForward <= 0) return false;
            if (_pointer == -1)
            {
                _value.Forward(ref _state);
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
            _value.Backward(ref _state);
            _pointer--;
            if (_parent is not null) _parent._pointer--;
            return true;
        }

        internal ConsequentialTimelineNode(TAction value, ConsequentialTimelineNode<TState, TAction>? parent, int depth, TState state)
        {
            _value = value;
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
    }
    public interface ITimelineEvaluator<TState, TInput> where TState : class
    {
        public abstract Task<ITimelineAction<TState>> Evaluate(TInput input);
    }
}
