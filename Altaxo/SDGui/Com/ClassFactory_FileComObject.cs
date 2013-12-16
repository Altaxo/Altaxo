using System;
using System.Runtime.InteropServices;  // For use of the GuidAttribute, ProgIdAttribute and ClassInterfaceAttribute.
using System.Text;

namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;

	internal class ClassFactory_FileCOMObject : ClassFactoryBase
	{
		public ClassFactory_FileCOMObject(ComManager comManager) 
			: base(comManager)
		{
		}

		public override void virtual_CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
		{
#if COMLOGGING
			Debug.ReportInfo("FileCOMObjectClassFactory.CreateInstance()");
			Debug.ReportInfo("Requesting Interface : {0}", riid);
#endif

			if (riid == Marshal.GenerateGuidForType(typeof(System.Runtime.InteropServices.ComTypes.IPersistFile)) ||
					riid == Marshal.GenerateGuidForType(typeof(IOleItemContainer)) ||
				riid == InterfaceGuid.IID_IDispatch ||
				riid == InterfaceGuid.IID_IUnknown)
			{
				if (null == _comManager.FileComObject)
					_comManager.FileComObject = new FileComObject(_comManager);

				ppvObject = Marshal.GetComInterfaceForObject(_comManager.FileComObject, typeof(System.Runtime.InteropServices.ComTypes.IPersistFile)); ;
			}
			else
			{
				throw new COMException("No interface", unchecked((int)0x80004002));
			}
		}
	}
}