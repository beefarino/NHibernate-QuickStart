using System;

namespace CodeOwls.PowerShell.Host.Utility
{
    public class EventArgs<T> : EventArgs
    {
        private readonly T _data;

        public EventArgs(T data)
        {
            _data = data;
        }

        public T Data
        {
            get { return _data; }
        }
    }
}