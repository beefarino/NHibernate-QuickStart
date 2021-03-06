using System;

namespace CodeOwls.PowerShell.Host
{
    public class ExitEventArgs : EventArgs
    {
        private readonly int _exitCode;

        public ExitEventArgs(int exitCode)
        {
            _exitCode = exitCode;
        }

        public int ExitCode
        {
            get { return _exitCode; }
        }
    }
}