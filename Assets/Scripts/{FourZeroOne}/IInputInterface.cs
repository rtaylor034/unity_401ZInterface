
using System.Collections.Generic;
using MorseCode.ITask;
using Perfection;


#nullable enable
namespace FourZeroOne
{
    using ResObj = Resolution.IResolution;
    public interface IInputInterface
    {
        public ITask<IOption<IEnumerable<R>>?> ReadSelection<R>(IEnumerable<R> from, int count) where R : class, ResObj;
    }
}