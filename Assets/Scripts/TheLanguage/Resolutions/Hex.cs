using Perfection;
using Resolution;
namespace Resolutions
{
    namespace Hex
    {
        public abstract record Hex : NoOp
        {
            public Coordinates Position { get; init; }
            public Updater<Coordinates> dPosition { init => Position = value(Position); }
        }
    }
}