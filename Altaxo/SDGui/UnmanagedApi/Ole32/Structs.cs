using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.UnmanagedApi.Ole32
{
	[StructLayout(LayoutKind.Sequential)]
	public struct OBJECTDESCRIPTOR
	{
		public int cbSize;
		public Guid clsid;
		public DVASPECT dwDrawAspect;
		public int sizelcx, sizelcy;
		public int pointlx, pointly;
		public int dwStatus;
		public int dwFullUserTypeName;
		public int dwSrcOfCopy;
		/* variable sized string data may appear here */
	};
}