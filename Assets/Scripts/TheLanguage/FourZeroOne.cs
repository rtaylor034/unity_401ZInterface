using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MorseCode.ITask;
using Perfection;
using ResObj = Resolution.IResolution;
using r = Resolutions;
using System;
using Token.Unsafe;
using Rule;

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
    public interface IInputInterface
    {
        public ITask<IEnumerable<R>?> ReadSelection<R>(IEnumerable<R> outOf, int count) where R : class, ResObj;
    }
    public interface IOutputInterface
    {
        public void WriteRuleSteps(IEnumerable<(IToken fromToken, Rule.IRule rule)> pairs);
        public void WriteToken(IToken token);
        public void WriteResolution(IOption<ResObj>? resolution);
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
namespace FourZeroOne.Programs
{
    namespace Standard
    {
        public record Program : IProgram
        {
            public State State { get => _state; init { _io.WriteState(value); _state = value; } }
            public IProgram dState(Updater<State> updater) => this with { State = updater(State) };
            public IInputInterface Input => _io;
            public IOutputInterface Output => _io;
            public Program(IO io)
            {
                _io = io;
            }
            private readonly IO _io;
            private readonly State _state;
        }
    
    }
}