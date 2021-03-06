using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using CodeOwls.PowerShell.Host.Utility;

namespace CodeOwls.PowerShell.Host.Executors
{
    public abstract class AsyncCommandExecutorBase : ICommandExecutor
    {
        private readonly SyncQueue<AsyncCommandResult> _queue;

        protected AsyncCommandExecutorBase()
        {
            _queue = new SyncQueue<AsyncCommandResult>();
        }

        protected AsyncCommandExecutorBase(SyncQueue<AsyncCommandResult> queue)
        {
            _queue = queue;
        }

        protected internal SyncQueue<AsyncCommandResult> Queue
        {
            get { return _queue; }
        }

        #region ICommandExecutor Members

        public Collection<PSObject> Execute(string command)
        {
            return Execute(command, null);
        }

        public Collection<PSObject> Execute(string command, Dictionary<string, object> parameters)
        {
            var ar = BeginExecute(command, parameters, false, null, null);
            return EndExecute(ar);
        }

        public IAsyncResult BeginExecute(string command, Dictionary<string, object> parameters, ExecutionOptions options,
                                         AsyncCallback callback, object asyncState)
        {
            AsyncCommandResult asyncCommandResult = new AsyncCommandResult(command, parameters, options, callback,
                                                                           asyncState);
            Queue.Enqueue(asyncCommandResult);

            return asyncCommandResult;
        }

        public Collection<PSObject> EndExecute(IAsyncResult ar)
        {
            AsyncCommandResult asyncCommandResult = ar as AsyncCommandResult;
            if (null == asyncCommandResult)
            {
                throw new InvalidOperationException("the IAsyncResult provided is not of the appropriate type");
            }

            while (!asyncCommandResult.IsCompleted)
            {
                asyncCommandResult.AsyncWaitHandle.WaitOne(100);
                DoWait();
            }
            return asyncCommandResult.GetCommandResults();
        }

        public abstract bool CancelCurrentExecution(int timeoutInMilliseconds);
        public abstract CommandExecutorState CurrentState { get; }

        #endregion

        public IAsyncResult BeginExecute(string command, Dictionary<string, object> parameters, bool outputToConsole,
                                         AsyncCallback callback, object asyncState)
        {
            ExecutionOptions options = outputToConsole ? ExecutionOptions.AddOutputter : ExecutionOptions.None;
            return BeginExecute(command, parameters, options, callback, asyncState);
        }

        protected internal void DoWait()
        {
        }
    }
}