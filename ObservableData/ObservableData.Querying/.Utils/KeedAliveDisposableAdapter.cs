using System;

namespace ObservableData.Querying.Utils
{
    public sealed class KeedAliveDisposableAdapter : IDisposable
    {
        private IDisposable _disposable;
        private object _keepAliveObject;

        public KeedAliveDisposableAdapter(IDisposable disposable, object keepAliveObject)
        {
            _disposable = disposable;
            _keepAliveObject = keepAliveObject;
        }

        public void Dispose()
        {
            GC.KeepAlive(_keepAliveObject);
            _keepAliveObject = null;
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}