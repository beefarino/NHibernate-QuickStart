using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using CodeOwls.PowerShell.Host.AutoComplete;
using CodeOwls.PowerShell.Host.Configuration;
using CodeOwls.PowerShell.Host.Console;
using CodeOwls.PowerShell.Host.Executors;
using CodeOwls.PowerShell.Host.History;
using CodeOwls.PowerShell.Host.Host;
using CodeOwls.PowerShell.Host.Utility;

namespace CodeOwls.PowerShell.Host
{
    public class Shell : AsyncCommandExecutorBase, IRunnableCommandExecutor
    {
        private readonly IConsole _consoleWindow;
        private readonly ShellConfiguration _shellConfiguration;
        private AutoCompleteWalker _autoCompleteWalker;
        private Executor _commandExecutor;
        private HistoryStackWalker _historyStackWalker;
        private Host.Host _host;
        private HostUI _hostUi;
        private HostRawUI _rawUi;
        private Runspace _runspace;

        private Thread _thread;

        public Shell(IConsole consoleWindow, ShellConfiguration shellConfiguration)
        {
            _consoleWindow = consoleWindow;
            _shellConfiguration = shellConfiguration;
        }

        #region IRunnableCommandExecutor Members

        public override bool CancelCurrentExecution(int timeoutInMilliseconds)
        {
            return _commandExecutor.CancelCurrentPipeline(timeoutInMilliseconds);
        }

        public void Stop()
        {
            Stop(false);
        }

        public void Stop(bool force)
        {
            var thread = _thread;
            _host.SetShouldExit(0);

            if (!force)
            {
                return;
            }

            if (null != thread)
            {
                if (!thread.Join(2500))
                {
                    thread.Abort();
                    thread.Join(5000);
                }
            }
        }

        public void Run()
        {
            var thread = new Thread(_Run);

            var existing = Interlocked.CompareExchange(ref _thread, thread, null);
            if (null != existing)
            {
                return;
            }

            StartupState state = new StartupState();
            thread.SetApartmentState(ApartmentState.MTA);
            thread.IsBackground = true;
            thread.Start(state);
            state.WaitForStartup();
        }

        public override CommandExecutorState CurrentState
        {
            get
            {
                return _runspace.RunspaceAvailability == RunspaceAvailability.Available
                           ? CommandExecutorState.Available
                           : CommandExecutorState.Unavailable;
            }
        }

        #endregion

        public event EventHandler<ExitEventArgs> ShouldExit;
        public event EventHandler<ProgressRecordEventArgs> Progress;
        public event EventHandler<EventArgs<bool>> CommandExecutionStateChange;

        private void InvokeShouldExit(int exitCode)
        {
            EventHandler<ExitEventArgs> handler = ShouldExit;
            if (handler != null)
            {
                handler(this, new ExitEventArgs(exitCode));
            }
        }

        private void _Run(object o)
        {
            try
            {
                StartupState state = (StartupState) o;
                Exception e = null;
                try
                {
                    InitializeRunspaceAndHost();
                }
                catch (Exception ex)
                {
                    state.SetComplete(ex);
                    return;
                }

                state.SetComplete(null);

                RunProfileScripts();
                RunInitializationScripts();

                ExecuteRunLoop();
            }
            catch (ThreadAbortException)
            {
            }
            finally
            {
                Interlocked.Exchange(ref _thread, null);
            }
        }

        private void ExecuteRunLoop()
        {
            var autoCompleteProviders = new List<IAutoCompleteProvider>
                                            {
                                                new AutoCompleteProviderChain(
                                                    new PowerShellTabExansionAutoCompleteProvider(_commandExecutor),
                                                    new CompositeAutoCompleteProvider(
                                                        new ProviderPathAutoCompleteProvider(_commandExecutor),
                                                        new CommandListAutoCompleteProvider(_commandExecutor)
                                                        )
                                                    )
                                            };

            _autoCompleteWalker = new AutoCompleteWalker(autoCompleteProviders);
            _historyStackWalker = new HistoryStackWalker(_commandExecutor);

            _consoleWindow.AutoCompleteWalker = _autoCompleteWalker;
            _consoleWindow.HistoryStackWalker = _historyStackWalker;

            WritePrompt();

            while (true)
            {
                WaitHandle[] handles = new[]
                                           {
                                               _host.ExitWaitHandle,
                                               _consoleWindow.CommandEnteredEvent,
                                               Queue.WaitHandle
                                           }.ToList().Where( f=>f != null).ToArray();

                int index = WaitHandle.WaitAny(handles);
                switch (index)
                {
                    case (0):
                        InvokeShouldExit(_host.ExitCode);
                        return;
                    case (1):
                        ExecuteConsoleCommand();
                        break;
                    case (2):
                        ExecuteQueuedCommand();
                        break;
                    default:
                        break;
                }
            }
        }

