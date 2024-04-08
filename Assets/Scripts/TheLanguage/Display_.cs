using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Display_
{
    public interface IDisplayable
    {
        public DisplayInfo Display { get; }
    }
    public record DisplayInfo
    {
        public int Size_ { get; init; }
        public string Graphic_ { get; init; }
        public bool ExampleInfo_ { get; init; }
    }
    public record Token : IDisplayable
    {
        public DisplayInfo Display => new()
        {
            Size_ = 1,
            Graphic_ = "something",
            ExampleInfo_ = true,
        };
    }
}