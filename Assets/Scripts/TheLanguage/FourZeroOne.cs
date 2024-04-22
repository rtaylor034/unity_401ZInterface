using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MorseCode.ITask;
using Perfection;
using ResObj = Resolution.IResolution;
using r = Resolutions;
using System;

#nullable enable
namespace FourZeroOne
{
    public interface IProgram
    {
        public IInputProvider Input { get; }
        public State State { get; }
        public ITask<IProgram> WithState(Updater<State> stateUpdater);
    }
    public interface IInputProvider
    {
        public ITask<IEnumerable<R>?> ReadSelection<R>(IEnumerable<R> outOf, int count) where R : class, ResObj;
    }
    public interface IOutputProvider
    {
        public ITask WriteState(State state);
    }
    public record Program : IProgram
    {
        public State State { get; init; }
        public IInputProvider Input => _input;
        public Program(IInputProvider input, IOutputProvider output)
        {
            _input = input;
            _output = output;
        }
        public async ITask<IProgram> WithState(Updater<State> stateUpdater)
        {
            State newState = stateUpdater(State);
            if (newState.Equals(State)) return this;
            await _output.WriteState(newState);
            return this with { State = newState };
        }
        private readonly IInputProvider _input;
        private readonly IOutputProvider _output;
    }

    public record State
    {
        public PMap<string, ResObj> Variables { get; init; }
        public Updater<PMap<string, ResObj>> dVariables { init => Variables = value(Variables); }
        public PList<Rule.IRule> Rules { get; init; }
        public Updater<PList<Rule.IRule>> dRules { init => Rules = value(Rules); }
        public BoardState Board { get; init; }
        public Updater<BoardState> dBoard { init => Board = value(Board); }
        public State WithResolution(ResObj resolution) { return resolution.ChangeState(this); }
    }
    public record BoardState
    {
        public readonly PIndexedSet<int, r.Unit> Units;
        public readonly PIndexedSet<r.Coordinates, r.Hex> Hexes;
        public Updater<PIndexedSet<int, r.Unit>> dUnits { init => Units = value(Units); }
        public Updater<PIndexedSet<r.Coordinates, r.Hex>> dHexes { init => Hexes = value(Hexes); }
        public BoardState()
        {
            Units = new(unit => unit.UUID, 13);
            Hexes = new(hex => hex.Position, 133);
        }
    }
}
namespace Program.Programs
{
    
}