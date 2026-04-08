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
    void Advise(IDataObject dataObject, [In] ref FORMATETC fetc, ADVF advf, IAdviseSink advise, out int pdwConnection);

    /// <summary>
    /// Terminates an advisory connection.
    /// </summary>
    void Unadvise(int dwConnection);

    /// <summary>
    /// Enumerates the current advisory connections.
    /// </summary>
    IEnumSTATDATA EnumAdvise();

    /// <summary>
    /// Sends a data-change notification to all advisory sinks.
    /// </summary>
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
    [PreserveSig]
    int Next([MarshalAs(UnmanagedType.U4)] int celt, [Out] tagOLEVERB rgelt, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);

    /// <summary>
    /// Skips a specified number of verbs in the enumeration sequence.
    /// </summary>
    [PreserveSig]
    int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);

    /// <summary>
    /// Resets the enumeration sequence to the beginning.
    /// </summary>
    void Reset();

    /// <summary>
    /// Creates a copy of the enumerator in its current state.
    /// </summary>
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
    [PreserveSig]
    int Next([In, MarshalAs(UnmanagedType.U4)] int celt, [Out] IntPtr rgelt, IntPtr pceltFetched);

    /// <summary>
    /// Skips a specified number of interfaces in the enumeration sequence.
    /// </summary>
    [PreserveSig]
    int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);

    /// <summary>
    /// Resets the enumeration sequence to the beginning.
    /// </summary>
    void Reset();

    /// <summary>
    /// Creates a copy of the enumerator in its current state.
    /// </summary>
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
    void Advise(IAdviseSink pAdvise, out int pdwConnection);

    /// <summary>
    /// Terminates an advisory connection.
    /// </summary>
    void Unadvise(int dwConnection);

    /// <summary>
    /// Enumerates the current advisory connections.
    /// </summary>
    [return: MarshalAs(UnmanagedType.Interface)]
    IEnumSTATDATA EnumAdvise();

    /// <summary>
    /// Sends a rename notification.
    /// </summary>
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
    void GetMoniker(uint dwAssign, uint dwWhichMoniker, ref object ppmk);

    /// <summary>
    /// Retrieves the object's container.
    /// </summary>
    void GetContainer(ref object ppContainer);

    /// <summary>
    /// Notifies the container that the object should be shown.
    /// </summary>
    void ShowObject();

    /// <summary>
    /// Notifies the object when its window is shown or hidden.
    /// </summary>
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
    [PreserveSig]
    int ParseDisplayName([In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out int pchEaten, out IMoniker ppmkOut);

    /// <summary>
    /// Enumerates the contained objects.
    /// </summary>
    [PreserveSig]
    int EnumObjects([In, MarshalAs(UnmanagedType.U4)] int grfFlags, out IEnumUnknown ppenum);

    /// <summary>
    /// Locks or unlocks the container.
    /// </summary>
    [PreserveSig]
    int LockContainer(bool fLock);

    /// <summary>
    /// Retrieves an object identified by an item name.
    /// </summary>
    IntPtr GetObject([MarshalAs(UnmanagedType.LPWStr)] string pszItem,
                     int dwSpeedNeeded,
                     [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc,
                     [In] ref Guid riid);

    /// <summary>
    /// Retrieves a storage object identified by an item name.
    /// </summary>
    [return: MarshalAs(UnmanagedType.Interface)]
    object GetObjectStorage([MarshalAs(UnmanagedType.LPWStr)] string pszItem,
                            IBindCtx pbc,
                            [In] ref Guid riid);

    /// <summary>
    /// Determines whether the specified object is running.
    /// </summary>
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
    [PreserveSig]
    int SetClientSite([In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pClientSite);

    /// <summary>
    /// Retrieves the current client site.
    /// </summary>
    IOleClientSite GetClientSite();

    /// <summary>
    /// Sets the host names of the container and object.
    /// </summary>
    [PreserveSig]
    int SetHostNames([In, MarshalAs(UnmanagedType.LPWStr)] string szContainerApp, [In, MarshalAs(UnmanagedType.LPWStr)] string szContainerObj);

    /// <summary>
    /// Closes the object.
    /// </summary>
    [PreserveSig]
    int Close([MarshalAs(UnmanagedType.I4)]tagOLECLOSE dwSaveOption);

    /// <summary>
    /// Sets a moniker for the object.
    /// </summary>
    [PreserveSig]
    int SetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [In, MarshalAs(UnmanagedType.Interface)] object pmk);

    /// <summary>
    /// Retrieves a moniker for the object.
    /// </summary>
    [PreserveSig]
    int GetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwAssign, [In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [MarshalAs(UnmanagedType.Interface)] out object moniker);

    /// <summary>
    /// Initializes the object from a data object.
    /// </summary>
    [PreserveSig]
    int InitFromData([In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObject, int fCreation, [In, MarshalAs(UnmanagedType.U4)] int dwReserved);

    /// <summary>
    /// Retrieves data from the clipboard.
    /// </summary>
    [PreserveSig]
    int GetClipboardData([In, MarshalAs(UnmanagedType.U4)] int dwReserved, out IDataObject data);

    /// <summary>
    /// Executes a verb on the object.
    /// </summary>
    [PreserveSig]
    int DoVerb(int iVerb, [In] IntPtr lpmsg, [In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, [In] COMRECT lprcPosRect);

    /// <summary>
    /// Enumerates the verbs supported by the object.
    /// </summary>
    [PreserveSig]
    int EnumVerbs(out IEnumOLEVERB e);

    /// <summary>
    /// Updates the object.
    /// </summary>
    [PreserveSig]
    int OleUpdate();

    /// <summary>
    /// Determines whether the object is up to date.
    /// </summary>
    [PreserveSig]
    int IsUpToDate();

    /// <summary>
    /// Retrieves the class identifier of the object.
    /// </summary>
    [PreserveSig]
    int GetUserClassID([In, Out] ref Guid pClsid);

    /// <summary>
    /// Retrieves a user-readable type name.
    /// </summary>
    [PreserveSig]
    int GetUserType([In, MarshalAs(UnmanagedType.U4)] int dwFormOfType, [MarshalAs(UnmanagedType.LPWStr)] out string userType);

    /// <summary>
    /// Sets the display extent of the object.
    /// </summary>
    [PreserveSig]
    int SetExtent([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, [In] tagSIZEL pSizel);

    /// <summary>
    /// Retrieves the display extent of the object.
    /// </summary>
    [PreserveSig]
    int GetExtent([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, [Out]tagSIZEL pSizel);

    /// <summary>
    /// Establishes an advisory connection.
    /// </summary>
    [PreserveSig]
    int Advise(IAdviseSink pAdvSink, out int cookie);

    /// <summary>
    /// Terminates an advisory connection.
    /// </summary>
    [PreserveSig]
    int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);

    /// <summary>
    /// Enumerates the advisory connections.
    /// </summary>
    [PreserveSig]
    int EnumAdvise(out IEnumSTATDATA e);

    /// <summary>
    /// Retrieves miscellaneous status flags.
    /// </summary>
    [PreserveSig]
    int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] int dwAspect, out int misc);

    /// <summary>
    /// Sets the recommended color palette.
    /// </summary>
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
    void GetClassID(out Guid pClassId);

    /// <summary>
    /// Determines whether the object has changed since it was last saved.
    /// </summary>
    [PreserveSig]
    int IsDirty();

    /// <summary>
    /// Loads the object from a stream.
    /// </summary>
    void Load([In, MarshalAs(UnmanagedType.Interface)] IStream pstm);

    /// <summary>
    /// Saves the object to a stream.
    /// </summary>
    void Save([In, MarshalAs(UnmanagedType.Interface)] IStream pstm, [In, MarshalAs(UnmanagedType.Bool)] bool fClearDirty);

    /// <summary>
    /// Retrieves the maximum size of the stream needed to save the object.
    /// </summary>
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
    void GetClassID(out Guid pClassID);

    /// <summary>
    /// Determines whether the object has changed since it was last saved.
    /// </summary>
    [PreserveSig]
    int IsDirty();

    /// <summary>
    /// Initializes a new storage object.
    /// </summary>
    void InitNew(IStorage pstg);

    /// <summary>
    /// Loads the object from storage.
    /// </summary>
    [PreserveSig]
    int Load(IStorage pstg);

    /// <summary>
    /// Saves the object to storage.
    /// </summary>
    void Save(IStorage pStgSave, bool fSameAsLoad);

    /// <summary>
    /// Notifies the object that a save operation has completed.
    /// </summary>
    void SaveCompleted(IStorage pStgNew);

    /// <summary>
    /// Tells the object to release all storage-related resources.
    /// </summary>
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
    [return: MarshalAs(UnmanagedType.Interface)]
    IStream CreateStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

    /// <summary>
    /// Opens an existing stream object.
    /// </summary>
    [return: MarshalAs(UnmanagedType.Interface)]
    IStream OpenStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr reserved1, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

    /// <summary>
    /// Creates a storage object.
    /// </summary>
    [return: MarshalAs(UnmanagedType.Interface)]
    IStorage CreateStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

    /// <summary>
    /// Opens an existing storage object.
    /// </summary>
    [return: MarshalAs(UnmanagedType.Interface)]
    IStorage OpenStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr pstgPriority, [In, MarshalAs(UnmanagedType.U4)] int grfMode, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.U4)] int reserved);

    /// <summary>
    /// Copies storage elements to another storage object.
    /// </summary>
    void CopyTo(int ciidExclude, [In, MarshalAs(UnmanagedType.LPArray)] Guid[] pIIDExclude, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest);

    /// <summary>
    /// Moves an element to another storage object.
    /// </summary>
    void MoveElementTo([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName, [In, MarshalAs(UnmanagedType.U4)] int grfFlags);

    /// <summary>
    /// Commits changes to the storage object.
    /// </summary>
    void Commit(int grfCommitFlags);

    /// <summary>
    /// Discards changes since the last commit.
    /// </summary>
    void Revert();

    /// <summary>
    /// Enumerates the elements in the storage object.
    /// </summary>
    void EnumElements([In, MarshalAs(UnmanagedType.U4)] int reserved1, IntPtr reserved2, [In, MarshalAs(UnmanagedType.U4)] int reserved3, [MarshalAs(UnmanagedType.Interface)] out object ppVal);

    /// <summary>
    /// Destroys a storage element.
    /// </summary>
    void DestroyElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsName);

    /// <summary>
    /// Renames a storage element.
    /// </summary>
    void RenameElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsOldName, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName);

    /// <summary>
    /// Sets the timestamps of a storage element.
    /// </summary>
    void SetElementTimes([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In] System.Runtime.InteropServices.ComTypes.FILETIME pctime, [In] System.Runtime.InteropServices.ComTypes.FILETIME patime, [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

    /// <summary>
    /// Sets the class identifier of the storage object.
    /// </summary>
    void SetClass([In] ref Guid clsid);

    /// <summary>
    /// Sets state bits for the storage object.
    /// </summary>
    void SetStateBits(int grfStateBits, int grfMask);

    /// <summary>
    /// Retrieves status information about the storage object.
    /// </summary>
    void Stat([Out] System.Runtime.InteropServices.ComTypes.STATSTG pStatStg, int grfStatFlag);
  }
}
