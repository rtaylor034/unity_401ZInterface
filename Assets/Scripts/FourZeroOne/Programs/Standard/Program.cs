
using UnityEngine;
using Perfection;
#nullable enable
namespace FourZeroOne.Programs.Standard
{
    public record Program : IProgram
    {
        public State State { get => _state; init { _io.WriteState(value); _state = value; } }
        public IProgram dState(Updater<State> updater) => this with { State = updater(State) };
        public IInputInterface Input => _io;
        public IOutputInterface Output => _io;
        public Program()
        {
            var gameObject = new GameObject("Standard Program IO");
            _io = gameObject.AddComponent<IO>();

        }
        private readonly IO _io;
        private readonly State _state;
    }
}