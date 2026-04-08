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
  /// <summary>
  /// Defines binding speed preferences.
  /// </summary>
  public enum BINDSPEED : int
  {
    /// <summary>Indefinite binding speed.</summary>
    BINDSPEED_INDEFINITE = 1,
    /// <summary>Moderate binding speed.</summary>
    BINDSPEED_MODERATE = 2,
    /// <summary>Immediate binding speed.</summary>
    BINDSPEED_IMMEDIATE = 3
  };

  /// <summary>
  /// Defines COM initialization options for a thread.
  /// </summary>
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

  /// <summary>
  /// Defines class context values for COM object activation.
  /// </summary>
  [Flags]
  public enum CLSCTX : uint
  {
    /// <summary>In-process server.</summary>
    CLSCTX_INPROC_SERVER = 0x1,
    /// <summary>In-process handler.</summary>
    CLSCTX_INPROC_HANDLER = 0x2,
    /// <summary>Local server.</summary>
    CLSCTX_LOCAL_SERVER = 0x4,
    /// <summary>16-bit in-process server.</summary>
    CLSCTX_INPROC_SERVER16 = 0x8,
    /// <summary>Remote server.</summary>
    CLSCTX_REMOTE_SERVER = 0x10,
    /// <summary>16-bit in-process handler.</summary>
    CLSCTX_INPROC_HANDLER16 = 0x20,
    /// <summary>Reserved value 1.</summary>
    CLSCTX_RESERVED1 = 0x40,
    /// <summary>Reserved value 2.</summary>
    CLSCTX_RESERVED2 = 0x80,
    /// <summary>Reserved value 3.</summary>
    CLSCTX_RESERVED3 = 0x100,
    /// <summary>Reserved value 4.</summary>
    CLSCTX_RESERVED4 = 0x200,
    /// <summary>Disables code download.</summary>
    CLSCTX_NO_CODE_DOWNLOAD = 0x400,
    /// <summary>Reserved value 5.</summary>
    CLSCTX_RESERVED5 = 0x800,
    /// <summary>Disables custom marshaling.</summary>
    CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
    /// <summary>Enables code download.</summary>
    CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
    /// <summary>Disables failure logging.</summary>
    CLSCTX_NO_FAILURE_LOG = 0x4000,
    /// <summary>Disables activate-as-activator.</summary>
    CLSCTX_DISABLE_AAA = 0x8000,
    /// <summary>Enables activate-as-activator.</summary>
    CLSCTX_ENABLE_AAA = 0x10000,
    /// <summary>Uses the default context.</summary>
    CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
    /// <summary>Activates a 32-bit server.</summary>
    CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000,
    /// <summary>Activates a 64-bit server.</summary>
    CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
    /// <summary>Enables cloaking.</summary>
    CLSCTX_ENABLE_CLOAKING = 0x100000,
    /// <summary>Runs in an app container.</summary>
    CLSCTX_APPCONTAINER = 0x400000,
    /// <summary>All in-process contexts.</summary>
    CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
    /// <summary>All server contexts.</summary>
    CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
    /// <summary>All contexts.</summary>
    CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
  }

  /// <summary>
  /// Defines COM class registration options.
  /// </summary>
  [Flags]
  public enum REGCLS : uint
  {
    /// <summary>Single-use class object.</summary>
    REGCLS_SINGLEUSE = 0,
    /// <summary>Multiple-use class object.</summary>
    REGCLS_MULTIPLEUSE = 1,
    /// <summary>Multiple-use class object with separate registration.</summary>
    REGCLS_MULTI_SEPARATE = 2,
    /// <summary>Suspends class registration.</summary>
    REGCLS_SUSPENDED = 4,
    /// <summary>Registers a surrogate.</summary>
    REGCLS_SURROGATE = 8
  }

  /// <summary>
  /// Defines common COM and OLE return values.
  /// </summary>
  public static class ComReturnValue
  {
    /// <summary>No error.</summary>
    public const int NOERROR = 0;
    /// <summary>Operation successful.</summary>
    public const int S_OK = 0;
    /// <summary>Operation returned false but succeeded.</summary>
    public const int S_FALSE = 1;
    /// <summary>Invalid OLE verb.</summary>
    public const int OLEOBJ_S_INVALIDVERB = 0x00040180;
    /// <summary>Use registry information.</summary>
    public const int OLE_S_USEREG = 0x00040000;
    /// <summary>Unspecified failure.</summary>
    public const int E_FAIL = unchecked((int)0x80004005);
    /// <summary>Not implemented.</summary>
    public const int E_NOTIMPL = unchecked((int)0x80004001);
    /// <summary>No such interface supported.</summary>
    public const int E_NOINTERFACE = unchecked((int)0x80004002);
    /// <summary>The deadline was exceeded.</summary>
    public const int MK_E_EXCEEDEDDEADLINE = unchecked((int)0x800401E1); // (MK_E_FIRST + 1)
    /// <summary>No object is available.</summary>
    public const int MK_E_NOOBJECT = unchecked((int)0x800401E5); // (MK_E_FIRST + 5)
    /// <summary>No storage is available.</summary>
    public const int MK_E_NOSTORAGE = unchecked((int)0x800401ED); //  (MK_E_FIRST + 13)
    /// <summary>Invalid data format.</summary>
    public const int DATA_E_FORMATETC = unchecked((int)0x80040064);
    /// <summary>Advising is not supported.</summary>
    public const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);
    /// <summary>Invalid data format.</summary>
    public const int DV_E_FORMATETC = unchecked((int)0x80040064);
    /// <summary>Invalid storage medium.</summary>
    public const int DV_E_TYMED = unchecked((int)0x80040069);
    /// <summary>Invalid clipboard format.</summary>
    public const int DV_E_CLIPFORMAT = unchecked((int)0x8004006A);
    /// <summary>Same format was already advised.</summary>
    public const int DATA_S_SAMEFORMATETC = unchecked(0x00040130);

    /// <summary>Invalid data aspect.</summary>
    public const int DV_E_DVASPECT = unchecked((int)0x8004006B);

    /// <summary>The user was prompted to save but chose the Cancel button from the prompt message box.</summary>
    public const int OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C);
  }

  /// <summary>
  /// Defines standard OLE verbs.
  /// </summary>
  public enum OLEIVERB
  {
    /// <summary>The primary verb.</summary>
    OLEIVERB_PRIMARY = 0,
    /// <summary>Show the object.</summary>
    OLEIVERB_SHOW = -1,
    /// <summary>Open the object.</summary>
    OLEIVERB_OPEN = -2,
    /// <summary>Hide the object.</summary>
    OLEIVERB_HIDE = -3,
    /// <summary>Activate the user interface.</summary>
    OLEIVERB_UIACTIVATE = -4,
    /// <summary>Activate the object in-place.</summary>
    OLEIVERB_INPLACEACTIVATE = -5,
    /// <summary>Discard the undo state.</summary>
    OLEIVERB_DISCARDUNDOSTATE = -6,
  }

  /// <summary>
  /// Defines miscellaneous OLE object status flags.
  /// </summary>
  [Flags]
  public enum OLEMISC : int
  {
    /// <summary>Recompose on resize.</summary>
    OLEMISC_RECOMPOSEONRESIZE = 1,
    /// <summary>Only iconic.</summary>
    OLEMISC_ONLYICONIC = 2,
    /// <summary>Insert, not replace.</summary>
    OLEMISC_INSERTNOTREPLACE = 4,
    /// <summary>Static object.</summary>
    OLEMISC_STATIC = 8,
    /// <summary>Cannot link inside.</summary>
    OLEMISC_CANTLINKINSIDE = 0x10,
    /// <summary>Can link by OLE 1.</summary>
    OLEMISC_CANLINKBYOLE1 = 0x20,
    /// <summary>Is a link object.</summary>
    OLEMISC_ISLINKOBJECT = 0x40,
    /// <summary>Supports inside-out activation.</summary>
    OLEMISC_INSIDEOUT = 0x80,
    /// <summary>Activates when visible.</summary>
    OLEMISC_ACTIVATEWHENVISIBLE = 0x100,
    /// <summary>Rendering is device-independent.</summary>
    OLEMISC_RENDERINGISDEVICEINDEPENDENT = 0x200,
    /// <summary>Invisible at runtime.</summary>
    OLEMISC_INVISIBLEATRUNTIME = 0x400,
    /// <summary>Always run.</summary>
    OLEMISC_ALWAYSRUN = 0x800,
    /// <summary>Acts like a button.</summary>
    OLEMISC_ACTSLIKEBUTTON = 0x1000,
    /// <summary>Acts like a label.</summary>
    OLEMISC_ACTSLIKELABEL = 0x2000,
    /// <summary>No UI activation.</summary>
    OLEMISC_NOUIACTIVATE = 0x4000,
    /// <summary>Alignable.</summary>
    OLEMISC_ALIGNABLE = 0x8000,
    /// <summary>Simple frame.</summary>
    OLEMISC_SIMPLEFRAME = 0x10000,
    /// <summary>Set client site first.</summary>
    OLEMISC_SETCLIENTSITEFIRST = 0x20000,
    /// <summary>IME mode supported.</summary>
    OLEMISC_IMEMODE = 0x40000,
    /// <summary>Ignores activate-when-visible.</summary>
    OLEMISC_IGNOREACTIVATEWHENVISIBLE = 0x80000,
    /// <summary>Wants to merge menus.</summary>
    OLEMISC_WANTSTOMENUMERGE = 0x100000,
    /// <summary>Supports multilevel undo.</summary>
    OLEMISC_SUPPORTSMULTILEVELUNDO = 0x200000
  }

  /// <summary>
  /// Defines storage mode flags.
  /// </summary>
  [Flags]
  public enum STGM
  {
    // Access
    /// <summary>Read access.</summary>
    READ = 0x00000000,

    /// <summary>Write access.</summary>
    WRITE = 0x00000001,
    /// <summary>Read/write access.</summary>
    READWRITE = 0x00000002,

    // Sharing
    /// <summary>Do not deny sharing.</summary>
    SHARE_DENY_NONE = 0x00000040,

    /// <summary>Deny read sharing.</summary>
    SHARE_DENY_READ = 0x00000030,
    /// <summary>Deny write sharing.</summary>
    SHARE_DENY_WRITE = 0x00000020,
    /// <summary>Deny all sharing.</summary>
    SHARE_EXCLUSIVE = 0x00000010,
    /// <summary>Priority mode.</summary>
    PRIORITY = 0x00040000,

    // Creation
    /// <summary>Create a new storage object.</summary>
    CREATE = 0x00001000,

    /// <summary>Convert existing storage.</summary>
    CONVERT = 0x00020000,
    /// <summary>Fail if the object already exists.</summary>
    FAILIFTHERE = 0x00000000,

    // Transactioning
    /// <summary>Direct mode.</summary>
    DIRECT = 0x00000000,

    /// <summary>Transacted mode.</summary>
    TRANSACTED = 0x00010000,

    // Transactioning Performance
    /// <summary>No scratch file.</summary>
    NOSCRATCH = 0x00100000,

    /// <summary>No snapshot.</summary>
    NOSNAPSHOT = 0x00200000,

    // Direct SWMR and Simple
    /// <summary>Simple mode.</summary>
    SIMPLE = 0x08000000,

    /// <summary>Direct single-writer multiple-reader mode.</summary>
    DIRECT_SWMR = 0x00400000,

    // Delete On Release
    /// <summary>Delete on release.</summary>
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

  /// <summary>
  /// Defines clipboard format identifiers.
  /// </summary>
  public static class CF
  {
    /// <summary>Text format.</summary>
    public const short CF_TEXT = 1;
    /// <summary>Bitmap format.</summary>
    public const short CF_BITMAP = 2;
    /// <summary>Metafile picture format.</summary>
    public const short CF_METAFILEPICT = 3;
    /// <summary>SYLK format.</summary>
    public const short CF_SYLK = 4;
    /// <summary>DIF format.</summary>
    public const short CF_DIF = 5;
    /// <summary>TIFF format.</summary>
    public const short CF_TIFF = 6;
    /// <summary>OEM text format.</summary>
    public const short CF_OEMTEXT = 7;
    /// <summary>DIB format.</summary>
    public const short CF_DIB = 8;
    /// <summary>Palette format.</summary>
    public const short CF_PALETTE = 9;
    /// <summary>Pen data format.</summary>
    public const short CF_PENDATA = 10;
    /// <summary>RIFF format.</summary>
    public const short CF_RIFF = 11;
    /// <summary>Wave format.</summary>
    public const short CF_WAVE = 12;
    /// <summary>Unicode text format.</summary>
    public const short CF_UNICODETEXT = 13;
    /// <summary>Enhanced metafile format.</summary>
    public const short CF_ENHMETAFILE = 14;
    /// <summary>File drop format.</summary>
    public const short CF_HDROP = 15;
    /// <summary>Locale format.</summary>
    public const short CF_LOCALE = 16;
    /// <summary>DIB version 5 format.</summary>
    public const short CF_DIBV5 = 17;
    /// <summary>Owner-display format.</summary>
    public const short CF_OWNERDISPLAY = 0x80;
    /// <summary>Displayed text format.</summary>
    public const short CF_DSPTEXT = 0x81;
    /// <summary>Displayed bitmap format.</summary>
    public const short CF_DSPBITMAP = 0x82;
    /// <summary>Displayed metafile picture format.</summary>
    public const short CF_DSPMETAFILEPICT = 0x83;
    /// <summary>Displayed enhanced metafile format.</summary>
    public const short CF_DSPENHMETAFILE = 0x8E;
  }

  /// <summary>
  /// Defines standard clipboard format names.
  /// </summary>
  public static class CFSTR
  {
    /// <summary>Embedded object format name.</summary>
    public const string CFSTR_EMBEDDEDOBJECT = "Embedded Object";
    /// <summary>Embed source format name.</summary>
    public const string CFSTR_EMBEDSOURCE = "Embed Source";
    /// <summary>Link source format name.</summary>
    public const string CFSTR_LINKSOURCE = "Link Source";
    /// <summary>Object descriptor format name.</summary>
    public const string CFSTR_OBJECTDESCRIPTOR = "Object Descriptor";
    /// <summary>Link source descriptor format name.</summary>
    public const string CFSTR_LINKSRCDESCRIPTOR = "Link Source Descriptor";
  }
}
