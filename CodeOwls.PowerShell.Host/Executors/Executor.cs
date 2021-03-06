using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using CodeOwls.PowerShell.Host.Utility;

namespace CodeOwls.PowerShell.Host.Executors
{
    internal class Executor
    {
        private readonly Runspace _runspace;
        private Pipeline _currentPipeline;

        public Executor(Runspace runspace)
        {
            _runspace = runspace;
        }

        private bool IsRunspaceBusy
        {
            get { return _runspace.RunspaceAvailability == RunspaceAvailability.Busy; }
        }

        public event EventHandler<EventArgs<Exception>> PipelineException;

        public bool CancelCurrentPipeline(int timeoutInMilliseconds)
        {
            var pipeline = _currentPipeline;
            var activeStates = new[] {PipelineState.Running, PipelineState.NotStarted, PipelineState.Stopping};
            var inactiveStates = new[] {PipelineState.Stopped, PipelineState.Failed, PipelineState.Completed};
            if (null != pipeline &&
                activeStates.Contains(pipeline.PipelineStateInfo.State))
            {
                pipeline.StopAsync();
                DateTime expiry = DateTime.Now + TimeSpan.FromMilliseconds(timeoutInMilliseconds);
                while (
                    (-1 == timeoutInMilliseconds || DateTime.Now <= expiry) &&
                    activeStates.Contains(pipeline.PipelineStateInfo.State))
                {
                    DoWait();
                }

                return inactiveStates.Contains(pipeline.PipelineStateInfo.State);
            }

            return true;
        }

        public string ExecuteAndGetStringResult(string command, out Exception exceptionThrown)
        {
            var results = ExecuteCommand(command, out exceptionThrown, ExecutionOptions.None);
            if (null != exceptionThrown)
            {
                return null;
            }

            if (null == results || ! results.Any())
            {
                return null;
            }

            PSObject pso = results[0];
            if (null == pso)
            {
                return String.Empty;
            }
            if (null == pso.BaseObject)
            {
                return pso.ToString();
            }
            return pso.BaseObject.ToString();
        }

        public Collection<PSObject> ExecuteCommand(string command, Dictionary<string, object> inputs,
                                                   out Exception error, ExecutionOptions options)
        {
            var cmd = new Command(command, true, false);

            if (null != inputs && inputs.Any())
            {
                Debug.Fail("inputs are not supported - fix calling code");
                //inputs.ToList().ForEach(pair =>
                //                        cmd.Parameters.Add(pair.Key, pair.Value));
            }

            var pipe = _runspace.CreatePipeline();

            pipe.Commands.Add(cmd);

            return ExecuteCommandHelper(pipe, out error, options);
        }

        public Collection<PSObject> ExecuteCommand(string command, out Exception error, ExecutionOptions options)
        {
            var pipe = _runspace.CreatePipeline(command,
                                                ExecutionOptions.None != (ExecutionOptions.AddToHistory & options));
            return ExecuteCommandHelper(pipe, out error, options);
        }

        private void RaisePipelineExceptionEvent(Exception e)
        {
            EventHandler<EventArgs<Exception>> handler = PipelineException;
            if (handler != null)
            {
                handler(this, new EventArgs<Exception>(e));
            }
        }

        private Collection<PSObject> ExecuteCommandHelper(Pipeline tempPipeline, out Exception exceptionThrown,
                                                          ExecutionOptions options)
        {
            exceptionThrown = null;
            Collection<PSObject> collection = null;
            ApplyExecutionOptionsToPipeline(options, tempPipeline);
            collection = ExecutePipeline(options, tempPipeline, collection, out exceptionThrown);
            return collection;
        }

        private static void ApplyExecutionOptionsToPipeline(ExecutionOptions options, Pipeline tempPipeline)
        {
            if ((options & ExecutionOptions.AddOutputter) != ExecutionOptions.None)
            {
                if (tempPipeline.Commands.Count == 1)
                {
                    tempPipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                }
                var item = new Command("Out-Default", false, true);
                tempPipeline.Commands.Add(item);
            }
        }

        private Collection<PSObject> ExecutePipeline(ExecutionOptions options, Pipeline tempPipeline,
                                                     Collection<PSObject> collection, out Exception exceptionThrown)
        {
            exceptionThrown = null;
            try
            {
                bool acquired = Monitor.TryEnter(_runspace);
                if (! acquired)
                {
                    return null;
                }

                try
                {
                    WaitWhileRunspaceIsBusy();

                    _currentPipeline = tempPipeline;

                    try
                    {
                        tempPipeline.InvokeAsync();
                        tempPipeline.Input.Close();

                        WaitWhilePipelineIsRunning(tempPipeline);
                    }
                    finally
                    {
                        _currentPipeline = null;
                    }

                    var exception = GetPipelineError(options, tempPipeline);

                    collection = tempPipeline.Output.ReadToEnd();

                    if (null != exception)
                    {
                        RaisePipelineExceptionEvent(exception);
                    }
                }
                finally
                {
                    Monitor.Exit(_runspace);
                }
            }
            catch (Exception exception)
            {
                exceptionThrown = exception;
            }
            return collection;
        }

        private void WaitWhilePipelineIsRunning(Pipeline tempPipeline)
        {
        }

        private void WaitWhileRunspaceIsBusy()
        {
            while (IsRunspaceBusy)
            {
                DoWait();
            }
        }

        private void DoWait()
        {
        }

        private Exception GetPipelineError(ExecutionOptions options, Pipeline tempPipeline)
        {
            Exception pipelineException = null;

            if (null != tempPipeline.PipelineStateInfo.Reason)
            {
                pipelineException = tempPipeline.PipelineStateInfo.Reason;
            }

            if (null == pipelineException &&
                0 < tempPipeline.Error.Count)
            {
                var error = tempPipeline.Error.Read();
                pipelineException = error as Exception;
                if (null == pipelineException)
                {
                    pipelineException = ((ErrorRecord) error).Exception;
                }
            }

            if (null != pipelineException &&
                0 == (options & ExecutionOptions.AddOutputter))
            {
                throw pipelineException;
            }

            return pipelineException;
        }
    }
}