using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.UnmanagedApi.Ole32
{
	// Interface IClassFactory is here to provide a C# definition of the
	// COM IClassFactory interface.
	[
		ComImport, // This interface originated from COM.
		ComVisible(false), // It is not hard to imagine that this interface must not be exposed to COM.
		InterfaceType(ComInterfaceType.InterfaceIsIUnknown), // Indicate that this interface is not IDispatch-based.
		Guid("00000001-0000-0000-C000-000000000046")  // This GUID is the actual GUID of IClassFactory.
	]
	public interface IClassFactory
	{
		void CreateInstance(IntPtr pUnkOuter, ref Guid riid, out IntPtr ppvObject);

		void LockServer(bool fLock);
	}

	[ComImport, Guid("00000110-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDataAdviseHolder
	{
		void Advise(IDataObject dataObject, [In] ref FORMATETC fetc, ADVF advf, IAdviseSink advise, out int pdwConnection);

		void Unadvise(int dwConnection);

		IEnumSTATDATA EnumAdvise();

		void SendOnDataChange(IDataObject dataObject, int dwReserved, ADVF advf);
	};

	[ComImport, Guid("00000104-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IEnumOLEVERB
	{
		[PreserveSig]
		int Next([MarshalAs(UnmanagedType.U4)] int celt, [Out] tagOLEVERB rgelt, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);

		[PreserveSig]
		int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);

		void Reset();

		void Clone(out IEnumOLEVERB ppenum);
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000100-0000-0000-C000-000000000046")]
	public interface IEnumUnknown
	{
		[PreserveSig]
		int Next([In, MarshalAs(UnmanagedType.U4)] int celt, [Out] IntPtr rgelt, IntPtr pceltFetched);

		[PreserveSig]
		int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);

		void Reset();

		void Clone(out IEnumUnknown ppenum);
	}

	[ComImport, Guid("00000111-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IOleAdviseHolder
	{
		void Advise(IAdviseSink pAdvise, out int pdwConnection);

		void Unadvise(int dwConnection);

		[return: MarshalAs(UnmanagedType.Interface)]
		IEnumSTATDATA EnumAdvise();

		void SendOnRename(IMoniker pmk);

		void SendOnSave();

		void SendOnClose();
	};

	[ComImport]
	[Guid("00000118-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IOleClientSite
	{
		void SaveObject();

		void GetMoniker(uint dwAssign, uint dwWhichMoniker, ref object ppmk);

		void GetContainer(ref object ppContainer);

		void ShowObject();

		void OnShowWindow(bool fShow);

		void RequestNewObjectLayout();
	}

	// COM interop does not support interface inheritance so we must duplicate.
	[ComImport, Guid("0000011C-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IOleItemContainer
	{
		// IParseDisplayName.
		[PreserveSig]
		int ParseDisplayName([In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out int pchEaten, out IMoniker ppmkOut);

		// IOleContainer.
		[PreserveSig]
		int EnumObjects([In, MarshalAs(UnmanagedType.U4)] int grfFlags, out IEnumUnknown ppenum);

		[PreserveSig]
		int LockContainer(bool fLock);

		// IOleItemContainer.
		IntPtr GetObject([MarshalAs(UnmanagedType.LPWStr)] string pszItem,
										 int dwSpeedNeeded,
										 [In, MarshalAs(UnmanagedType.Interface)] IBindCtx pbc,
										 [In] ref Guid riid);

		[return: MarshalAs(UnmanagedType.Interface)]
		object GetObjectStorage([MarshalAs(UnmanagedType.LPWStr)] string pszItem,
														IBindCtx pbc,
														[In] ref Guid riid);

		[PreserveSig]
		int IsRunning([MarshalAs(UnmanagedType.LPWStr)] string pszItem);
	};

	//***OCB Why is SuppressUnmanagedCodeSecurity here?  Which of our methods
	// need it?
	//[System.Security.SuppressUnmanagedCodeSecurity]
	[ComImport, Guid("00000112-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IOleObject
	{
		[PreserveSig]
		int SetClientSite([In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pClientSite);

		IOleClientSite GetClientSite();

		[PreserveSig]
		int SetHostNames([In, MarshalAs(UnmanagedType.LPWStr)] string szContainerApp, [In, MarshalAs(UnmanagedType.LPWStr)] string szContainerObj);

		[PreserveSig]
		int Close([MarshalAs(UnmanagedType.I4)]tagOLECLOSE dwSaveOption);

		[PreserveSig]
		int SetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [In, MarshalAs(UnmanagedType.Interface)] object pmk);

		[PreserveSig]
		int GetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwAssign, [In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [MarshalAs(UnmanagedType.Interface)] out object moniker);

		[PreserveSig]
		int InitFromData([In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObject, int fCreation, [In, MarshalAs(UnmanagedType.U4)] int dwReserved);

		[PreserveSig]
		int GetClipboardData([In, MarshalAs(UnmanagedType.U4)] int dwReserved, out IDataObject data);

		[PreserveSig]
		int DoVerb(int iVerb, [In] IntPtr lpmsg, [In, MarshalAs(UnmanagedType.Interface)] IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, [In] COMRECT lprcPosRect);

		[PreserveSig]
		int EnumVerbs(out IEnumOLEVERB e);

		[PreserveSig]
		int OleUpdate();

		[PreserveSig]
		int IsUpToDate();

		[PreserveSig]
		int GetUserClassID([In, Out] ref Guid pClsid);

		[PreserveSig]
		int GetUserType([In, MarshalAs(UnmanagedType.U4)] int dwFormOfType, [MarshalAs(UnmanagedType.LPWStr)] out string userType);

		[PreserveSig]
		int SetExtent([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, [In] tagSIZEL pSizel);

		[PreserveSig]
		int GetExtent([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, [Out]tagSIZEL pSizel);

		[PreserveSig]
		int Advise(IAdviseSink pAdvSink, out int cookie);

		[PreserveSig]
		int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);

		[PreserveSig]
		int EnumAdvise(out IEnumSTATDATA e);

		[PreserveSig]
		int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] int dwAspect, out int misc);

		[PreserveSig]
		int SetColorScheme([In] tagLOGPALETTE pLogpal);
	}

	[ComImport, Guid("00000109-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IPersistStream
	{
		void GetClassID(out Guid pClassId);

		[PreserveSig]
		int IsDirty();

		void Load([In, MarshalAs(UnmanagedType.Interface)] IStream pstm);

		void Save([In, MarshalAs(UnmanagedType.Interface)] IStream pstm, [In, MarshalAs(UnmanagedType.Bool)] bool fClearDirty);

		long GetSizeMax();
	}

	[ComImport, Guid("0000010A-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IPersistStorage
	{
		void GetClassID(out Guid pClassID);

		[PreserveSig]
		int IsDirty();

		void InitNew(IStorage pstg);

		[PreserveSig]
		int Load(IStorage pstg);

		void Save(IStorage pStgSave, bool fSameAsLoad);

		void SaveCompleted(IStorage pStgNew);

		[PreserveSig]
		int HandsOffStorage();
	}

	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000B-0000-0000-C000-000000000046")]
	public interface IStorage
	{
		/// <summary>
		/// ÔÚµ±Ç°´æ´¢ÖÐ½¨Á¢ÐÂÁ÷£¬µÃµ½Á÷¶ÔÏó
		/// </summary>
		/// <param name="pwcsName"></param>
		/// <param name="grfMode"></param>
		/// <param name="reserved1"></param>
		/// <param name="reserved2"></param>
		/// <returns></returns>
		[return: MarshalAs(UnmanagedType.Interface)]
		IStream CreateStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

		/// <summary>
		/// ´ò¿ªÁ÷£¬µÃµ½Á÷¶ÔÏó
		/// </summary>
		/// <param name="pwcsName"></param>
		/// <param name="reserved1"></param>
		/// <param name="grfMode"></param>
		/// <param name="reserved2"></param>
		/// <returns></returns>
		[return: MarshalAs(UnmanagedType.Interface)]
		IStream OpenStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr reserved1, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

		/// <summary>
		/// ÔÚµ±Ç°´æ´¢ÖÐ½¨Á¢ÐÂ´æ´¢£¬µÃµ½×Ó´æ´¢¶ÔÏó
		/// </summary>
		/// <param name="pwcsName"></param>
		/// <param name="grfMode"></param>
		/// <param name="reserved1"></param>
		/// <param name="reserved2"></param>
		/// <returns></returns>
		[return: MarshalAs(UnmanagedType.Interface)]
		IStorage CreateStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

		/// <summary>
		/// ´ò¿ª×Ó´æ´¢£¬µÃµ½×Ó´æ´¢¶ÔÏó
		/// </summary>
		/// <param name="pwcsName"></param>
		/// <param name="pstgPriority"></param>
		/// <param name="grfMode"></param>
		/// <param name="snbExclude"></param>
		/// <param name="reserved"></param>
		/// <returns></returns>
		[return: MarshalAs(UnmanagedType.Interface)]
		IStorage OpenStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr pstgPriority, [In, MarshalAs(UnmanagedType.U4)] int grfMode, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.U4)] int reserved);

		void CopyTo(int ciidExclude, [In, MarshalAs(UnmanagedType.LPArray)] Guid[] pIIDExclude, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest);

		void MoveElementTo([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName, [In, MarshalAs(UnmanagedType.U4)] int grfFlags);

		void Commit(int grfCommitFlags);

		void Revert();

		void EnumElements([In, MarshalAs(UnmanagedType.U4)] int reserved1, IntPtr reserved2, [In, MarshalAs(UnmanagedType.U4)] int reserved3, [MarshalAs(UnmanagedType.Interface)] out object ppVal);

		void DestroyElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsName);

		void RenameElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsOldName, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName);

		void SetElementTimes([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In] System.Runtime.InteropServices.ComTypes.FILETIME pctime, [In] System.Runtime.InteropServices.ComTypes.FILETIME patime, [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

		/// <summary>
		/// ÔÚµ±Ç°´æ´¢ÖÐ½¨Á¢Ò»¸öÌØÊâµÄÁ÷¶ÔÏó£¬ÓÃÀ´±£´æCLSID
		/// </summary>
		/// <param name="clsid">CLSID</param>
		void SetClass([In] ref Guid clsid);

		void SetStateBits(int grfStateBits, int grfMask);

		/// <summary>
		/// È¡µÃµ±Ç°´æ´¢ÖÐµÄÏµÍ³ÐÅÏ¢
		/// </summary>
		/// <param name="pStatStg">·µ»ØµÄÏµÍ³ÐÅÏ¢</param>
		/// <param name="grfStatFlag"></param>
		void Stat([Out] System.Runtime.InteropServices.ComTypes.STATSTG pStatStg, int grfStatFlag);
	}
}