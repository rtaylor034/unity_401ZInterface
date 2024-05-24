
using System.Collections.Generic;
using Perfection;
#nullable enable
namespace FourZeroOne
{
    public interface IProgram
    {
        public IInputInterface Input { get; }
        public IOutputInterface Output { get; }
        public State State { get; }
        public IProgram dState(Updater<State> updater);
    }
}