using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;  
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;

	[Guid("072CDB1D-745E-4213-9124-53667725B839"),  // We indicate a specific CLSID for "SimpleCOMObject" for convenience of searching the registry.
	ClassInterface(ClassInterfaceType.None)  // Specify that we will not generate any additional interface with a name like _SimpleCOMObject.
	]
	public class FileComObject :
		ReferenceCountedObjectBase, 
		IPersistFile,
		IOleItemContainer
	{
		private Altaxo.AltaxoDocument _currentProject;

		/// <summary>
		/// The file moniker for the current project name.
		/// </summary>
		private IMoniker _fileMoniker;

		/// <summary>
		/// The cookie that is received when the _moniker is registered in the running object table.
		/// </summary>
		private int _fileMonikerRotCookie;

		/// <summary>
		/// Moniker that is a combination of the file moniker with a wildcard item, and thus is valid for all items in file.
		/// </summary>
		private IMoniker _fileWithWildCardItemMoniker;

		/// <summary>
		/// The cookie that is received when <see cref="_fileWithWildCardItemMoniker"/> is registered in the running object table.
		/// </summary>
		private int _fileWithWildCardItemMonikerRotCookie;

		public event Action<IMoniker> FileMonikerChanged;

		private List<DocumentComObject> _documentComObjects;

		public FileComObject(ComManager comManager)
			: base(comManager)
		{
			// 1. Monitor the change of the project instance
			Current.ProjectService.ProjectChanged += EhCurrentProjectInstanceChanged;
			Current.ProjectService.ProjectRenamed += EhCurrentProjectFileNameChanged;

			_currentProject = Current.Project;

			// 2. inside the project, monitor the change of any child documents that are Com objects (graphs)
			_currentProject.GraphDocumentCollection.CollectionChanged += EhGraphDocumentRenamed;
			_documentComObjects = new List<DocumentComObject>();

			EhCurrentProjectInstanceChanged(null, null);
		}

		public IMoniker FileMoniker { get { return _fileMoniker; } }

		private void EhCurrentProjectInstanceChanged(object sender, Altaxo.Main.ProjectEventArgs e)
		{
			if (object.ReferenceEquals(Current.Project, _currentProject))
				return;

#if COMLOGGING
			Debug.ReportInfo("FileComObject.EhCurrentProjectInstanceChanged");
#endif

			if (null != _currentProject)
			{
				_currentProject.GraphDocumentCollection.CollectionChanged -= EhGraphDocumentRenamed;
			}

			_currentProject = Current.Project;

			if (null != _currentProject)
			{
				_currentProject.GraphDocumentCollection.CollectionChanged += EhGraphDocumentRenamed;
				EhCurrentProjectFileNameChanged(Current.ProjectService.CurrentProjectFileName);
			}
		}

		private void EhGraphDocumentRenamed(Main.NamedObjectCollectionChangeType changeType, object item, string oldName, string newName)
		{
			if (changeType == Main.NamedObjectCollectionChangeType.ItemRenamed)
			{
				foreach (DocumentComObject comObj in _documentComObjects)
				{
					if (object.ReferenceEquals(comObj.Document, item))
						comObj.EhDocumentRenamed(_fileMoniker);
				}
			}
		}

		private void EhCurrentProjectFileNameChanged(object sender, Altaxo.Main.ProjectRenameEventArgs e)
		{
			EhCurrentProjectFileNameChanged(e.NewName);
		}

		private void EhCurrentProjectFileNameChanged(string fileName)
		{
			// see Brockschmidt, Inside Ole 2nd ed., page 996

#if COMLOGGING
			Debug.ReportInfo("FileComObject.EhCurrentProjectFileNameChanged");
#endif

			ROTUnregister(ref _fileWithWildCardItemMonikerRotCookie);
			_fileWithWildCardItemMoniker = null;
			ROTUnregister(ref _fileMonikerRotCookie);
			_fileMoniker = null;

			if (!string.IsNullOrEmpty(fileName))
			{
				Ole32Func.CreateFileMoniker(fileName, out _fileMoniker);
				if (null != _fileMoniker)
				{
					ROTRegisterAsRunning(_fileMoniker, this, ref _fileMonikerRotCookie, typeof(IPersistFile));

					// Notify all other item Com objects of the new _fileMoniker
					if (null != FileMonikerChanged)
						FileMonikerChanged(_fileMoniker);

					// now register also a file moniker with a wild card item, that handles all items that are not open in the moment
					IMoniker wildCardItemMoniker;
					Ole32Func.CreateItemMoniker("!", "\\", out wildCardItemMoniker);
					if (null != wildCardItemMoniker)
					{
						_fileMoniker.ComposeWith(wildCardItemMoniker, false, out _fileWithWildCardItemMoniker);
						ROTRegisterAsRunning(_fileWithWildCardItemMoniker, this, ref _fileWithWildCardItemMonikerRotCookie, typeof(IOleItemContainer));
					}
				}
			}
		}

		public IList<DocumentComObject> DocumentComObjects
		{
			get
			{
				return _documentComObjects;
			}
		}

		#region Running Object Table management (ROT)

		internal static IRunningObjectTable GetROT()
		{
			IRunningObjectTable rot;
			Int32 hr = Ole32Func.GetRunningObjectTable(0, out rot);
			System.Diagnostics.Debug.Assert(hr == ComReturnValue.NOERROR);
			return rot;
		}

		public static string GetDisplayName(IMoniker m)
		{
			string s;
			IBindCtx bc = CreateBindCtx();
			m.GetDisplayName(bc, null, out s);
			Marshal.ReleaseComObject(bc);  // seems to be recommended
			return s;
		}

		public static IBindCtx CreateBindCtx()
		{
			IBindCtx bc;
			int rc = Ole32Func.CreateBindCtx(0, out bc);
			System.Diagnostics.Debug.Assert(rc == ComReturnValue.S_OK);
			return bc;
		}

		internal static void ROTUnregister(ref int cookie)
		{
			// Revoke any existing file moniker. p988
			IRunningObjectTable rot = GetROT();
			if (0 != cookie)
			{
				rot.Revoke(cookie);
				cookie = 0;
			}
		}

		private static void ROTRegisterAsRunning(IMoniker new_moniker, object o, ref int rot_cookie, Type intf)
		{
			// Revoke any existing file moniker. p988
			ROTUnregister(ref rot_cookie);

			// Register the moniker in the running object table (ROT).
#if COMLOGGING
			Debug.ReportInfo("Registering {0} in ROT", GetDisplayName(new_moniker));
#endif
			IRunningObjectTable rot = GetROT();
			// This flag solved a terrible problem where Word would stop
			// communicating after its first call to GetObject().
			rot_cookie = rot.Register(1 /*ROTFLAGS_REGISTRATIONKEEPSALIVE*/, o, new_moniker);
		}

		#endregion Running Object Table management (ROT)

		#region Interface IPersistFile

		public void GetClassID(out Guid pClassID)
		{
#if COMLOGGING
			Debug.ReportInfo("FileComObject.GetClassID");
#endif

			pClassID = this.GetType().GUID;
		}

		public void GetCurFile(out string ppszFileName)
		{
#if COMLOGGING
			Debug.ReportInfo("FileComObject.GetCurFile -> {0}", _currentProject.FileName);
#endif

			ppszFileName = Current.ProjectService.CurrentProjectFileName;
		}

		public int IsDirty()
		{
#if COMLOGGING
			Debug.ReportInfo("FileComObject.IsDirty -> FALSE");
#endif
			return ComReturnValue.S_FALSE;
		}

		public void Load(string pszFileName, int dwMode)
		{
#if COMLOGGING
			Debug.ReportInfo("FileComObject.Load filename: {0}", pszFileName);
#endif

			Current.ProjectService.OpenProject(pszFileName, false);
		}

		public void Save(string pszFileName, bool fRemember)
		{
#if COMLOGGING
			Debug.ReportInfo("FileComObject.Save filename: {0}", pszFileName);
#endif

			Current.ProjectService.SaveProject(pszFileName);
		}

		public void SaveCompleted(string pszFileName)
		{
#if COMLOGGING
			Debug.ReportInfo("FileComObject.SaveCompleted filename: {0}", pszFileName);
#endif
			throw new NotImplementedException();
		}

		#endregion Interface IPersistFile

		#region Interface IOleItemContainer

		public int ParseDisplayName(IBindCtx pbc, string pszDisplayName, out int pchEaten, out IMoniker ppmkOut)
		{
#if COMLOGGING
			Debug.ReportInfo("FileComObject.ParseDisplayName");
#endif
			throw new NotImplementedException();
		}

		public int EnumObjects(int grfFlags, out IEnumUnknown ppenum)
		{
#if COMLOGGING
			Debug.ReportInfo("FileComObject.EnumObjects");
#endif
			throw new NotImplementedException();
		}

		public int LockContainer(bool fLock)
		{
#if COMLOGGING
			Debug.ReportWarning("FileComObject.LockContainer -> not implemented");
#endif

			return ComReturnValue.NOERROR;
		}

		public IntPtr GetObject(string pszItem, int dwSpeedNeeded, IBindCtx pbc, ref Guid riid)
		{
			// Brockschmidt, Inside Ole 2nd ed. page 1003

#if COMLOGGING
			Debug.ReportInfo("FileComObject.GetObject {0}, Requesting Interface : {	}", pszItem, riid);
#endif

			Altaxo.Graph.Gdi.GraphDocument doc;
			bool isRunning = _currentProject.GraphDocumentCollection.TryGetValue(pszItem, out doc);

			if (((int)BINDSPEED.BINDSPEED_IMMEDIATE == dwSpeedNeeded || (int)BINDSPEED.BINDSPEED_MODERATE == dwSpeedNeeded) && !isRunning)
				throw Marshal.GetExceptionForHR(ComReturnValue.MK_E_EXCEEDEDDEADLINE);

			if (null == doc) // in this application we can do nothing but to return intptr.Zero
				return IntPtr.Zero;

			if (riid == Marshal.GenerateGuidForType(typeof(System.Runtime.InteropServices.ComTypes.IDataObject)) ||
				riid == Marshal.GenerateGuidForType(typeof(IOleObject)) ||
				riid == InterfaceGuid.IID_IDispatch ||
				riid == InterfaceGuid.IID_IUnknown)
			{
				var documentComObject = _comManager.GetDocumentsComObjectForDocument(doc);
				IntPtr ppvObject = Marshal.GetComInterfaceForObject(documentComObject, typeof(System.Runtime.InteropServices.ComTypes.IDataObject));

				var action = new Action(() => Current.ProjectService.ShowDocumentView(doc));
				Current.Gui.BeginExecute(action);

				return ppvObject;
			}
			else
			{
				throw new COMException("No interface", unchecked((int)0x80004002));
			}
		}

		public object GetObjectStorage(string pszItem, IBindCtx pbc, ref Guid riid)
		{
#if COMLOGGING
			Debug.ReportInfo("FileComObject.GetObjectStorage");
#endif
			throw new NotImplementedException();
		}

		public int IsRunning(string pszItem)
		{
#if COMLOGGING
			Debug.ReportInfo("FileComObject.IsRunning");
#endif
			Altaxo.Graph.Gdi.GraphDocument doc;
			bool isRunning = _currentProject.GraphDocumentCollection.TryGetValue(pszItem, out doc);
			return isRunning ? ComReturnValue.NOERROR : ComReturnValue.S_FALSE;
		}

		#endregion Interface IOleItemContainer
	}
}