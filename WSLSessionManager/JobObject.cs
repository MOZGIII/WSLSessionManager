using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WSLSessionManager
{
    public class JobObject : IDisposable
    {
        #region WinAPI

        internal static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern IntPtr CreateJobObject(IntPtr a, string lpName);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, UInt32 cbJobObjectInfoLength);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

            [DllImport("kernel32.dll", SetLastError = true)]
            internal static extern bool CloseHandle(IntPtr hObject);

            [StructLayout(LayoutKind.Sequential)]
            internal struct IO_COUNTERS
            {
                internal UInt64 ReadOperationCount;
                internal UInt64 WriteOperationCount;
                internal UInt64 OtherOperationCount;
                internal UInt64 ReadTransferCount;
                internal UInt64 WriteTransferCount;
                internal UInt64 OtherTransferCount;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct JOBOBJECT_BASIC_LIMIT_INFORMATION
            {
                internal Int64 PerProcessUserTimeLimit;
                internal Int64 PerJobUserTimeLimit;
                internal UInt32 LimitFlags;
                internal UIntPtr MinimumWorkingSetSize;
                internal UIntPtr MaximumWorkingSetSize;
                internal UInt32 ActiveProcessLimit;
                internal UIntPtr Affinity;
                internal UInt32 PriorityClass;
                internal UInt32 SchedulingClass;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct SECURITY_ATTRIBUTES
            {
                internal UInt32 nLength;
                internal IntPtr lpSecurityDescriptor;
                internal Int32 bInheritHandle;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                internal JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
                internal IO_COUNTERS IoInfo;
                internal UIntPtr ProcessMemoryLimit;
                internal UIntPtr JobMemoryLimit;
                internal UIntPtr PeakProcessMemoryUsed;
                internal UIntPtr PeakJobMemoryUsed;
            }

            internal enum JobObjectInfoType
            {
                AssociateCompletionPortInformation = 7,
                BasicLimitInformation = 2,
                BasicUIRestrictions = 4,
                EndOfJobTimeInformation = 6,
                ExtendedLimitInformation = 9,
                SecurityLimitInformation = 5,
                GroupInformation = 11
            }
        }

        private static void SetInformationJobObject(IntPtr hJob, object extendedInfo, NativeMethods.JobObjectInfoType type)
        {
            int length = Marshal.SizeOf(extendedInfo);
            IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
            try
            {
                Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);

                if (!NativeMethods.SetInformationJobObject(hJob, type, extendedInfoPtr, (uint)length))
                {
                    throw new Exception(string.Format("SetInformationJobObject failed.  Error: {0}", Marshal.GetLastWin32Error()));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(extendedInfoPtr);
            }
        }

        private static void AssignProcessToJobObject(IntPtr job, IntPtr process)
        {
            if (!NativeMethods.AssignProcessToJobObject(job, process))
            {
                throw new Exception(string.Format("AssignProcessToJobObject failed.  Error: {0}", Marshal.GetLastWin32Error()));
            }
        }

        #endregion WinAPI

        private IntPtr handle = IntPtr.Zero;

        public JobObject()
        {
            handle = NativeMethods.CreateJobObject(IntPtr.Zero, null);

            var info = new NativeMethods.JOBOBJECT_BASIC_LIMIT_INFORMATION
            {
                LimitFlags = 0x2000
            };

            var extendedInfo = new NativeMethods.JOBOBJECT_EXTENDED_LIMIT_INFORMATION
            {
                BasicLimitInformation = info
            };

            SetInformationJobObject(handle, extendedInfo, NativeMethods.JobObjectInfoType.ExtendedLimitInformation);
        }

        public void AddProcess(IntPtr processHandle)
        {
            AssignProcessToJobObject(handle, processHandle);
        }

        public void AddProcess(Process process)
        {
            AddProcess(process.Handle);
        }

        public void AddProcess(int processId)
        {
            AddProcess(Process.GetProcessById(processId));
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

                NativeMethods.CloseHandle(handle);
                handle = IntPtr.Zero;

                disposedValue = true;
            }
        }

        ~JobObject()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // The following line is required because we've overriden the finalizer.
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
