using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.UnmanagedApi.Ole32
{
	public static class Ole32Func
	{
		// CoInitializeEx() can be used to set the apartment model
		// of individual threads.
		[DllImport("ole32.dll")]
		public static extern int CoInitializeEx(IntPtr pvReserved, uint dwCoInit);

		// CoUninitialize() is used to uninitialize a COM thread.
		[DllImport("ole32.dll")]
		public static extern void CoUninitialize();

		// CoRegisterClassObject() is used to register a Class Factory
		// into COM's internal table of Class Factories.
		[DllImport("ole32.dll")]
		public static extern int CoRegisterClassObject([In] ref Guid rclsid,
			[MarshalAs(UnmanagedType.IUnknown)] object pUnk, uint dwClsContext,
			uint flags, out uint lpdwRegister);

		// Called by an COM EXE Server that can register multiple class objects
		// to inform COM about all registered classes, and permits activation
		// requests for those class objects.
		// This function causes OLE to inform the SCM about all the registered
		// classes, and begins letting activation requests into the server process.
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

		[DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void CoDisconnectObject([MarshalAs(UnmanagedType.IUnknown)] object pUnk, uint dwReserved);

		[DllImport("ole32.dll")]
		public static extern int CreateDataAdviseHolder(out IDataAdviseHolder ppDAHolder);

		[DllImport("ole32.dll")]
		public static extern int CreateOleAdviseHolder([MarshalAs(UnmanagedType.Interface)] out IOleAdviseHolder ppOAHolder);

		[DllImport("ole32.dll")]
		public static extern int OleRegGetUserType([In] ref Guid clsid, uint dwFormOfType, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszUserType);

		[DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void WriteClassStg(IStorage pStg, [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid);

		[DllImport("ole32.dll")]
		public static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

		[DllImport("Ole32.dll", EntryPoint = "CreateBindCtx", CharSet = CharSet.Auto)]
		public static extern int CreateBindCtx(uint reserved, out IBindCtx bc);

		[DllImport("ole32.dll", ExactSpelling = true, PreserveSig = false)]
		public static extern void CoFileTimeNow(out System.Runtime.InteropServices.ComTypes.FILETIME time);

		[DllImport("ole32.dll")]
		public static extern int CreateFileMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszPathName, out IMoniker ppmk);

		[DllImport("ole32.dll")]
		public static extern int CreateItemMoniker([MarshalAs(UnmanagedType.LPWStr)] string lpszDelim, [MarshalAs(UnmanagedType.LPWStr)] string lpszItem, out System.Runtime.InteropServices.ComTypes.IMoniker ppmk);

		[DllImport("ole32.dll")]
		public static extern int CreateGenericComposite(IMoniker pmkFirst, IMoniker pmkRest, out IMoniker ppmkComposite);

		[DllImport("ole32.dll")]
		public static extern int OleSaveToStream(IPersistStream pPStm, IStream pStm);

		[DllImport("ole32.dll")]
		public static extern int OleIsCurrentClipboard([In]IDataObject pDataObject);

		[DllImport("ole32.dll")]
		public static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out IStream ppstm);
	}
}