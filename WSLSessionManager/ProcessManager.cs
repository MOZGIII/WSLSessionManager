using System;
using System.Diagnostics;

namespace WSLSessionManager
{
    internal class ProcessManager : IDisposable
    {
        public enum ProcessDesiredState
        {
            Idle,
            Running,
            Terminated
        }

        private JobObject job = null;
        private Process process = null;
        private ProcessDesiredState state = ProcessDesiredState.Idle;

        public event EventHandler Crashed;

        public bool IsCrashed { get; private set; } = false;
        public ProcessDesiredState State { get => state; }

        public ProcessManager(ProcessStartInfo processStartInfo)
        {
            job = new JobObject();
            process = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };
            process.Exited += Process_Exited;
        }

        public void Dispose()
        {
            Stop();
            process.Dispose();
            job.Dispose();
        }

        public void Start()
        {
            state = ProcessDesiredState.Running;
            bool started = process.Start();

            if (!started || process.HasExited)
            {
                OnCrash();
                return;
            }

            if (started)
            {
                // Only add to job object if a new process started.
                job.AddProcess(process);
            }
        }

        public void Stop()
        {
            state = ProcessDesiredState.Terminated;
            bool closeAttemptSucceeded = process.CloseMainWindow();
            if (closeAttemptSucceeded && !process.WaitForExit(10000))
            {
                process.Kill();
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (state == ProcessDesiredState.Running)
            {
                OnCrash();
            }
        }

        private void OnCrash()
        {
            IsCrashed = true;
            Crashed(this, EventArgs.Empty);
        }
    }
}
