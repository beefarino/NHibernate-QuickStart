using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace CodeOwls.PowerShell.Host.Executors
{
    public interface ICommandExecutor
    {
        CommandExecutorState CurrentState { get; }
        Collection<PSObject> Execute(string command);
        Collection<PSObject> Execute(string command, Dictionary<string, object> parameters);

        IAsyncResult BeginExecute(string command, Dictionary<string, object> parameters, ExecutionOptions options,
                                  AsyncCallback callback, object asyncState);

        Collection<PSObject> EndExecute(IAsyncResult ar);
        bool CancelCurrentExecution(int timeoutInMilliseconds);
    }
}