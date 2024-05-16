using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MorseCode.ITask;
using Perfection;
using ResObj = Resolution.IResolution;
using r = Resolutions;
using rb = Resolutions.Board;
using System;
using Token.Unsafe;

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
        public ITask<IOption<IEnumerable<R>>?> ReadSelection<R>(IEnumerable<R> outOf, int count) where R : class, ResObj;
    }
    public interface IOutputInterface
    {
        /// <summary>
        /// Called when a token is transformed via rules, <paramref name="pairs"/> holding the transformation steps. <br></br>
        /// <i><see cref="WriteToken(IToken)"/> is only called on the final resulting token.</i>
        /// </summary>
        /// <param name="pairs"></param>
        public void WriteRuleSteps(IEnumerable<(IToken fromToken, Rule.IRule rule)> pairs);
        /// <summary>
        /// Called when <paramref name="token"/> *starts* resolving.<br></br>
        /// <i>Only called on final realization after all rules are applied.</i>
        /// </summary>
        /// <param name="token"></param>
        public void WriteToken(IToken token);
        /// <summary>
        /// Called when a token resolves to <paramref name="resolution"/>.
        /// </summary>
        /// <param name="resolution"></param>
        public void WriteResolution(IOption<ResObj>? resolution);
    }
    
    public record State
    {
        public PMap<VariableIdentifier, IOption<ResObj>> Variables { get; init; }
        public Updater<PMap<VariableIdentifier, IOption<ResObj>>> dVariables { init => Variables = value(Variables); }
        public PList<Rule.IRule> Rules { get; init; }
        public Updater<PList<Rule.IRule>> dRules { init => Rules = value(Rules); }
        public PIndexedSet<ResObj, Rule.IRule> Effects { get; init; } 
        public Updater<PIndexedSet<ResObj, Rule.IRule>> dEffects { init => Effects = value(Effects); }
        public BoardState Board { get; init; }
        public Updater<BoardState> dBoard { init => Board = value(Board); }
        public State WithResolution(ResObj resolution) { return resolution.ChangeState(this); }
    }
    public record BoardState
    {
        public readonly PIndexedSet<int, rb.Unit> Units;
        public readonly PIndexedSet<rb.Coordinates, rb.Hex> Hexes;
        public Updater<PIndexedSet<int, rb.Unit>> dUnits { init => Units = value(Units); }
        public Updater<PIndexedSet<rb.Coordinates, rb.Hex>> dHexes { init => Hexes = value(Hexes); }
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
            public Program()
            {
                var gameObject = new GameObject("Standard Program IO");
                _io = gameObject.AddComponent<IO>();

            }
            private readonly IO _io;
            private readonly State _state;
        }
    
    }
}