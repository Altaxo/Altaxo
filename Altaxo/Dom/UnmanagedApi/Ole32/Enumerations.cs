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
using System.Linq;
using System.Text;

namespace Altaxo.UnmanagedApi.Ole32
{
	public enum BINDSPEED : int
	{
		BINDSPEED_INDEFINITE = 1,
		BINDSPEED_MODERATE = 2,
		BINDSPEED_IMMEDIATE = 3
	};

	[Flags]
	public enum COINIT : uint
	{
		/// Initializes the thread for multi-threaded object concurrency.
		COINIT_MULTITHREADED = 0x0,

		/// Initializes the thread for apartment-threaded object concurrency.
		COINIT_APARTMENTTHREADED = 0x2,

		/// Disables DDE for Ole1 support.
		COINIT_DISABLE_OLE1DDE = 0x4,

		/// Trades memory for speed.
		COINIT_SPEED_OVER_MEMORY = 0x8
	}

	[Flags]
	public enum CLSCTX : uint
	{
		CLSCTX_INPROC_SERVER = 0x1,
		CLSCTX_INPROC_HANDLER = 0x2,
		CLSCTX_LOCAL_SERVER = 0x4,
		CLSCTX_INPROC_SERVER16 = 0x8,
		CLSCTX_REMOTE_SERVER = 0x10,
		CLSCTX_INPROC_HANDLER16 = 0x20,
		CLSCTX_RESERVED1 = 0x40,
		CLSCTX_RESERVED2 = 0x80,
		CLSCTX_RESERVED3 = 0x100,
		CLSCTX_RESERVED4 = 0x200,
		CLSCTX_NO_CODE_DOWNLOAD = 0x400,
		CLSCTX_RESERVED5 = 0x800,
		CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
		CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
		CLSCTX_NO_FAILURE_LOG = 0x4000,
		CLSCTX_DISABLE_AAA = 0x8000,
		CLSCTX_ENABLE_AAA = 0x10000,
		CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
		CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000,
		CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
		CLSCTX_ENABLE_CLOAKING = 0x100000,
		CLSCTX_APPCONTAINER = 0x400000,
		CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
		CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
		CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
	}

	[Flags]
	public enum REGCLS : uint
	{
		REGCLS_SINGLEUSE = 0,
		REGCLS_MULTIPLEUSE = 1,
		REGCLS_MULTI_SEPARATE = 2,
		REGCLS_SUSPENDED = 4,
		REGCLS_SURROGATE = 8
	}

	public static class ComReturnValue
	{
		public const int NOERROR = 0;
		public const int S_OK = 0;
		public const int S_FALSE = 1;
		public const int OLEOBJ_S_INVALIDVERB = 0x00040180;
		public const int OLE_S_USEREG = 0x00040000;
		public const int E_FAIL = unchecked((int)0x80004005);
		public const int E_NOTIMPL = unchecked((int)0x80004001);
		public const int E_NOINTERFACE = unchecked((int)0x80004002);
		public const int MK_E_EXCEEDEDDEADLINE = unchecked((int)0x800401E1); // (MK_E_FIRST + 1)
		public const int MK_E_NOOBJECT = unchecked((int)0x800401E5); // (MK_E_FIRST + 5)
		public const int MK_E_NOSTORAGE = unchecked((int)0x800401ED); //  (MK_E_FIRST + 13)
		public const int DATA_E_FORMATETC = unchecked((int)0x80040064);
		public const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);
		public const int DV_E_FORMATETC = unchecked((int)0x80040064);
		public const int DV_E_TYMED = unchecked((int)0x80040069);
		public const int DV_E_CLIPFORMAT = unchecked((int)0x8004006A);
		public const int DATA_S_SAMEFORMATETC = unchecked((int)0x00040130);

		public const int DV_E_DVASPECT = unchecked((int)0x8004006B);

