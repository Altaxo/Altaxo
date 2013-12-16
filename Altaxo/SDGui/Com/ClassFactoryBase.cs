using Altaxo.UnmanagedApi.Ole32;
using System;

namespace Altaxo.Com
{
	internal class ClassFactoryBase : IClassFactory
	{
		public ClassFactoryBase()
		{
		}

		protected UInt32 m_locked = 0;
		protected uint m_ClassContext = (uint)CLSCTX.CLSCTX_LOCAL_SERVER;
		protected Guid m_ClassId;
		protected uint m_Flags;
		protected uint m_Cookie;

		public virtual void virtual_CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
		{
			IntPtr nullPtr = new IntPtr(0);
			ppvObject = nullPtr;
		}

		public uint ClassContext
		{
			get
			{
				return m_ClassContext;
			}
			set
			{
				m_ClassContext = value;
			}
		}

		public Guid ClassId
		{
			get
			{
				return m_ClassId;
			}
			set
			{
				m_ClassId = value;
			}
		}

		public uint Flags
		{
			get
			{
				return m_Flags;
			}
			set
			{
				m_Flags = value;
			}
		}

		public bool RegisterClassObject()
		{
			// Register the class factory
			int i = Ole32Func.CoRegisterClassObject
				(
				ref m_ClassId,
				this,
				ClassContext,
				Flags,
				out m_Cookie
				);

			if (i == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool RevokeClassObject()
		{
			int i = Ole32Func.CoRevokeClassObject(m_Cookie);

			if (i == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool ResumeClassObjects()
		{
			int i = Ole32Func.CoResumeClassObjects();

			if (i == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#region IClassFactory Implementations

		public void CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
		{
			virtual_CreateInstance(pUnkOuter, ref riid, out ppvObject);
		}

		public void LockServer(bool bLock)
		{
			if (bLock)
			{
				ComManager.InterlockedIncrementServerLockCount();
			}
			else
			{
				ComManager.InterlockedDecrementServerLockCount();
			}

			// Always attempt to see if we need to shutdown this server application.
			ComManager.AttemptToTerminateServer();
		}

		#endregion IClassFactory Implementations
	}
}