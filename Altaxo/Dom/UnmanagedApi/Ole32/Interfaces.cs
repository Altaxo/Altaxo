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
  /// Provides the COM <c>IClassFactory</c> interface.
  /// </summary>
  [
    ComImport, // This interface originated from COM.
    ComVisible(false), // It is not hard to imagine that this interface must not be exposed to COM.
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown), // Indicate that this interface is not IDispatch-based.
    Guid("00000001-0000-0000-C000-000000000046")  // This GUID is the actual GUID of IClassFactory.
  ]
  public interface IClassFactory
  {
    /// <summary>
    /// Creates an uninitialized object.
    /// </summary>
    /// <param name="pUnkOuter">A pointer to the controlling unknown for aggregation, or <see cref="IntPtr.Zero"/>.</param>
    /// <param name="riid">The identifier of the requested interface.</param>
    /// <param name="ppvObject">On return, the created object interface pointer.</param>
    void CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject);

    /// <summary>
    /// Locks or unlocks the server in memory.
    /// </summary>
    /// <param name="fLock"><see langword="true"/> to lock the server; otherwise, <see langword="false"/>.</param>
    void LockServer(bool fLock);
  }

  /// <summary>
  /// Provides advisory connections for data transfer objects.
  /// </summary>
  [ComImport, Guid("00000110-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IDataAdviseHolder
  {
    /// <summary>
    /// Creates an advisory connection.
    /// </summary>
    /// <param name="dataObject">The data object to monitor for changes.</param>
    /// <param name="fetc">The requested data format and target device information.</param>
    /// <param name="advf">The advisory flags that control the connection.</param>
    /// <param name="advise">The sink that receives change notifications.</param>
    /// <param name="pdwConnection">When this method returns, contains the advisory connection identifier.</param>
    void Advise(IDataObject dataObject, [In] ref FORMATETC fetc, ADVF advf, IAdviseSink advise, out int pdwConnection);

    /// <summary>
    /// Terminates an advisory connection.
    /// </summary>
    /// <param name="dwConnection">The advisory connection identifier to terminate.</param>
    void Unadvise(int dwConnection);

    /// <summary>
    /// Enumerates the current advisory connections.
    /// </summary>
    /// <returns>An enumerator for the current advisory connections.</returns>
    IEnumSTATDATA EnumAdvise();

    /// <summary>
    /// Sends a data-change notification to all advisory sinks.
    /// </summary>
    /// <param name="dataObject">The data object whose data changed.</param>
    /// <param name="dwReserved">Reserved. Must be zero.</param>
    /// <param name="advf">The advisory flags associated with the notification.</param>
    void SendOnDataChange(IDataObject dataObject, int dwReserved, ADVF advf);
  };

  /// <summary>
  /// Enumerates OLE verbs.
  /// </summary>
  [ComImport, Guid("00000104-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IEnumOLEVERB
  {
    /// <summary>
    /// Retrieves a specified number of verbs in the enumeration sequence.
    /// </summary>
    /// <param name="celt">The number of verbs to retrieve.</param>
    /// <param name="rgelt">Receives the retrieved verb description.</param>
    /// <param name="pceltFetched">Receives the number of verbs actually fetched.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int Next([MarshalAs(UnmanagedType.U4)] int celt, [Out] tagOLEVERB rgelt, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);

    /// <summary>
    /// Skips a specified number of verbs in the enumeration sequence.
    /// </summary>
    /// <param name="celt">The number of verbs to skip.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);

    /// <summary>
    /// Resets the enumeration sequence to the beginning.
    /// </summary>
    void Reset();

    /// <summary>
    /// Creates a copy of the enumerator in its current state.
    /// </summary>
    /// <param name="ppenum">When this method returns, contains the cloned enumerator.</param>
    void Clone(out IEnumOLEVERB ppenum);
  }

  /// <summary>
  /// Enumerates unknown COM interfaces.
  /// </summary>
  [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000100-0000-0000-C000-000000000046")]
  public interface IEnumUnknown
  {
    /// <summary>
    /// Retrieves a specified number of interfaces in the enumeration sequence.
    /// </summary>
    /// <param name="celt">The number of interface pointers to retrieve.</param>
    /// <param name="rgelt">A pointer that receives the fetched interface pointers.</param>
    /// <param name="pceltFetched">A pointer that receives the number of fetched interfaces.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int Next([In, MarshalAs(UnmanagedType.U4)] int celt, [Out] IntPtr rgelt, IntPtr pceltFetched);

    /// <summary>
    /// Skips a specified number of interfaces in the enumeration sequence.
    /// </summary>
    /// <param name="celt">The number of interfaces to skip.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);

    /// <summary>
    /// Resets the enumeration sequence to the beginning.
    /// </summary>
    void Reset();

    /// <summary>
    /// Creates a copy of the enumerator in its current state.
    /// </summary>
    /// <param name="ppenum">When this method returns, contains the cloned enumerator.</param>
    void Clone(out IEnumUnknown ppenum);
  }

  /// <summary>
  /// Manages advisory connections for OLE objects.
  /// </summary>
  [ComImport, Guid("00000111-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IOleAdviseHolder
  {
    /// <summary>
    /// Creates an advisory connection.
    /// </summary>
    /// <param name="pAdvise">The sink that receives OLE notifications.</param>
    /// <param name="pdwConnection">When this method returns, contains the advisory connection identifier.</param>
    void Advise(IAdviseSink pAdvise, out int pdwConnection);

    /// <summary>
    /// Terminates an advisory connection.
    /// </summary>
    /// <param name="dwConnection">The advisory connection identifier to terminate.</param>
    void Unadvise(int dwConnection);

    /// <summary>
    /// Enumerates the current advisory connections.
    /// </summary>
    /// <returns>An enumerator for the current advisory connections.</returns>
    [return: MarshalAs(UnmanagedType.Interface)]
    IEnumSTATDATA EnumAdvise();

    /// <summary>
    /// Sends a rename notification.
    /// </summary>
    /// <param name="pmk">The moniker that identifies the renamed object.</param>
    void SendOnRename(IMoniker pmk);

    /// <summary>
    /// Sends a save notification.
    /// </summary>
    void SendOnSave();

    /// <summary>
    /// Sends a close notification.
    /// </summary>
    void SendOnClose();
  };

  /// <summary>
  /// Provides callbacks to an OLE client site.
  /// </summary>
  [ComImport]
  [Guid("00000118-0000-0000-C000-000000000046")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IOleClientSite
  {
    /// <summary>
    /// Requests that the object save itself.
    /// </summary>
    void SaveObject();

    /// <summary>
    /// Retrieves a moniker assigned to the object.
    /// </summary>
    /// <param name="dwAssign">The assignment for the moniker.</param>
    /// <param name="dwWhichMoniker">The kind of moniker to retrieve.</param>
    /// <param name="ppmk">When this method returns, contains the requested moniker.</param>
    void GetMoniker(uint dwAssign, uint dwWhichMoniker, ref object ppmk);

    /// <summary>
    /// Retrieves the object's container.
    /// </summary>
    /// <param name="ppContainer">When this method returns, contains the container object.</param>
    void GetContainer(ref object ppContainer);

    /// <summary>
    /// Notifies the container that the object should be shown.
    /// </summary>
    void ShowObject();

    /// <summary>
    /// Notifies the object when its window is shown or hidden.
    /// </summary>
    /// <param name="fShow"><see langword="true"/> if the object's window is shown; otherwise, <see langword="false"/>.</param>
    void OnShowWindow(bool fShow);

    /// <summary>
    /// Requests a new object layout.
    /// </summary>
    void RequestNewObjectLayout();
  }

  /// <summary>
  /// Provides access to embedded item objects in a compound document.
  /// </summary>
  [ComImport, Guid("0000011C-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IOleItemContainer
  {
    /// <summary>
    /// Parses a display name into a moniker.
    /// </summary>
    /// <param name="pbc">The bind context used during parsing.</param>
    /// <param name="pszDisplayName">The display name to parse.</param>
    /// <param name="pchEaten">When this method returns, contains the number of parsed characters.</param>
    /// <param name="ppmkOut">When this method returns, contains the resulting moniker.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int ParseDisplayName([In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out int pchEaten, out IMoniker ppmkOut);

    /// <summary>
    /// Enumerates the contained objects.
    /// </summary>
    /// <param name="grfFlags">The enumeration flags.</param>
    /// <param name="ppenum">When this method returns, contains the object enumerator.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int EnumObjects([In, MarshalAs(UnmanagedType.U4)] int grfFlags, out IEnumUnknown ppenum);

    /// <summary>
    /// Locks or unlocks the container.
    /// </summary>
    /// <param name="fLock"><see langword="true"/> to lock the container; otherwise, <see langword="false"/>.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int LockContainer(bool fLock);

    /// <summary>
    /// Retrieves an object identified by an item name.
    /// </summary>
    /// <param name="pszItem">The item name that identifies the object.</param>
    /// <param name="dwSpeedNeeded">The requested retrieval speed hint.</param>
    /// <param name="pbc">The bind context used to retrieve the object.</param>
    /// <param name="riid">The interface identifier of the requested object.</param>
    /// <returns>A pointer to the requested interface.</returns>
    IntPtr GetObject([MarshalAs(UnmanagedType.LPWStr)] string pszItem,
                     int dwSpeedNeeded,
                     [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc,
                     [In] ref Guid riid);

    /// <summary>
    /// Retrieves a storage object identified by an item name.
    /// </summary>
    /// <param name="pszItem">The item name that identifies the storage object.</param>
    /// <param name="pbc">The bind context used to retrieve the storage object.</param>
    /// <param name="riid">The interface identifier of the requested storage interface.</param>
    /// <returns>The requested storage object.</returns>
    [return: MarshalAs(UnmanagedType.Interface)]
    object GetObjectStorage([MarshalAs(UnmanagedType.LPWStr)] string pszItem,
                            IBindCtx pbc,
                            [In] ref Guid riid);

    /// <summary>
    /// Determines whether the specified object is running.
    /// </summary>
    /// <param name="pszItem">The item name that identifies the object.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int IsRunning([MarshalAs(UnmanagedType.LPWStr)] string pszItem);
  };

  /// <summary>
  /// Provides operations for an OLE object.
  /// </summary>
  [ComImport, Guid("00000112-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IOleObject
  {
    /// <summary>
    /// Sets the client site of the object.
    /// </summary>
    /// <param name="pClientSite">The client site to associate with the object.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int SetClientSite([In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pClientSite);

    /// <summary>
    /// Retrieves the current client site.
    /// </summary>
    /// <returns>The current client site.</returns>
    IOleClientSite GetClientSite();

    /// <summary>
    /// Sets the host names of the container and object.
    /// </summary>
    /// <param name="szContainerApp">The name of the container application.</param>
    /// <param name="szContainerObj">The name of the container document or object.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int SetHostNames([In, MarshalAs(UnmanagedType.LPWStr)] string szContainerApp, [In, MarshalAs(UnmanagedType.LPWStr)] string szContainerObj);

    /// <summary>
    /// Closes the object.
    /// </summary>
    /// <param name="dwSaveOption">The save option to use while closing the object.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int Close([MarshalAs(UnmanagedType.I4)]tagOLECLOSE dwSaveOption);

    /// <summary>
    /// Sets a moniker for the object.
    /// </summary>
    /// <param name="dwWhichMoniker">The kind of moniker to set.</param>
    /// <param name="pmk">The moniker object to assign.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int SetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [In, MarshalAs(UnmanagedType.Interface)] object pmk);

    /// <summary>
    /// Retrieves a moniker for the object.
    /// </summary>
    /// <param name="dwAssign">The moniker assignment to use.</param>
    /// <param name="dwWhichMoniker">The kind of moniker to retrieve.</param>
    /// <param name="moniker">When this method returns, contains the retrieved moniker.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int GetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwAssign, [In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [MarshalAs(UnmanagedType.Interface)] out object moniker);

    /// <summary>
    /// Initializes the object from a data object.
    /// </summary>
    /// <param name="pDataObject">The data object used for initialization.</param>
    /// <param name="fCreation">Nonzero to indicate a new object should be created from the data.</param>
    /// <param name="dwReserved">Reserved. Must be zero.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int InitFromData([In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObject, int fCreation, [In, MarshalAs(UnmanagedType.U4)] int dwReserved);

    /// <summary>
    /// Retrieves data from the clipboard.
    /// </summary>
    /// <param name="dwReserved">Reserved. Must be zero.</param>
    /// <param name="data">When this method returns, contains the clipboard data object.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int GetClipboardData([In, MarshalAs(UnmanagedType.U4)] int dwReserved, out IDataObject data);

    /// <summary>
    /// Executes a verb on the object.
    /// </summary>
    /// <param name="iVerb">The verb identifier to execute.</param>
    /// <param name="lpmsg">A pointer to the message that triggered the verb, or <see cref="IntPtr.Zero"/>.</param>
    /// <param name="pActiveSite">The active client site for in-place activation.</param>
    /// <param name="lindex">Reserved. Must be zero.</param>
    /// <param name="hwndParent">A handle to the parent window.</param>
    /// <param name="lprcPosRect">The position rectangle for displaying the object.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int DoVerb(int iVerb, [In] IntPtr lpmsg, [In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, [In] COMRECT lprcPosRect);

    /// <summary>
    /// Enumerates the verbs supported by the object.
    /// </summary>
    /// <param name="e">When this method returns, contains the verb enumerator.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int EnumVerbs(out IEnumOLEVERB e);

    /// <summary>
    /// Updates the object.
    /// </summary>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int OleUpdate();

    /// <summary>
    /// Determines whether the object is up to date.
    /// </summary>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int IsUpToDate();

    /// <summary>
    /// Retrieves the class identifier of the object.
    /// </summary>
    /// <param name="pClsid">When this method returns, contains the class identifier.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int GetUserClassID([In, Out] ref Guid pClsid);

    /// <summary>
    /// Retrieves a user-readable type name.
    /// </summary>
    /// <param name="dwFormOfType">The form of the user type string to retrieve.</param>
    /// <param name="userType">When this method returns, contains the user-readable type name.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int GetUserType([In, MarshalAs(UnmanagedType.U4)] int dwFormOfType, [MarshalAs(UnmanagedType.LPWStr)] out string userType);

    /// <summary>
    /// Sets the display extent of the object.
    /// </summary>
    /// <param name="dwDrawAspect">The drawing aspect whose extent is being set.</param>
    /// <param name="pSizel">The extent to apply.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int SetExtent([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, [In] tagSIZEL pSizel);

    /// <summary>
    /// Retrieves the display extent of the object.
    /// </summary>
    /// <param name="dwDrawAspect">The drawing aspect whose extent is requested.</param>
    /// <param name="pSizel">Receives the current extent.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int GetExtent([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, [Out]tagSIZEL pSizel);

    /// <summary>
    /// Establishes an advisory connection.
    /// </summary>
    /// <param name="pAdvSink">The sink that receives notifications.</param>
    /// <param name="cookie">When this method returns, contains the connection identifier.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int Advise(IAdviseSink pAdvSink, out int cookie);

    /// <summary>
    /// Terminates an advisory connection.
    /// </summary>
    /// <param name="dwConnection">The advisory connection identifier to terminate.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);

    /// <summary>
    /// Enumerates the advisory connections.
    /// </summary>
    /// <param name="e">When this method returns, contains the advisory enumerator.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int EnumAdvise(out IEnumSTATDATA e);

    /// <summary>
    /// Retrieves miscellaneous status flags.
    /// </summary>
    /// <param name="dwAspect">The requested drawing aspect.</param>
    /// <param name="misc">When this method returns, contains the miscellaneous status flags.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] int dwAspect, out int misc);

    /// <summary>
    /// Sets the recommended color palette.
    /// </summary>
    /// <param name="pLogpal">The logical palette to associate with the object.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int SetColorScheme([In] tagLOGPALETTE pLogpal);
  }

  /// <summary>
  /// Provides persistence operations based on a stream.
  /// </summary>
  [ComImport, Guid("00000109-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IPersistStream
  {
    /// <summary>
    /// Retrieves the class identifier.
    /// </summary>
    /// <param name="pClassId">When this method returns, contains the class identifier.</param>
    void GetClassID(out Guid pClassId);

    /// <summary>
    /// Determines whether the object has changed since it was last saved.
    /// </summary>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int IsDirty();

    /// <summary>
    /// Loads the object from a stream.
    /// </summary>
    /// <param name="pstm">The source stream.</param>
    void Load([In, MarshalAs(UnmanagedType.Interface)] IStream pstm);

    /// <summary>
    /// Saves the object to a stream.
    /// </summary>
    /// <param name="pstm">The destination stream.</param>
    /// <param name="fClearDirty"><see langword="true"/> to clear the dirty flag after saving.</param>
    void Save([In, MarshalAs(UnmanagedType.Interface)] IStream pstm, [In, MarshalAs(UnmanagedType.Bool)] bool fClearDirty);

    /// <summary>
    /// Retrieves the maximum size of the stream needed to save the object.
    /// </summary>
    /// <returns>The maximum stream size required to save the object.</returns>
    long GetSizeMax();
  }

  /// <summary>
  /// Provides persistence operations based on structured storage.
  /// </summary>
  [ComImport, Guid("0000010A-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IPersistStorage
  {
    /// <summary>
    /// Retrieves the class identifier.
    /// </summary>
    /// <param name="pClassID">When this method returns, contains the class identifier.</param>
    void GetClassID(out Guid pClassID);

    /// <summary>
    /// Determines whether the object has changed since it was last saved.
    /// </summary>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int IsDirty();

    /// <summary>
    /// Initializes a new storage object.
    /// </summary>
    /// <param name="pstg">The storage object used for initialization.</param>
    void InitNew(IStorage pstg);

    /// <summary>
    /// Loads the object from storage.
    /// </summary>
    /// <param name="pstg">The source storage object.</param>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int Load(IStorage pstg);

    /// <summary>
    /// Saves the object to storage.
    /// </summary>
    /// <param name="pStgSave">The destination storage object.</param>
    /// <param name="fSameAsLoad"><see langword="true"/> if the destination storage is the same as the load storage.</param>
    void Save(IStorage pStgSave, bool fSameAsLoad);

    /// <summary>
    /// Notifies the object that a save operation has completed.
    /// </summary>
    /// <param name="pStgNew">The new storage object to use after saving.</param>
    void SaveCompleted(IStorage pStgNew);

    /// <summary>
    /// Tells the object to release all storage-related resources.
    /// </summary>
    /// <returns>The HRESULT returned by the COM method.</returns>
    [PreserveSig]
    int HandsOffStorage();
  }

  /// <summary>
  /// Provides access to structured storage objects.
  /// </summary>
  [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000B-0000-0000-C000-000000000046")]
  public interface IStorage
  {
    /// <summary>
    /// Creates a stream object.
    /// </summary>
    /// <param name="pwcsName">The name of the stream to create.</param>
    /// <param name="grfMode">The storage mode flags.</param>
    /// <param name="reserved1">Reserved. Must be zero.</param>
    /// <param name="reserved2">Reserved. Must be zero.</param>
    /// <returns>The created stream object.</returns>
    [return: MarshalAs(UnmanagedType.Interface)]
    IStream CreateStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

    /// <summary>
    /// Opens an existing stream object.
    /// </summary>
    /// <param name="pwcsName">The name of the stream to open.</param>
    /// <param name="reserved1">Reserved. Must be <see cref="IntPtr.Zero"/>.</param>
    /// <param name="grfMode">The storage mode flags.</param>
    /// <param name="reserved2">Reserved. Must be zero.</param>
    /// <returns>The opened stream object.</returns>
    [return: MarshalAs(UnmanagedType.Interface)]
    IStream OpenStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr reserved1, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

    /// <summary>
    /// Creates a storage object.
    /// </summary>
    /// <param name="pwcsName">The name of the storage to create.</param>
    /// <param name="grfMode">The storage mode flags.</param>
    /// <param name="reserved1">Reserved. Must be zero.</param>
    /// <param name="reserved2">Reserved. Must be zero.</param>
    /// <returns>The created storage object.</returns>
    [return: MarshalAs(UnmanagedType.Interface)]
    IStorage CreateStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

    /// <summary>
    /// Opens an existing storage object.
    /// </summary>
    /// <param name="pwcsName">The name of the storage to open.</param>
    /// <param name="pstgPriority">A priority storage object, or <see cref="IntPtr.Zero"/>.</param>
    /// <param name="grfMode">The storage mode flags.</param>
    /// <param name="snbExclude">A block of names to exclude, or <see cref="IntPtr.Zero"/>.</param>
    /// <param name="reserved">Reserved. Must be zero.</param>
    /// <returns>The opened storage object.</returns>
    [return: MarshalAs(UnmanagedType.Interface)]
    IStorage OpenStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr pstgPriority, [In, MarshalAs(UnmanagedType.U4)] int grfMode, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.U4)] int reserved);

    /// <summary>
    /// Copies storage elements to another storage object.
    /// </summary>
    /// <param name="ciidExclude">The number of interface identifiers to exclude.</param>
    /// <param name="pIIDExclude">The interface identifiers to exclude from the copy operation.</param>
    /// <param name="snbExclude">A block of element names to exclude.</param>
    /// <param name="stgDest">The destination storage object.</param>
    void CopyTo(int ciidExclude, [In, MarshalAs(UnmanagedType.LPArray)] Guid[] pIIDExclude, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest);

    /// <summary>
    /// Moves an element to another storage object.
    /// </summary>
    /// <param name="pwcsName">The name of the element to move.</param>
    /// <param name="stgDest">The destination storage object.</param>
    /// <param name="pwcsNewName">The new name of the moved element.</param>
    /// <param name="grfFlags">The move flags.</param>
    void MoveElementTo([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName, [In, MarshalAs(UnmanagedType.U4)] int grfFlags);

    /// <summary>
    /// Commits changes to the storage object.
    /// </summary>
    /// <param name="grfCommitFlags">The commit flags.</param>
    void Commit(int grfCommitFlags);

    /// <summary>
    /// Discards changes since the last commit.
    /// </summary>
    void Revert();

    /// <summary>
    /// Enumerates the elements in the storage object.
    /// </summary>
    /// <param name="reserved1">Reserved. Must be zero.</param>
    /// <param name="reserved2">Reserved. Must be <see cref="IntPtr.Zero"/>.</param>
    /// <param name="reserved3">Reserved. Must be zero.</param>
    /// <param name="ppVal">When this method returns, contains the element enumerator.</param>
    void EnumElements([In, MarshalAs(UnmanagedType.U4)] int reserved1, IntPtr reserved2, [In, MarshalAs(UnmanagedType.U4)] int reserved3, [MarshalAs(UnmanagedType.Interface)] out object ppVal);

    /// <summary>
    /// Destroys a storage element.
    /// </summary>
    /// <param name="pwcsName">The name of the element to destroy.</param>
    void DestroyElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsName);

    /// <summary>
    /// Renames a storage element.
    /// </summary>
    /// <param name="pwcsOldName">The current name of the element.</param>
    /// <param name="pwcsNewName">The new name for the element.</param>
    void RenameElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsOldName, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName);

    /// <summary>
    /// Sets the timestamps of a storage element.
    /// </summary>
    /// <param name="pwcsName">The name of the element whose timestamps are set.</param>
    /// <param name="pctime">The new creation time.</param>
    /// <param name="patime">The new access time.</param>
    /// <param name="pmtime">The new modification time.</param>
    void SetElementTimes([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In] System.Runtime.InteropServices.ComTypes.FILETIME pctime, [In] System.Runtime.InteropServices.ComTypes.FILETIME patime, [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

    /// <summary>
    /// Sets the class identifier of the storage object.
    /// </summary>
    /// <param name="clsid">The class identifier to assign.</param>
    void SetClass([In] ref Guid clsid);

    /// <summary>
    /// Sets state bits for the storage object.
    /// </summary>
    /// <param name="grfStateBits">The state bits to set.</param>
    /// <param name="grfMask">The mask that specifies which bits are affected.</param>
    void SetStateBits(int grfStateBits, int grfMask);

    /// <summary>
    /// Retrieves status information about the storage object.
    /// </summary>
    /// <param name="pStatStg">Receives the storage status information.</param>
    /// <param name="grfStatFlag">The flags that control the returned information.</param>
    void Stat([Out] System.Runtime.InteropServices.ComTypes.STATSTG pStatStg, int grfStatFlag);
  }
}
