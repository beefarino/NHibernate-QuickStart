using System;
using System.Management.Automation;

namespace CodeOwls.PowerShell.Host
{
    public class ProgressRecordEventArgs : EventArgs
    {
        private readonly ProgressRecord _record;
        private readonly long _sourceId;


        public ProgressRecordEventArgs(long sourceId, ProgressRecord record)
        {
            _sourceId = sourceId;
            _record = record;
        }

        public ProgressRecord ProgressRecord
        {
            get { return _record; }
        }

        public long SourceId
        {
            get { return _sourceId; }
        }
    }
}