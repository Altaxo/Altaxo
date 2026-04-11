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
    /// <param name="pvReserved">Reserved. Must be <see cref="IntPtr.Zero"/>.</param>
    /// <param name="dwCoInit">The concurrency model and initialization options.</param>
    /// <returns>The HRESULT returned by COM.</returns>
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
    /// <param name="rclsid">The CLSID of the class object to register.</param>
    /// <param name="pUnk">The class factory object to register.</param>
    /// <param name="dwClsContext">The execution context in which the class object runs.</param>
    /// <param name="flags">The registration flags.</param>
    /// <param name="lpdwRegister">When this method returns, contains the registration token.</param>
    /// <returns>The HRESULT returned by COM.</returns>
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
    /// <returns>The HRESULT returned by COM.</returns>
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
    /// <param name="ppDAHolder">When this method returns, contains the created data advise holder.</param>
    /// <returns>The HRESULT returned by COM.</returns>
    [DllImport("ole32.dll")]
    public static extern int CreateDataAdviseHolder(out IDataAdviseHolder ppDAHolder);

    /// <summary>
    /// Creates an OLE advise holder object.
    /// </summary>
    /// <param name="ppOAHolder">When this method returns, contains the created OLE advise holder.</param>
    /// <returns>The HRESULT returned by COM.</returns>
    [DllImport("ole32.dll")]
    public static extern int CreateOleAdviseHolder([MarshalAs(UnmanagedType.Interface)] out IOleAdviseHolder ppOAHolder);

    /// <summary>
    /// Retrieves a user-readable type name from the registry.
    /// </summary>
    /// <param name="clsid">The CLSID whose user type is requested.</param>
    /// <param name="dwFormOfType">The form of the user type string to retrieve.</param>
    /// <param name="pszUserType">A buffer that receives the user-readable type name.</param>
    /// <returns>The HRESULT returned by COM.</returns>
    [DllImport("ole32.dll")]
    public static extern int OleRegGetUserType([In] ref Guid clsid, uint dwFormOfType, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszUserType);

    /// <summary>
    /// Writes a class identifier to a storage object.
    /// </summary>
    /// <param name="pStg">The storage object that receives the class identifier.</param>
    /// <param name="rclsid">The class identifier to write.</param>
    [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
    public static extern void WriteClassStg(IStorage pStg, [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid);

    /// <summary>
    /// Retrieves the running object table.
    /// </summary>
    /// <param name="reserved">Reserved. Must be zero.</param>
    /// <param name="pprot">When this method returns, contains the running object table.</param>
    /// <returns>The HRESULT returned by COM.</returns>
    [DllImport("ole32.dll")]
    public static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

    /// <summary>
    /// Creates a bind context.
    /// </summary>
    /// <param name="reserved">Reserved. Must be zero.</param>
    /// <param name="bc">When this method returns, contains the created bind context.</param>
    /// <returns>The HRESULT returned by COM.</returns>
    [DllImport("Ole32.dll", EntryPoint = "CreateBindCtx", CharSet = CharSet.Auto)]
    public static extern int CreateBindCtx(uint reserved, out IBindCtx bc);

    /// <summary>
    /// Retrieves the current system time as a COM file time.
    /// </summary>
    /// <param name="time">When this method returns, contains the current system time.</param>
    [DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
    public static extern void CoFileTimeNow(out System.Runtime.InteropServices.ComTypes.FILETIME time);

    /// <summary>
    /// Creates a file moniker.
    /// </summary>
    /// <param name="lpszPathName">The path represented by the file moniker.</param>
    /// <param name="ppmk">When this method returns, contains the created moniker.</param>
    /// <returns>The HRESULT returned by COM.</returns>
    [DllImport("ole32.dll")]
    public static extern int CreateFileMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszPathName, out IMoniker ppmk);

    /// <summary>
    /// Creates an item moniker.
    /// </summary>
    /// <param name="lpszDelim">The delimiter that separates the item names.</param>
    /// <param name="lpszItem">The item name to represent.</param>
    /// <param name="ppmk">When this method returns, contains the created item moniker.</param>
    /// <returns>The HRESULT returned by COM.</returns>
    [DllImport("ole32.dll")]
    public static extern int CreateItemMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszDelim, [MarshalAs(UnmanagedType.LPWStr)] string lpszItem, out System.Runtime.InteropServices.ComTypes.IMoniker ppmk);

    /// <summary>
    /// Creates a composite moniker from two monikers.
    /// </summary>
    /// <param name="pmkFirst">The first moniker in the composite.</param>
    /// <param name="pmkRest">The remaining moniker to compose with <paramref name="pmkFirst"/>.</param>
    /// <param name="ppmkComposite">When this method returns, contains the composite moniker.</param>
    /// <returns>The HRESULT returned by COM.</returns>
    [DllImport("ole32.dll")]
    public static extern int CreateGenericComposite(IMoniker pmkFirst, IMoniker pmkRest, out IMoniker ppmkComposite);

    /// <summary>
    /// Saves an object to a stream.
    /// </summary>
    /// <param name="pPStm">The object that implements <see cref="IPersistStream"/>.</param>
    /// <param name="pStm">The destination stream.</param>
    /// <returns>The HRESULT returned by OLE.</returns>
    [DllImport("ole32.dll")]
    public static extern int OleSaveToStream(IPersistStream pPStm, IStream pStm);

    /// <summary>
    /// Determines whether the specified data object is on the clipboard.
    /// </summary>
    /// <param name="pDataObject">The data object to test.</param>
    /// <returns>The HRESULT returned by OLE.</returns>
    [DllImport("ole32.dll")]
    public static extern int OleIsCurrentClipboard([In] IDataObject pDataObject);

    /// <summary>
    /// Creates a stream object on a global memory handle.
    /// </summary>
    /// <param name="hGlobal">A handle to the global memory block.</param>
    /// <param name="fDeleteOnRelease"><see langword="true"/> to free the memory when the stream is released.</param>
    /// <param name="ppstm">When this method returns, contains the created stream.</param>
    /// <returns>The HRESULT returned by OLE.</returns>
    [DllImport("ole32.dll")]
    public static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out IStream ppstm);

    /// <summary>
    /// Opens an existing structured storage object.
    /// </summary>
    /// <param name="pwcsName">The path of the storage object to open.</param>
    /// <param name="pstgPriority">A priority storage object, or <see langword="null"/>.</param>
    /// <param name="grfmode">The access mode to use when opening the storage.</param>
    /// <param name="snbExclude">A block of names to exclude, or <see cref="IntPtr.Zero"/>.</param>
    /// <param name="researved">Reserved. Must be zero.</param>
    /// <param name="ppstgOpen">When this method returns, contains the opened storage object.</param>
    /// <returns>The HRESULT returned by OLE.</returns>
    [DllImport("ole32.dll")]
    public static extern int StgOpenStorage([MarshalAs(UnmanagedType.LPWStr)] string pwcsName, IStorage? pstgPriority, STGM grfmode, IntPtr snbExclude, uint researved, out IStorage ppstgOpen);
  }
}
