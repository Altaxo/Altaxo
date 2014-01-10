using System;
using System.Runtime.InteropServices;  // For use of the GuidAttribute, ProgIdAttribute and ClassInterfaceAttribute.
using System.Text;

namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;

	internal class ClassFactory_ProjectFileComObject : ClassFactoryBase
	{
		public ClassFactory_ProjectFileComObject(ComManager comManager)
			: base(comManager)
		{
		}

		public override void virtual_CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.CreateInstance(), requesting interface {1}", this.GetType().Name, riid);
#endif

			if (riid == Marshal.GenerateGuidForType(typeof(System.Runtime.InteropServices.ComTypes.IPersistFile)) ||
					riid == Marshal.GenerateGuidForType(typeof(IOleItemContainer)) ||
				riid == InterfaceGuid.IID_IDispatch ||
				riid == InterfaceGuid.IID_IUnknown)
			{
				// notify the ComManager that we are about to enter linked object mode.
				_comManager.EnterLinkedObjectMode();
				ppvObject = Marshal.GetComInterfaceForObject(_comManager.FileComObject, typeof(System.Runtime.InteropServices.ComTypes.IPersistFile));
			}
			else
			{
				throw new COMException("No interface", unchecked((int)0x80004002));
			}
		}
	}
}