        private void ExecuteQueuedCommand()
        {
            var asynResult = Queue.Dequeue();
            Collection<PSObject> results = null;
            try
            {
                results = ExecuteCommand(asynResult.Command, asynResult.Parameters, asynResult.ExecutionOptions);
                asynResult.SetComplete(results, false, null);
            }
            catch (Exception e)
            {
                asynResult.SetComplete(results, false, e);
            }
        }

        private void ExecuteConsoleCommand()
        {
            _historyStackWalker.Reset();
            _autoCompleteWalker.Reset();

            var input = _consoleWindow.ReadLine();

            ExecuteCommand(input, ExecutionOptions.AddToHistory | ExecutionOptions.AddOutputter);

            WritePrompt();
        }

        private void WritePrompt()
        {
            Exception error;
            var prompt = _commandExecutor.ExecuteAndGetStringResult("prompt", out error);
            WritePrompt(prompt);
        }

        private void WritePrompt(string prompt)
        {
            if (String.IsNullOrEmpty(prompt))
            {
                return;
            }

            prompt = prompt.Trim();
            _consoleWindow.WritePrompt(prompt);
        }

        private Collection<PSObject> ExecuteCommand(string input)
        {
            return ExecuteCommand(input, ExecutionOptions.None);
        }

        private Collection<PSObject> ExecuteCommand(string input, ExecutionOptions executionOptions)
        {
            Exception error;
            var onx = CommandExecutionStateChange;
            if (null != onx)
            {
                onx(this, new EventArgs<bool>(true));
            }

            var results = _commandExecutor.ExecuteCommand(
                input,
                out error,
                executionOptions
                );

            if (null != onx)
            {
                onx(this, new EventArgs<bool>(false));
            }
            return results;
        }


        private Collection<PSObject> ExecuteCommand(string command, Dictionary<string, object> arguments,
                                                    ExecutionOptions options)
        {
            Exception error;
            var onx = CommandExecutionStateChange;
            if (null != onx)
            {
                onx(this, new EventArgs<bool>(true));
            }

            var results = _commandExecutor.ExecuteCommand(
                command,
                arguments,
                out error,
                options
                );

            if (null != onx)
            {
                onx(this, new EventArgs<bool>(false));
            }
            return results;
        }

        private void InitializeRunspaceAndHost()
        {
            if (null == _shellConfiguration.RunspaceConfiguration)
            {
                _shellConfiguration.RunspaceConfiguration = RunspaceConfiguration.Create();
            }

            _shellConfiguration.Cmdlets.ToList().ForEach(
                cce => _shellConfiguration.RunspaceConfiguration.Cmdlets.Append(cce)
                );

            _rawUi = new HostRawUI(_consoleWindow, _shellConfiguration.ShellName);
            _hostUi = new HostUI(_consoleWindow, _shellConfiguration.UISettings, _rawUi);
            _host = new Host.Host(_shellConfiguration.ShellName, _shellConfiguration.ShellVersion, _hostUi,
                                           _shellConfiguration.RunspaceConfiguration);

            _hostUi.Progress += NotifyProgress;

            _runspace = _host.Runspace;
            _runspace.Open();

            _commandExecutor = new Executor(_runspace);
            _commandExecutor.PipelineException += OnPipelineException;


            _shellConfiguration.InitialVariables.ToList().ForEach(pair =>
                                                                  _runspace.SessionStateProxy.PSVariable.Set(pair)
                );
        }

        private void OnPipelineException(object sender, EventArgs<Exception> e)
        {
            _host.UI.WriteErrorLine(e.Data.ToString());
        }

        private void NotifyProgress(object sender, ProgressRecordEventArgs e)
        {
            var ev = Progress;
            if (null == ev)
            {
                return;
            }

            ev(sender, e);
        }

        private void RunProfileScripts()
        {
            if (null == _shellConfiguration.ProfileScripts)
            {
                return;
            }

            foreach (var entry in _shellConfiguration.ProfileScripts.InRunOrder)
            {
                var fileInfo = new FileInfo(entry);
                if (! fileInfo.Exists)
                {
                    continue;
                }

                Exception error;
                _commandExecutor.ExecuteCommand(fileInfo.ToDotSource(), out error, ExecutionOptions.AddOutputter);
            }
        }

        private void RunInitializationScripts()
        {
            if (null == _shellConfiguration.RunspaceConfiguration.InitializationScripts)
            {
                return;
            }

            foreach (ScriptConfigurationEntry entry in _shellConfiguration.RunspaceConfiguration.InitializationScripts)
            {
                Exception error;
                _commandExecutor.ExecuteCommand(entry.Definition, out error, ExecutionOptions.AddOutputter);
            }
        }

        #region Nested type: StartupState

        private class StartupState
        {
            private readonly ManualResetEvent _complete;
            private Exception _exception;

            public StartupState()
            {
                _complete = new ManualResetEvent(false);
            }

            public Exception WaitForStartup()
            {
                _complete.WaitOne(1000);
                return _exception;
            }

            public void SetComplete(Exception e)
            {
                _exception = e;
                _complete.Set();
            }
        }

        #endregion
    }
}