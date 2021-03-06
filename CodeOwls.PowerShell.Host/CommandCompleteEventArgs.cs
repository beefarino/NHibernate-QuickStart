using System;

namespace CodeOwls.PowerShell.Host
{
    public class CommandCompleteEventArgs : EventArgs
    {
        public CommandCompleteEventArgs()
        {
        }

        public CommandCompleteEventArgs(string cmd)
        {
            Command = cmd;
        }

        public string Command { get; set; }
    }
}