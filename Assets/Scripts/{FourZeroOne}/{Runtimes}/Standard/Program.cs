
using UnityEngine;
using Perfection;
#nullable enable
namespace FourZeroOne.Runtimes.Standard
{
    public record Runtime : IRuntime
    {
        public State State { get => _state; init { _io.WriteState(value); _state = value; } }
        public IRuntime dState(Updater<State> updater) => this with { State = updater(State) };
        public IInputInterface Input => _io;
        public IOutputInterface Output => _io;
        public Runtime()
        {
            var gameObject = new GameObject("Standard Runtime IO");
            _io = gameObject.AddComponent<IO>();

        }
        private readonly IO _io;
        private readonly State _state;
    }
}