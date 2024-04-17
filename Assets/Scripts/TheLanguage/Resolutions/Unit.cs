using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perfection;
using Resolution;
namespace Resolutions
{
    public sealed record Unit : NoOp
    {
        public readonly int UUID;
        public int HP { get; init; }
        public Updater<int> dHP { init => HP = value(HP); }
        public Coordinates Position { get; init; }
        public Updater<Coordinates> dPosition { init => Position = value(Position); }
        public Unit(int id)
        {
            UUID = id;
        }
    }
}