using Altaxo.Graph.Gdi;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;

using System.Windows;

namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;
	using UnmanagedApi.User32;

	public class ComManager : Altaxo.Main.IComManager
	{
		public bool IsActive { get; private set; }

		private int _numberOfObjectsInUse;  // Keeps a count on the total number of objects alive.
		private int _numberOfServerLocks;// Keeps a lock count on this application.

		private ClassFactory_GraphDocumentEmbeddedComObject _classFactoryOfDocumentComObject;

		private ClassFactory_ProjectFileComObject _classFactoryOfFileComObject;

		private GarbageCollector _garbageCollector;

		private Dictionary<GraphDocument, GraphDocumentLinkedComObject> _linkedDocumentsComObjects = new Dictionary<GraphDocument, GraphDocumentLinkedComObject>();

		private GraphDocumentEmbeddedComObject _embeddedComObject;

		private ProjectFileComObject _fileComObject;

		protected WeakReference _lastUsedDataObject;

		/// <summary>
		/// The application is in embedded mode. This does <b>not</b> mean that the application was started with the -embedding flag in the command line! (Since this happens also when the application is
		/// started by clicking a linked object). We can only be sure to be in embedding mode when IOleObject.SetHostNames is called on the DocumentComObject.
		/// </summary>
		public bool IsInEmbeddedObjectMode { get; private set; }

		public string ContainerApplicationName { get; private set; }

		public string ContainerDocumentName { get; private set; }

		public object EmbeddedObject { get { return _embeddedComObject != null ? _embeddedComObject.Document : null; } }

		public bool ApplicationWasStartedWithEmbeddingArg { get; private set; }

		public bool ApplicationShouldExitAfterProcessingArgs { get; private set; }

		public AltaxoComApplicationAdapter ApplicationAdapter { get; private set; }

		private GuiThreadStack _guiThreadStack;

		public ComManager(AltaxoComApplicationAdapter appAdapter)
		{
			ApplicationAdapter = appAdapter;
			_guiThreadStack = new GuiThreadStack(appAdapter.IsInvokeRequiredForGuiThread, appAdapter.InvokeGuiThread);
			_fileComObject = new ProjectFileComObject(this);
		}

		public GraphDocumentEmbeddedComObject GetNewEmbeddedGraphDocumentComObject()
		{
			if (null != _embeddedComObject)
				throw new InvalidOperationException("There is already an embedded object present in this application instance!");

			return new GraphDocumentEmbeddedComObject(this);
		}

		public GraphDocumentLinkedComObject GetDocumentsComObjectForGraphDocument(GraphDocument doc)
		{
			if (null == doc)
				throw new ArgumentNullException();

#if COMLOGGING
			Debug.ReportInfo("{0}.GetDocumentsComObjectForGraphDocument Name={1}", this.GetType().Name, doc.Name);
#endif

			if (null != doc && _linkedDocumentsComObjects.ContainsKey(doc))
				return _linkedDocumentsComObjects[doc];

			// else we must create a new DocumentComObject
			var newComObject = new GraphDocumentLinkedComObject(doc, _fileComObject, this);
			_linkedDocumentsComObjects.Add(doc, newComObject);
			return newComObject;
		}

		public GraphDocumentDataObject GetDocumentsDataObjectForGraphDocument(GraphDocument doc)
		{
			var newComObject = new GraphDocumentDataObject(doc, _fileComObject, this);
			_lastUsedDataObject = new WeakReference(newComObject);
			return newComObject;
		}

		public System.Runtime.InteropServices.ComTypes.IDataObject GetDocumentsComObjectForDocument(object obj)
		{
			if (obj is GraphDocument)
			{
				var doc = (GraphDocument)obj;
				return GetDocumentsComObjectForGraphDocument(doc);
			}

			return null;
		}

		public System.Runtime.InteropServices.ComTypes.IDataObject GetDocumentsDataObjectForDocument(object obj)
		{
			if (obj is GraphDocument)
			{
				var doc = (GraphDocument)obj;
				return GetDocumentsDataObjectForGraphDocument(doc);
			}

			return null;
		}

		public void NotifyDocumentOfDocumentsComObjectChanged(GraphDocumentEmbeddedComObject documentComObject, GraphDocument oldDocument, GraphDocument newDocument)
		{
			System.Diagnostics.Debug.Assert(null == oldDocument);
			System.Diagnostics.Debug.Assert(null != newDocument);
			_embeddedComObject = documentComObject;
			EnterEmbeddedObjectMode();
		}

		public IEnumerable<GraphDocumentLinkedComObject> GraphDocumentLinkedComObjects
		{
			get
			{
				return _linkedDocumentsComObjects.Values;
			}
		}

		public bool IsInvokeRequiredForGuiThread()
		{
			return ApplicationAdapter.IsInvokeRequiredForGuiThread();
		}

		/// <summary>
		/// Invokes the GUI thread (blocking). The special feature is here that there is no deadlock even if another action executed in the Gui tasks waits for completion, supposed
		/// that it has used the <see cref="FromGuiThreadExecute"/> method.
		/// </summary>
		/// <param name="action">The action to be executed in the Gui thread.</param>
		public void InvokeGuiThread(Action action)
		{
			_guiThreadStack.InvokeGuiThread(action);
		}

		/// <summary>
		/// Executes an action <paramref name="action"/> in a separate task (blocking). If the action (or a child task) wants to invoke the Gui thread again, this is possible without deadlock.
		/// This procedure must be called only from the Gui thread. The procedure returns when the action has been finished.
		/// </summary>
		/// <param name="action">The action. Please not that the action is executed in a thread other than the Gui thread.</param>
		/// <exception cref="System.InvalidOperationException">Is thrown if this procedure is called from a thread that is not the Gui thread.</exception>
		public void FromGuiThreadExecute(Action action)
		{
			_guiThreadStack.FromGuiThreadExecute(action);
		}

		public bool IsInEmbeddedMode
		{
			get { return IsInEmbeddedObjectMode; }
			set
			{
				IsInEmbeddedObjectMode |= value;
			}
		}

		public void SetHostNames(string containerApplicationName, string containerFileName)
		{
			// see Brockschmidt, Inside Ole 2nd ed. page 992
			// calling SetHostNames is the only sign that our object is embedded (and thus not linked)
			// this means that we have to switch the user interface from within this function

			IsInEmbeddedMode = true;
			ContainerApplicationName = containerApplicationName;
			ContainerDocumentName = containerFileName;
			ApplicationAdapter.SetHostNames(containerApplicationName, containerFileName, EmbeddedObject);
		}

		public ProjectFileComObject FileComObject
		{
			get
			{
				return _fileComObject;
			}
		}

		// This method performs a thread-safe incrementation of the objects count.
		public int InterlockedIncrementObjectsCount()
		{
			// Increment the global count of objects.
			return Interlocked.Increment(ref _numberOfObjectsInUse);
		}

		// This method performs a thread-safe decrementation the objects count.
		public int InterlockedDecrementObjectsCount()
		{
			// Decrement the global count of objects.
			return Interlocked.Decrement(ref _numberOfObjectsInUse);
		}

		// Returns the total number of objects alive currently.
		public int ObjectsCount
		{
			get
			{
				return _numberOfObjectsInUse;
			}
		}

		// This method performs a thread-safe incrementation the
		// server lock count.
		public int InterlockedIncrementServerLockCount()
		{
			// Increment the global lock count of this server.
			return Interlocked.Increment(ref _numberOfServerLocks);
		}

		// This method performs a thread-safe decrementation the
		// server lock count.
		public int InterlockedDecrementServerLockCount()
		{
			// Decrement the global lock count of this server.
			return Interlocked.Decrement(ref _numberOfServerLocks);
		}

		// Returns the current server lock count.
		public int ServerLockCount
		{
			get
			{
				return _numberOfServerLocks;
			}
		}

		// AttemptToTerminateServer() will check to see if
		// the objects count and the server lock count has
		// both dropped to zero.
		// If so, we post a WM_QUIT message to the main thread's
		// message loop. This will cause the message loop to
		// exit and hence the termination of this application.
		public void AttemptToTerminateServer()
		{
			lock (this)
			{
				// Get the most up-to-date values of these critical data.
				int iObjsInUse = ObjectsCount;
				int iServerLocks = ServerLockCount;

#if COMLOGGING
				{
					// Print out these info for debug purposes.
					StringBuilder sb = new StringBuilder("");
					sb.AppendFormat("NumberOfObjectsInUse : {0}. NumberOfServerLocks : {1}", iObjsInUse, iServerLocks);
					Debug.ReportInfo(sb.ToString());
				}
#endif

				if ((iObjsInUse > 0) || (iServerLocks > 0))
				{
#if COMLOGGING
					Debug.ReportInfo("There are still referenced objects or the server lock count is non-zero.");
#endif
				}
				else
				{
					UIntPtr wParam = new UIntPtr(0);
					IntPtr lParam = new IntPtr(0);

					// Stop the program now

					ApplicationAdapter.BeginClosingApplication();
				}
			}
		}

		public void RegisterApplicationForCom()
		{
			try
			{
				Register(Registry.ClassesRoot);
				return; // if it was successful to register the computer account, we return
			}
			catch (Exception)
			{
			}

			// if not successful to register into HKLM, we use the user's registry
			try
			{
				using (var sf = Registry.CurrentUser.OpenSubKey("Software", true))
				{
					using (var cl = sf.OpenSubKey("Classes", true))
					{
						Register(cl);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public void UnregisterApplicationForCom()
		{
			try
			{
				Unregister(Registry.ClassesRoot);
			}
			catch (Exception)
			{
			}

			// unregister also from the user's registry
			try
			{
				using (var sf = Registry.CurrentUser.OpenSubKey("Software", true))
				{
					using (var cl = sf.OpenSubKey("Classes", true))
					{
						Register(cl);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void Unregister(RegistryKey root)
		{
			root.DeleteSubKey(".axoprj");
			root.DeleteSubKey("Altaxo.Project");
			root.DeleteSubKey("Altaxo.Graph.0");
			root.DeleteSubKey("CLSID\\" + Marshal.GenerateGuidForType(typeof(GraphDocumentEmbeddedComObject)).ToString("B").ToUpperInvariant());
		}

		private void Register(RegistryKey root)
		{
			RegistryKey key1 = null;
			RegistryKey key2 = null;
			RegistryKey key3 = null;
			RegistryKey key4 = null;

			RegistryValueKind applicationFileNameKind = RegistryValueKind.String;
			string applicationFileName = System.Reflection.Assembly.GetEntryAssembly().Location;
			string programFilesPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
			if (applicationFileName.ToUpperInvariant().StartsWith(programFilesPath.ToUpperInvariant()))
			{
				applicationFileNameKind = RegistryValueKind.ExpandString;
				applicationFileName = "%ProgramFiles%" + applicationFileName.Substring(programFilesPath.Length);
			}

			try
			{
				{
					// Register the project file extension
					key1 = root.CreateSubKey(".axoprj");
					key1.SetValue(null, "Altaxo.Project");
				}

				{
					// Register the project file Com object
					key1 = root.CreateSubKey("Altaxo.Project"); // set ProgID
					key1.SetValue("DefaultIcon", string.Format("{0},0", applicationFileName), applicationFileNameKind);
					key2 = key1.CreateSubKey("NotInsertable");
					key2 = key1.CreateSubKey("CLSID");
					var fileComObject_IID = typeof(ProjectFileComObject).GUID.ToString("B").ToUpperInvariant();
					key2.SetValue(null, fileComObject_IID);
					key2 = key1.CreateSubKey("shell");
					key3 = key2.CreateSubKey("open");
					key4 = key3.CreateSubKey("command");
					key4.SetValue(null, string.Format("\"{0}\" \"%1\"", applicationFileName), applicationFileNameKind);
				}

				{
					// publish CLSID of file Com object and associate it with the application
					key1 = root.CreateSubKey("CLSID\\" + typeof(ProjectFileComObject).GUID.ToString("B").ToUpperInvariant());
					key1.SetValue(null, "Altaxo project");
					key2 = key1.CreateSubKey("LocalServer32");
					key2.SetValue(null, applicationFileName, applicationFileNameKind);
				}

				{
					// register the Graph document embedded object (note that this is an Altaxo mini project)
					key1 = root.CreateSubKey(GraphDocumentEmbeddedComObject.USER_TYPE);
					key1.SetValue(null, GraphDocumentEmbeddedComObject.USER_TYPE_LONG);
					key2 = key1.CreateSubKey("CLSID");
					key2.SetValue(null, typeof(GraphDocumentEmbeddedComObject).GUID.ToString("B").ToUpperInvariant());
					key2 = key1.CreateSubKey("Insertable");
				}

				{
					// publish CLSID of file GraphDocumentEmbeddedObject and associate it with the application
					key1 = root.CreateSubKey("CLSID\\" + typeof(GraphDocumentEmbeddedComObject).GUID.ToString("B").ToUpperInvariant());
					key1.SetValue(null, GraphDocumentEmbeddedComObject.USER_TYPE_LONG);

					key2 = key1.CreateSubKey("LocalServer32");
					key2.SetValue(null, applicationFileName, applicationFileNameKind);

					key2 = key1.CreateSubKey("InprocHandler32");
					key2.SetValue(null, "OLE32.DLL"); // The entry InprocHandler32 is neccessary! Without this entry Word does not start the server. (Brockschmidt Inside Ole 2nd ed. says that it isn't neccessary).

					key2 = key1.CreateSubKey("ProgID");
					key2.SetValue(null, GraphDocumentEmbeddedComObject.USER_TYPE);

					key2 = key1.CreateSubKey("VersionIndependentProgID");
					key2.SetValue(null, "Altaxo.Graph");

					key2 = key1.CreateSubKey("Insertable");

					key2 = key1.CreateSubKey("DataFormats");
					key3 = key2.CreateSubKey("GetSet");
					key4 = key3.CreateSubKey("0");
					key4.SetValue(null, "3,9,32,1"); // Metafile on MFPICT in get-direction

					key4 = key3.CreateSubKey("1");
					key4.SetValue(null, "2,9,1,1"); // Bitmap on HGlobal in get-direction

					key2 = key1.CreateSubKey("DefaultIcon");
					key2.SetValue(null, string.Format("{0},0", applicationFileName), applicationFileNameKind);

					key2 = key1.CreateSubKey("verb");
					key3 = key2.CreateSubKey("0");
					key3.SetValue(null, "&Edit,0,2");

					key3 = key2.CreateSubKey("-1");
					key3.SetValue(null, "Show,0,0");

					key3 = key2.CreateSubKey("-2");
					key3.SetValue(null, "Open,0,0");

					key3 = key2.CreateSubKey("-3");
					key3.SetValue(null, "Hide,0,1");

					key2 = key1.CreateSubKey("AuxUserType");
					key3 = key2.CreateSubKey("2");
					key3.SetValue(null, "Altaxo");

					key3 = key2.CreateSubKey("3");
					key3.SetValue(null, "Altaxo Graph Document");

					key2 = key1.CreateSubKey("MiscStatus"); // see Brockschmidt, Inside Ole 2nd ed. page 832
					key2.SetValue(null, ((int)(OLEMISC.OLEMISC_CANTLINKINSIDE)).ToString(System.Globalization.CultureInfo.InvariantCulture)); // DEFAULT: OLEMISC_CANTLINKINSIDE

					key3 = key2.CreateSubKey(((int)DVASPECT.DVASPECT_CONTENT).ToString(System.Globalization.CultureInfo.InvariantCulture)); // For DVASPECT_CONTENT
					key3.SetValue(null, ((int)(OLEMISC.OLEMISC_CANTLINKINSIDE | OLEMISC.OLEMISC_RENDERINGISDEVICEINDEPENDENT)).ToString(System.Globalization.CultureInfo.InvariantCulture));  // OLEMISC_RECOMPOSEONRESIZE | OLEMISC_CANTLINKINSIDE
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error while registering the server:\n" + ex.ToString());
				throw;
			}
			finally
			{
				if (key4 != null)
					key4.Close();
				if (key3 != null)
					key3.Close();
				if (key2 != null)
					key2.Close();
				if (key1 != null)
					key1.Close();
			}
		}

		// ProcessArguments() will process the command-line arguments
		// of this application.
		// If the return value is true, we carry
		// on and start this application.
		// If the return value is false, we terminate
		// this application immediately.
		public bool ProcessArguments(string[] args)
		{
			bool bRet = true;

			if (args.Length > 0)
			{
				switch (args[0].ToLower())
				{
					case "-embedding":
						ApplicationWasStartedWithEmbeddingArg = true;
						break;

					case "-register":
					case "/register":
						RegisterApplicationForCom();
						break;

					case "-unregister":
					case "/unregister":
						UnregisterApplicationForCom();
						break;

					default:
#if COMLOGGING
						Debug.ReportError("Unknown argument: " + args[0] + "\nValid are : -register, -unregister and -embedding");
#endif
						break;
				}
			}

			return bRet;
		}

		public void StartLocalServer()
		{
			IsActive = true;

#if COMLOGGING
			Debug.ReportInfo("Starting local server");
#endif

			{
				// Register the FileComObject
				_classFactoryOfFileComObject = new ClassFactory_ProjectFileComObject(this);
				_classFactoryOfFileComObject.ClassContext = (uint)CLSCTX.CLSCTX_LOCAL_SERVER;
				_classFactoryOfFileComObject.ClassId = Marshal.GenerateGuidForType(typeof(ProjectFileComObject));
				_classFactoryOfFileComObject.Flags = (uint)REGCLS.REGCLS_SINGLEUSE | (uint)REGCLS.REGCLS_SUSPENDED;
				_classFactoryOfFileComObject.RegisterClassObject();
#if COMLOGGING
				Debug.ReportInfo("{0}.StartLocalServer Registered: {1}", this.GetType().Name, _classFactoryOfFileComObject.GetType().Name);
#endif
			}

			if (ApplicationWasStartedWithEmbeddingArg)
			{
				// Register the SimpleCOMObjectClassFactory.
				_classFactoryOfDocumentComObject = new ClassFactory_GraphDocumentEmbeddedComObject(this);
				_classFactoryOfDocumentComObject.ClassContext = (uint)CLSCTX.CLSCTX_LOCAL_SERVER;
				_classFactoryOfDocumentComObject.ClassId = Marshal.GenerateGuidForType(typeof(GraphDocumentEmbeddedComObject));
				_classFactoryOfDocumentComObject.Flags = (uint)REGCLS.REGCLS_SINGLEUSE | (uint)REGCLS.REGCLS_SUSPENDED;
				_classFactoryOfDocumentComObject.RegisterClassObject();
#if COMLOGGING
				Debug.ReportInfo("{0}.StartLocalServer Registered: {1}", this.GetType().Name, _classFactoryOfDocumentComObject.GetType().Name);
#endif
			}

			ClassFactoryBase.ResumeClassObjects();

			// Start up the garbage collection thread.
			_garbageCollector = new GarbageCollector(1000);
			Thread GarbageCollectionThread = new Thread(new ThreadStart(_garbageCollector.GCWatch));

			// Set the name of the thread object.
			GarbageCollectionThread.Name = "GarbCollThread";
			GarbageCollectionThread.IsBackground = true;

			// Start the thread.
			GarbageCollectionThread.Start();
		}

		private void ConvertCurrentClipboardToPermanentDataObject()
		{
			if (IsInvokeRequiredForGuiThread())
				throw new ApplicationException("This function requires to be in the Gui thread");

			DataObjectBase lastUsedDataObject = null;
			// convert the clipboard data object to a permanent .NET data object
			if (null != _lastUsedDataObject && null != (lastUsedDataObject = _lastUsedDataObject.Target as DataObjectBase))
			{
				lastUsedDataObject.ConvertToNetDataObjectAndPutToClipboard();
			}
			lastUsedDataObject = null;
		}

		public void StopLocalServer()
		{
			if (IsInvokeRequiredForGuiThread())
			{
				InvokeGuiThread(ConvertCurrentClipboardToPermanentDataObject);
			}
			else // we are running in the Gui thread
			{
				ConvertCurrentClipboardToPermanentDataObject();
			}

			if (IsInvokeRequiredForGuiThread())
			{
				InternalStopLocalServer();
			}
			else // we are running in the Gui thread
			{
				FromGuiThreadExecute(InternalStopLocalServer);
			}
		}

		/// <summary>
		/// Notifies the ComManager that we are about to enter linked object mode.
		/// </summary>
		/// <remarks>
		/// We do the following here:
		/// <para>
		/// We revoke the class factory of the GraphDocumentComObject. If this is not done and a user copies an embedded graph object, the container application would try
		/// to open the graph mini project just here in this application instance, because the class factory of the GraphDocumentComObject is still active. In order to avoid this,
		/// the class factory of the GraphDocumentComObject is revoked here (if it is active).
		/// </para>
		/// </remarks>
		public void EnterLinkedObjectMode()
		{
			if (null != _classFactoryOfDocumentComObject)
			{
				_classFactoryOfDocumentComObject.RevokeClassObject();
#if COMLOGGING
				Debug.ReportInfo("{0}.EnterLinkedObjectMode Revoked: {1}", this.GetType().Name, _classFactoryOfDocumentComObject.GetType().Name);
#endif
				_classFactoryOfDocumentComObject = null;
			}
		}

		public void EnterEmbeddedObjectMode()
		{
			if (null != _classFactoryOfFileComObject)
			{
				_classFactoryOfFileComObject.RevokeClassObject();
#if COMLOGGING
				Debug.ReportInfo("{0}.EnterEmbeddedObjectMode Revoked: {1}", this.GetType().Name, _classFactoryOfFileComObject.GetType().Name);
#endif
				_classFactoryOfFileComObject = null;
			}
		}

		private void InternalStopLocalServer()
		{
#if COMLOGGING
			Debug.ReportInfo("Stop local server");
#endif

			if (null != _embeddedComObject)
			{
				_embeddedComObject.Dispose();
				_embeddedComObject = null;
			}

			foreach (var co in _linkedDocumentsComObjects.Values)
			{
				co.Dispose();
			}
			_linkedDocumentsComObjects.Clear();

			if (null != _fileComObject)
			{
				_fileComObject.Dispose();
				_fileComObject = null;
			}

			if (null != _classFactoryOfDocumentComObject)
			{
				_classFactoryOfDocumentComObject.RevokeClassObject();
#if COMLOGGING
				Debug.ReportInfo("{0}.StopLocalServer:{1} Revoked.", this.GetType().Name, _classFactoryOfDocumentComObject.GetType().Name);
#endif
				_classFactoryOfDocumentComObject = null;
			}

			if (null != _classFactoryOfFileComObject)
			{
				_classFactoryOfFileComObject.RevokeClassObject();
#if COMLOGGING
				Debug.ReportInfo("{0}.StopLocalServer:{1} Revoked.", this.GetType().Name, _classFactoryOfFileComObject.GetType().Name);
#endif
				_classFactoryOfFileComObject = null;
			}

			if (null != _garbageCollector)
			{
				// Now stop the Garbage Collector thread.
				_garbageCollector.StopThread();
				_garbageCollector.WaitForThreadToStop();
				_garbageCollector = null;
#if COMLOGGING
				Debug.ReportInfo("StopLocalServer: GarbageCollector thread stopped.");
#endif
			}

			IsActive = false;
		}
	}
}