
// Neccessary in order to use records in .NET framework
#if NETFRAMEWORK
namespace System.Runtime.CompilerServices
    {
        using System.ComponentModel;
        /// <summary>
        /// Reserved to be used by the compiler for tracking metadata.
        /// This class should not be used by developers in source code.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static class IsExternalInit
        {
        }
    }
#endif
