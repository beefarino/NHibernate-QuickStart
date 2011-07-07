using System;
using System.Runtime.Serialization;

namespace CodeOwls.PowerShell.Paths.Exceptions
{
    [Serializable]
    public class CopyOrMoveItemInternalException : ProviderException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public CopyOrMoveItemInternalException()
        {

        }

        public CopyOrMoveItemInternalException(string path, string copyPath, Exception e)
            : base( String.Format("An internal error occurred attempting to copy or move the item at [{0}] to [{1}].", path, copyPath), e )
        {
        }

        protected CopyOrMoveItemInternalException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}