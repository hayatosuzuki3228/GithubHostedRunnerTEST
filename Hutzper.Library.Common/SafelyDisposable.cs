namespace Hutzper.Library.Common
{
    // IDisposableを安全に破棄する
    public interface ISafelyDisposable : IDisposable
    {
        public T? DisposeSafely<T>(T obj) where T : IDisposable;

        protected virtual void DisposeExplicit() { }

        protected virtual void DisposeImplicit() { }
    }

    // ISafelyDisposable 実装
    [Serializable]
    public abstract class SafelyDisposable : ISafelyDisposable
    {
        public bool Disposed { get; private set; }

        public virtual T? DisposeSafely<T>(T? obj) where T : IDisposable
        {
            try
            {
                obj?.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }

            return default;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == this.Disposed)
            {
                if (true == disposing)
                {
                    this.DisposeExplicit();
                }

                this.DisposeImplicit();

                this.Disposed = true;
            }
        }

        protected virtual void DisposeExplicit() { }

        protected virtual void DisposeImplicit() { }

        ~SafelyDisposable()
        {
            this.Dispose(false);
        }
    }

    // ISafelyDisposable 実装
    [Serializable]
    public abstract class SafelyDisposableEventArgs : System.EventArgs, ISafelyDisposable
    {
        public bool Disposed { get; private set; }

        public virtual T? DisposeSafely<T>(T? obj) where T : IDisposable
        {
            try
            {
                obj?.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }

            return default;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == this.Disposed)
            {
                if (true == disposing)
                {
                    this.DisposeExplicit();
                }

                this.DisposeImplicit();

                this.Disposed = true;
            }
        }

        protected virtual void DisposeExplicit() { }

        protected virtual void DisposeImplicit() { }


        ~SafelyDisposableEventArgs()
        {
            this.Dispose(false);
        }
    }

    // ISafelyDisposable 実装
    [Serializable]
    public abstract record SafelyDisposableRecord : ISafelyDisposable
    {
        public bool Disposed { get; private set; }

        public virtual T? DisposeSafely<T>(T? obj) where T : IDisposable
        {
            try
            {
                obj?.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }

            return default;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == this.Disposed)
            {
                if (true == disposing)
                {
                    this.DisposeExplicit();
                }

                this.DisposeImplicit();

                this.Disposed = true;
            }
        }

        protected virtual void DisposeExplicit() { }

        protected virtual void DisposeImplicit() { }

        ~SafelyDisposableRecord()
        {
            this.Dispose(false);
        }
    }
}