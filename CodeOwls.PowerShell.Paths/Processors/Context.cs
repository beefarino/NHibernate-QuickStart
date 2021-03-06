/*
   Copyright (c) 2011 Code Owls LLC, All Rights Reserved.

   Licensed under the Microsoft Reciprocal License (Ms-RL) (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.opensource.org/licenses/ms-rl

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Provider;
using System.Security.AccessControl;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.PowerShell.Provider.PathNodeProcessors
{
    public class Context : IContext
    {
        private readonly CmdletProvider _provider;

        public Context(CmdletProvider provider, IPathNodeProcessor pathProcessor, object dynamicParameters)
        {
            PathProcessor = pathProcessor;
            DynamicParameters = dynamicParameters;
            _provider = provider;
        }

        public Context( IContext context, object dynamicParameters )
        {
            Context c = context as Context;
            if( null == c )
            {
                throw new ArgumentException( "the context provided is of an incompatible type");
            }

            _provider = c._provider;
            PathProcessor = c.PathProcessor;
            DynamicParameters = dynamicParameters;
        }

        public string GetResourceString(string baseName, string resourceId)
        {
            return _provider.GetResourceString(baseName, resourceId);
        }

        public void ThrowTerminatingError(ErrorRecord errorRecord)
        {
            _provider.ThrowTerminatingError(errorRecord);
        }

        public bool ShouldProcess(string target)
        {
            return _provider.ShouldProcess(target);
        }

        public bool ShouldProcess(string target, string action)
        {
            return _provider.ShouldProcess(target, action);
        }

        public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption)
        {
            return _provider.ShouldProcess(verboseDescription, verboseWarning, caption);
        }

        public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out ShouldProcessReason shouldProcessReason)
        {
            return _provider.ShouldProcess(verboseDescription, verboseWarning, caption, out shouldProcessReason);
        }

        public bool ShouldContinue(string query, string caption)
        {
            return _provider.ShouldContinue(query, caption);
        }

        public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll)
        {
            return _provider.ShouldContinue(query, caption, ref yesToAll, ref noToAll);
        }

        public bool TransactionAvailable()
        {
            return _provider.TransactionAvailable();
        }

        public void WriteVerbose(string text)
        {
            _provider.WriteVerbose(text);
        }

        public void WriteWarning(string text)
        {
            _provider.WriteWarning(text);
        }

        public void WriteProgress(ProgressRecord progressRecord)
        {
            _provider.WriteProgress(progressRecord);
        }

        public void WriteDebug(string text)
        {
            _provider.WriteDebug(text);
        }

        public void WriteItemObject(object item, string path, bool isContainer)
        {
            _provider.WriteItemObject(item, path, isContainer);
        }

        public void WritePropertyObject(object propertyValue, string path)
        {
            _provider.WritePropertyObject(propertyValue, path);
        }

        public void WriteSecurityDescriptorObject(ObjectSecurity securityDescriptor, string path)
        {
            _provider.WriteSecurityDescriptorObject(securityDescriptor, path);
        }

        public void WriteError(ErrorRecord errorRecord)
        {
            _provider.WriteError(errorRecord);
        }

        public bool Stopping
        {
            get { return _provider.Stopping; }
        }

        public SessionState SessionState
        {
            get { return _provider.SessionState; }
        }

        public ProviderIntrinsics InvokeProvider
        {
            get { return _provider.InvokeProvider; }
        }

        public CommandInvocationIntrinsics InvokeCommand
        {
            get { return _provider.InvokeCommand; }
        }

        public PSCredential Credential
        {
            get { return _provider.Credential; }
        }

        public bool Force
        {
            get { return _provider.Force.IsPresent; }
        }

        public string Filter
        {
            get { return _provider.Filter; }
        }

        public IEnumerable<string> Include
        {
            get { return _provider.Include; }
        }

        public IEnumerable<string> Exclude
        {
            get { return _provider.Exclude; }
        }

        public object DynamicParameters
        {
            get; 
            private set;
        }

        public IPathNodeProcessor PathProcessor { get; private set; }
    }
}
