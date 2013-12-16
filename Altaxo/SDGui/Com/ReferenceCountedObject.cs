using System;
using System.Runtime.InteropServices;

namespace Altaxo.Com
{
	/// <summary>
	/// Summary description for ReferenceCountedObjectBase.
	/// </summary>
	[ComVisible(false)]  // This ComVisibleAttribute is set to false so that TLBEXP and REGASM will not expose it nor COM-register it.
	public class ReferenceCountedObjectBase
	{
		public ReferenceCountedObjectBase()
		{
			ComManager.InterlockedIncrementObjectsCount();
		}

		~ReferenceCountedObjectBase()
		{
			ComManager.InterlockedDecrementObjectsCount();
			ComManager.AttemptToTerminateServer();
		}
	}
}