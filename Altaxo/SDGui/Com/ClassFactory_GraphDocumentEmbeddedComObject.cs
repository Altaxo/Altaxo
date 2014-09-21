#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Runtime.InteropServices;
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

		public override void InternalCreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject)
		{
			ComDebug.ReportInfo("{0}.CreateInstance(), requesting interface {1}", this.GetType().Name, riid);

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