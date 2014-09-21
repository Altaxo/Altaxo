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
	}

	[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal struct DVTARGETDEVICE
	{
		[MarshalAs(UnmanagedType.U4)]
		public int tdSize;

		[MarshalAs(UnmanagedType.U2)]
		public short tdDriverNameOffset;

		[MarshalAs(UnmanagedType.U2)]
		public short tdDeviceNameOffset;

		[MarshalAs(UnmanagedType.U2)]
		public short tdPortNameOffset;

		[MarshalAs(UnmanagedType.U2)]
		public short tdExtDevmodeOffset;

		[MarshalAs(UnmanagedType.LPArray, SizeConst = 1)]
		public byte[] tdData;
	}
}