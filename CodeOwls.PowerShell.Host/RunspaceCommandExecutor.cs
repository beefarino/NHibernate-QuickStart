using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using CodeOwls.PowerShell.Host.Executors;

namespace CodeOwls.PowerShell.Host
{
    public class RunspaceCommandExecutor : AsyncCommandExecutorBase, IRunnableCommandExecutor
    {
        private readonly Executor _executor;
        private readonly Runspace _runspace;
        private readonly ManualResetEvent _stopThreadEvent;
        private Thread _thread;

        public RunspaceCommandExecutor(Runspace runspace)
        {
            _stopThreadEvent = new ManualResetEvent(false);

            _executor = new Executor(runspace);

            runspace.StateChanged += OnRunspaceStateChanged;
            _runspace = runspace;
        }

        private void OnRunspaceStateChanged(object sender, RunspaceStateEventArgs e)
        {
            if (RunspaceState.Closing == e.RunspaceStateInfo.State)
            {
                _runspace.StateChanged -= OnRunspaceStateChanged;
                Stop(true);
            }
        }

        private void ProcessQueue()
        {
            while (true)
            {
                var handles = new[]
                                  {
                                      _stopThreadEvent,
                                      Queue.WaitHandle
                                  };

                int index = WaitHandle.WaitAny(handles);
                if (0 == index)
                {
                    return;
                }

                ExecuteQueuedCommands();
            }
        }

        private void ExecuteQueuedCommands()
        {
            var asynResults = Queue.DequeueAny();
            asynResults.ForEach(ExecuteQueuedCommand);
        }

        private void ExecuteQueuedCommand(AsyncCommandResult asynResult)
        {
            Collection<PSObject> results = null;
            try
            {
                Exception error = null;
                results = _executor.ExecuteCommand(asynResult.Command, asynResult.Parameters, out error,
                                                   asynResult.ExecutionOptions);
                asynResult.SetComplete(results, false, error);
            }
            catch (Exception e)
            {
                asynResult.SetComplete(results, false, e);
            }
        }

        #region Implementation of IRunnableAsyncCommandExecutor

        public override CommandExecutorState CurrentState
        {
            get
            {
                return _runspace.RunspaceAvailability == RunspaceAvailability.Available
                           ? CommandExecutorState.Available
                           : CommandExecutorState.Unavailable;
            }
        }

        public override bool CancelCurrentExecution(int timeoutInMilliseconds)
        {
            return _executor.CancelCurrentPipeline(timeoutInMilliseconds);
        }

        public void Stop()
        {
            Stop(false);
        }

        public void Stop(bool force)
        {
            var thread = Interlocked.Exchange(ref _thread, null);
            if (null == thread)
            {
                return;
            }

            _stopThreadEvent.Set();

            if (force && ! thread.Join(2500))
            {
                try
                {
                    thread.Abort();
                    thread.Join(5000);
                }
                catch
                {
                }
            }
        }

        public void Run()
        {
            var thread = new Thread(ProcessQueue);
            if (null != Interlocked.CompareExchange(ref _thread, thread, null))
            {
                return;
            }

            thread.IsBackground = true;
            thread.Name = GetType().FullName;

            _stopThreadEvent.Reset();
            thread.Start();
        }

        #endregion
    }
}