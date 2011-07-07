using System;
using System.Runtime.Serialization;

namespace CodeOwls.PowerShell.Paths.Exceptions
{
    [Serializable]
    public class CopyOrMoveToDifferentContainerTypeException : ProviderException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public CopyOrMoveToDifferentContainerTypeException()
        {
        }

        public CopyOrMoveToDifferentContainerTypeException(string copy, string copyPath)
            : base("The item at [" + copy + "] cannot be copied to the specified location ["+copyPath+"] because the destination contains different types of items.")
        {
        }

        protected CopyOrMoveToDifferentContainerTypeException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}