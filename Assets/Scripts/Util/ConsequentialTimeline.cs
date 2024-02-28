using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//completely unnecessarily generic class that makes 0 sense to have other than to make things more abstract :)

#nullable enable
namespace GStructures
{
    public class ConsequentialTimeline<T, S> where T : IOnTimeline<S> where S : class
    {
        private ConsequentialTimelineNode<T, S> _head;
        private S _state;

        public ConsequentialTimeline(S infoState, T head)
        {
            _state = infoState;
            _head = new(head, null, 0, _state);
        }
    }
    public class ConsequentialTimelineNode<T, S> where T : IOnTimeline<S> where S : class
    {
        public int LeftForward => _consequences.Count - _pointer;
        public int LeftBackward => _pointer + 1;
        public ConsequentialTimelineNode<T, S>? Parent => _parent;

        internal T _value;
        internal S _state;
        internal ConsequentialTimelineNode<T, S>? _parent;
        internal int _pointer = -1;
        internal int _depth;
        internal List<ConsequentialTimelineNode<T, S>> _consequences;

        public void AddConsequence(T consequence)
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

        internal ConsequentialTimelineNode(T value, ConsequentialTimelineNode<T, S>? parent, int depth, S state)
        {
            _value = value;
            _parent = parent;
            _depth = depth;
            _consequences = new();
            _state = state;
        }
    }

    public interface IOnTimeline<T>
    {
        public abstract void Forward(ref T state);
        public abstract void Backward(ref T state);
    }
}
