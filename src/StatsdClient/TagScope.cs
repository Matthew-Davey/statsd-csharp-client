using System;

namespace StatsdClient
{
    public class TagScope : IDisposable
    {
        readonly Action _onDispose;

        internal TagScope(object tags, Action onDispose)
        {
            _onDispose = onDispose;
            Tags = tags;
        }

        public Object Tags { get; private set; }

        public void Dispose()
        {
            _onDispose();
        }
    }
}