using System;
using System.Runtime.InteropServices;  // For use of the GuidAttribute, ProgIdAttribute and ClassInterfaceAttribute.
using System.Text;

namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;

	internal class ClassFactory_GraphDocumentEmbeddedComObject : ClassFactoryBase
	{
		public ClassFactory_GraphDocumentEmbeddedComObject(ComManager comManager)
			: base(comManager)
		{
		}

		public override void virtual_CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
		{
#if COMLOGGING
			Debug.ReportInfo("{0}.CreateInstance(), requesting interface {1}", this.GetType().Name, riid);
#endif

			if (riid == Marshal.GenerateGuidForType(typeof(System.Runtime.InteropServices.ComTypes.IDataObject)) ||
				riid == InterfaceGuid.IID_IDispatch ||
				riid == InterfaceGuid.IID_IUnknown)
			{
				var documentComObject = _comManager.GetNewEmbeddedGraphDocumentComObject();

				ppvObject = Marshal.GetComInterfaceForObject(documentComObject, typeof(System.Runtime.InteropServices.ComTypes.IDataObject));
			}
			else
			{
				throw new COMException("No interface", unchecked((int)0x80004002));
			}
		}
	}
}