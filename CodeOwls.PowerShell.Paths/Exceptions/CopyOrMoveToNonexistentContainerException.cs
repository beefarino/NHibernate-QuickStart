using System;
using System.Runtime.Serialization;

namespace CodeOwls.PowerShell.Paths.Exceptions
{
    [Serializable]
    public class CopyOrMoveToNonexistentContainerException : ProviderException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public CopyOrMoveToNonexistentContainerException()
        {           
        }

        public CopyOrMoveToNonexistentContainerException(string copyPath)
            : base("No item container exists at [" + copyPath + "], you cannot copy or move items there.")
        {
        }

        protected CopyOrMoveToNonexistentContainerException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}