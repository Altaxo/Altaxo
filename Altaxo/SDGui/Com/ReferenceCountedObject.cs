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
		protected ComManager _comManager;
		public ReferenceCountedObjectBase(ComManager comManager)
		{
			_comManager = comManager;
			_comManager.InterlockedIncrementObjectsCount();
		}

		~ReferenceCountedObjectBase()
		{
			_comManager.InterlockedDecrementObjectsCount();
			_comManager.AttemptToTerminateServer();
		}
	}
}