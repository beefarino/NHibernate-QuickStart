using System;

namespace CodeOwls.PowerShell.Host.Executors
{
    [Flags]
    public enum ExecutionOptions
    {
        None = 0,
        AddOutputter = 1,
        AddToHistory = 2,
    }
}