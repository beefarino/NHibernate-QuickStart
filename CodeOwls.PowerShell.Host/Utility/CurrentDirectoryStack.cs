using System;

namespace CodeOwls.PowerShell.Host.Utility
{
    public static class CurrentDirectoryStack
    {
        public static IDisposable Push(string path)
        {
            return new Session(path);
        }

        #region Nested type: Session

        private class Session : IDisposable
        {
            private readonly string _newCurrentDirectory;
            private readonly string _oldCurrentDirectory;

            public Session(string newCurrentDirectory)
            {
                _newCurrentDirectory = newCurrentDirectory;
                _oldCurrentDirectory = Environment.CurrentDirectory;
                Environment.CurrentDirectory = _newCurrentDirectory;
            }

            #region IDisposable Members

            public void Dispose()
            {
                Environment.CurrentDirectory = _oldCurrentDirectory;
            }

            #endregion
        }

        #endregion
    }
}