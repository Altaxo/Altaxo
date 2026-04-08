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
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.UnmanagedApi.Ole32
{
  /// <summary>
  /// Provides unmanaged Ole32 function imports.
  /// </summary>
  public static class Ole32Func
  {
    /// <summary>
    /// Initializes the COM library for the calling thread.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int CoInitializeEx(IntPtr pvReserved, uint dwCoInit);

    /// <summary>
    /// Uninitializes the COM library on the current thread.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern void CoUninitialize();

    /// <summary>
    /// Registers a class factory with COM.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int CoRegisterClassObject([In] ref Guid rclsid,
      [MarshalAs(UnmanagedType.IUnknown)] object pUnk, uint dwClsContext,
      uint flags, out uint lpdwRegister);

    /// <summary>
    /// Informs COM that all registered class objects are ready to receive activation requests.
    /// </summary>
    /// <remarks>
    /// Called by an COM EXE Server that can register multiple class objects
    /// to inform COM about all registered classes, and permits activation
    /// requests for those class objects.
    /// This function causes OLE to inform the SCM about all the registered
    /// classes, and begins letting activation requests into the server process.
    /// </remarks>
    [DllImport("ole32.dll")]
    public static extern int CoResumeClassObjects();

    /// <summary>
    /// Informs OLE that a class object, previously registered with the CoRegisterClassObject function, is no longer available for use.
    /// It is used to unregister a Class Factory from COM's internal table of Class Factories.
    /// </summary>
    /// <param name="dwRegister">A token previously returned from the CoRegisterClassObject function.</param>
    /// <returns>This function can return the standard return values E_INVALIDARG, E_OUTOFMEMORY, and E_UNEXPECTED, as well as the following values.
    /// <code>
    /// S_OK : The class object was revoked successfully.
    /// </code>
    /// </returns>
    [DllImport("ole32.dll")]
    public static extern int CoRevokeClassObject(uint dwRegister);

    /// <summary>
    /// Disconnects an object from all its external connections.
    /// </summary>
    /// <param name="pUnk">The object to disconnect.</param>
    /// <param name="dwReserved">Reserved. Must be zero.</param>
    [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
    public static extern void CoDisconnectObject([MarshalAs(UnmanagedType.IUnknown)] object pUnk, uint dwReserved);

    /// <summary>
    /// Creates a data advise holder object.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int CreateDataAdviseHolder(out IDataAdviseHolder ppDAHolder);

    /// <summary>
    /// Creates an OLE advise holder object.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int CreateOleAdviseHolder([MarshalAs(UnmanagedType.Interface)] out IOleAdviseHolder ppOAHolder);

    /// <summary>
    /// Retrieves a user-readable type name from the registry.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int OleRegGetUserType([In] ref Guid clsid, uint dwFormOfType, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszUserType);

    /// <summary>
    /// Writes a class identifier to a storage object.
    /// </summary>
    [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
    public static extern void WriteClassStg(IStorage pStg, [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid);

    /// <summary>
    /// Retrieves the running object table.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

    /// <summary>
    /// Creates a bind context.
    /// </summary>
    [DllImport("Ole32.dll", EntryPoint = "CreateBindCtx", CharSet = CharSet.Auto)]
    public static extern int CreateBindCtx(uint reserved, out IBindCtx bc);

    /// <summary>
    /// Retrieves the current system time as a COM file time.
    /// </summary>
    [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
    public static extern void CoFileTimeNow(out System.Runtime.InteropServices.ComTypes.FILETIME time);

    /// <summary>
    /// Creates a file moniker.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int CreateFileMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszPathName, out IMoniker ppmk);

    /// <summary>
    /// Creates an item moniker.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int CreateItemMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszDelim, [MarshalAs(UnmanagedType.LPWStr)] string lpszItem, out System.Runtime.InteropServices.ComTypes.IMoniker ppmk);

    /// <summary>
    /// Creates a composite moniker from two monikers.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int CreateGenericComposite(IMoniker pmkFirst, IMoniker pmkRest, out IMoniker ppmkComposite);

    /// <summary>
    /// Saves an object to a stream.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int OleSaveToStream(IPersistStream pPStm, IStream pStm);

    /// <summary>
    /// Determines whether the specified data object is on the clipboard.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int OleIsCurrentClipboard([In] IDataObject pDataObject);

    /// <summary>
    /// Creates a stream object on a global memory handle.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out IStream ppstm);

    /// <summary>
    /// Opens an existing structured storage object.
    /// </summary>
    [DllImport("ole32.dll")]
    public static extern int StgOpenStorage([MarshalAs(UnmanagedType.LPWStr)] string pwcsName, IStorage? pstgPriority, STGM grfmode, IntPtr snbExclude, uint researved, out IStorage ppstgOpen);
  }
}
