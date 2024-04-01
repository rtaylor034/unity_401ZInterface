using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using MC = MorseCode.ITask;

// Made with duct tape and dreams.

namespace GStructures
{
    public class ControlledTask : MC.ITask
    {
        public ControlledAwaiter Awaiter { get; private set; }

        /// <summary>
        /// Creates a task that halts <see langword="await"/> execution until Resolve() is called.
        /// </summary>
        public ControlledTask()
        {
            Awaiter = new ControlledAwaiter();
        }

        /// <summary>
        /// Resolves this task and releases <see langword="await"/> execution.
        /// </summary>
        public void Resolve()
        {
            Awaiter.Resolve();
        }

        public MC.IAwaiter GetAwaiter() => Awaiter;
        public MC.IConfiguredTask ConfigureAwait(bool continueOnCapturedContext) => throw new GStructures.NahException();

        public class ControlledAwaiter : MC.IAwaiter
        {
            private bool _completed;
            public bool IsCompleted => _completed;
            private Action _continueAction;

            public ControlledAwaiter()
            {
                _completed = false;
            }
            public void Resolve()
            {
                if (_completed) throw new Exception("Awaiter already resolved");
                _completed = true;
                _continueAction();
            }
            public void OnCompleted(Action continuation)
            {
                _continueAction = continuation;
            }
            // yea, no idea how this is supposed to be different than non-unsafe OnCompleted(). if we crash we crash.
            // the documentation also isnt very helpful. sucks to suck!
            public void UnsafeOnCompleted(Action continuation)
            {
                _continueAction = continuation;
            }

            public void GetResult() { }
        }
    }

    public class ControlledTask<T> : MC.ITask<T>
    {
        public ControlledAwaiter<T> Awaiter { get; private set; }
        public T Result => Awaiter.GetResult();

        /// <summary>
        /// <inheritdoc cref="ControlledTask.ControlledTask"/><br></br>
        /// > Yields type <typeparamref name="T"/> when resolved.
        /// </summary>
        public ControlledTask()
        {
            Awaiter = new ControlledAwaiter<T>();
        }

        /// <summary>
        /// <inheritdoc cref="ControlledTask.Resolve"/> <br></br>
        /// > Yields <paramref name="result"/>.
        /// </summary>
        /// <param name="result"></param>
        public void Resolve(T result)
        {
            Awaiter.Resolve(result);
        }

        public MC.IAwaiter<T> GetAwaiter() => Awaiter;
        MC.IAwaiter MC.ITask.GetAwaiter() => Awaiter;

        MC.IConfiguredTask<T> MC.ITask<T>.ConfigureAwait(bool continueOnCapturedContext) => throw new GStructures.NahException();
        MC.IConfiguredTask MC.ITask.ConfigureAwait(bool continueOnCapturedContext) => throw new GStructures.NahException();

        public class ControlledAwaiter<B> : ControlledTask.ControlledAwaiter, MC.IAwaiter<B>
        {
            private B _result;

            public ControlledAwaiter() : base() { }
            public void Resolve(B result)
            {
                _result = result;
                base.Resolve();
            }
            // reeks
            public new B GetResult() => _result;
        }
    }
}