		/// <summary>The user was prompted to save but chose the Cancel button from the prompt message box.</summary>
		public const int OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C);
	}

	public enum OLEIVERB
	{
		OLEIVERB_PRIMARY = 0,
		OLEIVERB_SHOW = -1,
		OLEIVERB_OPEN = -2,
		OLEIVERB_HIDE = -3,
		OLEIVERB_UIACTIVATE = -4,
		OLEIVERB_INPLACEACTIVATE = -5,
		OLEIVERB_DISCARDUNDOSTATE = -6,
	}

	[Flags]
	public enum OLEMISC : int
	{
		OLEMISC_RECOMPOSEONRESIZE = 1,
		OLEMISC_ONLYICONIC = 2,
		OLEMISC_INSERTNOTREPLACE = 4,
		OLEMISC_STATIC = 8,
		OLEMISC_CANTLINKINSIDE = 0x10,
		OLEMISC_CANLINKBYOLE1 = 0x20,
		OLEMISC_ISLINKOBJECT = 0x40,
		OLEMISC_INSIDEOUT = 0x80,
		OLEMISC_ACTIVATEWHENVISIBLE = 0x100,
		OLEMISC_RENDERINGISDEVICEINDEPENDENT = 0x200,
		OLEMISC_INVISIBLEATRUNTIME = 0x400,
		OLEMISC_ALWAYSRUN = 0x800,
		OLEMISC_ACTSLIKEBUTTON = 0x1000,
		OLEMISC_ACTSLIKELABEL = 0x2000,
		OLEMISC_NOUIACTIVATE = 0x4000,
		OLEMISC_ALIGNABLE = 0x8000,
		OLEMISC_SIMPLEFRAME = 0x10000,
		OLEMISC_SETCLIENTSITEFIRST = 0x20000,
		OLEMISC_IMEMODE = 0x40000,
		OLEMISC_IGNOREACTIVATEWHENVISIBLE = 0x80000,
		OLEMISC_WANTSTOMENUMERGE = 0x100000,
		OLEMISC_SUPPORTSMULTILEVELUNDO = 0x200000
	}

	[Flags]
	public enum STGM
	{
		// Access
		READ = 0x00000000,

		WRITE = 0x00000001,
		READWRITE = 0x00000002,

		// Sharing
		SHARE_DENY_NONE = 0x00000040,

		SHARE_DENY_READ = 0x00000030,
		SHARE_DENY_WRITE = 0x00000020,
		SHARE_EXCLUSIVE = 0x00000010,
		PRIORITY = 0x00040000,

		// Creation
		CREATE = 0x00001000,

		CONVERT = 0x00020000,
		FAILIFTHERE = 0x00000000,

		// Transactioning
		DIRECT = 0x00000000,

		TRANSACTED = 0x00010000,

		// Transactioning Performance
		NOSCRATCH = 0x00100000,

		NOSNAPSHOT = 0x00200000,

		// Direct SWMR and Simple
		SIMPLE = 0x08000000,

		DIRECT_SWMR = 0x00400000,

		// Delete On Release
		DELETEONRELEASE = 0x04000000
	}

	/// <summary>
	/// Indicates whether an object should be saved before closing.
	/// </summary>
	public enum tagOLECLOSE : int
	{
		/// <summary>
		/// The object should be saved if it is dirty.
		/// </summary>
		OLECLOSE_SAVEIFDIRTY = 0,

		/// <summary>
		/// The object should not be saved, even if it is dirty.
		/// This flag is typically used when an object is being deleted.
		/// </summary>
		OLECLOSE_NOSAVE = 1,

		/// <summary>
		/// If the object is dirty, the IOleObject.Close implementation should
		/// display a dialog box to let the end user determine whether to save the object.
		/// However, if the object is in the running state but its user interface is invisible,
		/// the end user should not be prompted, and the close should be handled as if
		/// OLECLOSE_SAVEIFDIRTY had been specified.
		/// </summary>
		OLECLOSE_PROMPTSAVE = 2
	};

	public static class CF
	{
		public const short CF_TEXT = 1;
		public const short CF_BITMAP = 2;
		public const short CF_METAFILEPICT = 3;
		public const short CF_SYLK = 4;
		public const short CF_DIF = 5;
		public const short CF_TIFF = 6;
		public const short CF_OEMTEXT = 7;
		public const short CF_DIB = 8;
		public const short CF_PALETTE = 9;
		public const short CF_PENDATA = 10;
		public const short CF_RIFF = 11;
		public const short CF_WAVE = 12;
		public const short CF_UNICODETEXT = 13;
		public const short CF_ENHMETAFILE = 14;
		public const short CF_HDROP = 15;
		public const short CF_LOCALE = 16;
		public const short CF_DIBV5 = 17;
		public const short CF_OWNERDISPLAY = 0x80;
		public const short CF_DSPTEXT = 0x81;
		public const short CF_DSPBITMAP = 0x82;
		public const short CF_DSPMETAFILEPICT = 0x83;
		public const short CF_DSPENHMETAFILE = 0x8E;
	}

	public static class CFSTR
	{
		public const string CFSTR_EMBEDDEDOBJECT = "Embedded Object";
		public const string CFSTR_EMBEDSOURCE = "Embed Source";
		public const string CFSTR_LINKSOURCE = "Link Source";
		public const string CFSTR_OBJECTDESCRIPTOR = "Object Descriptor";
		public const string CFSTR_LINKSRCDESCRIPTOR = "Link Source Descriptor";
	}
}