using System;
using System.Runtime.InteropServices;

namespace WSLSessionManager
{
    internal class Wow64FsRedirectionDisabler : IDisposable
    {
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);
        }

        private IntPtr oldValue;
        private bool disabled = false;

        public Wow64FsRedirectionDisabler()
        {
            if (!NativeMethods.Wow64DisableWow64FsRedirection(ref oldValue))
            {
                throw new Exception(string.Format("Wow64DisableWow64FsRedirection failed.  Error: {0}", Marshal.GetLastWin32Error()));
            }
            disabled = true;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // No managed state to dispose.
                }

                if (disabled)
                {
                    if (!NativeMethods.Wow64RevertWow64FsRedirection(oldValue))
                    {
                        throw new Exception(string.Format("Wow64RevertWow64FsRedirection failed.  Error: {0}", Marshal.GetLastWin32Error()));
                    }
                }

                disposedValue = true;
            }
        }

        ~Wow64FsRedirectionDisabler()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
