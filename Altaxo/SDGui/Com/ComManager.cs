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

	// Note that ComManager is NOT declared as public.
	// This is so that it will not be exposed to COM when we call regasm
	// or tlbexp.
	public class ComManager : Altaxo.Main.IComManager
	{
		//		private Font _font;
	


		public bool IsActive { get; private set; }

		private int _numberOfObjectsInUse;  // Keeps a count on the total number of objects alive.
		private int _numberOfServerLocks;// Keeps a lock count on this application.

		private ClassFactory_GraphDocumentComObject _classFactoryOfDocumentComObject;

		private ClassFactory_ProjectFileComObject _classFactoryOfFileComObject;

		private GarbageCollector _garbageCollector;

		public Dictionary<GraphDocument, GraphDocumentComObject> _documentsComObject = new Dictionary<GraphDocument, GraphDocumentComObject>();

		public ProjectFileComObject _fileComObject;

		/// <summary>
		/// The application is in embedded mode. This does <b>not</b> mean that the application was started with the -embedding flag in the command line! (Since this happens also when the application is
		/// started by clicking a linked object). We can only be sure to be in embedding mode when IOleObject.SetHostNames is called on the DocumentComObject.
		/// </summary>
		public bool IsInEmbeddedObjectMode { get; private set; }

		public string ContainerApplicationName { get; private set; }

		public string ContainerDocumentName { get; private set; }

		public object EmbeddedObject { get; private set; }

		public bool ApplicationWasStartedWithEmbeddingArg { get; private set; }

		public bool ApplicationShouldExitAfterProcessingArgs { get; private set; }

		public AltaxoComApplicationAdapter ApplicationAdapter { get; private set; }

		private GuiThreadStack _guiThreadStack;

		public ComManager(AltaxoComApplicationAdapter appAdapter)
		{
			ApplicationAdapter = appAdapter;
			_guiThreadStack = new GuiThreadStack(appAdapter.IsInvokeRequiredForGuiThread, appAdapter.InvokeGuiThread);
		}


	

		public GraphDocumentComObject GetDocumentsComObjectForGraphDocument(GraphDocument doc)
		{
			if (null!=doc && _documentsComObject.ContainsKey(doc))
				return _documentsComObject[doc];

			// else we must create a new DocumentComObject
			var newComObject = new GraphDocumentComObject(doc, _fileComObject, this);

			// note: the addition to the dictionary is done by the DocumentComObject itself, that's why it is not done here

			return newComObject;
		}

		public GraphDocumentDataObject GetDocumentsDataObjectForGraphDocument(GraphDocument doc)
		{
			var newComObject = new GraphDocumentDataObject(doc, _fileComObject, this);
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

		public void NotifyDocumentOfDocumentsComObjectChanged(GraphDocumentComObject documentComObject, GraphDocument oldDocument, GraphDocument newDocument)
		{
			if (null != oldDocument)
				_documentsComObject.Remove(oldDocument);

			if (null != newDocument)
				_documentsComObject.Add(newDocument, documentComObject);
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

		public void SetHostNames(string containerApplicationName, string containerFileName, object embeddedObject)
		{
			// see Brockschmidt, Inside Ole 2nd ed. page 992
			// calling SetHostNames is the only sign that our object is embedded (and thus not linked)
			// this means that we have to switch the user interface from within this function

			IsInEmbeddedMode = true;
			ContainerApplicationName = containerApplicationName;
			ContainerDocumentName = containerFileName;
			EmbeddedObject = embeddedObject;

			ApplicationAdapter.SetHostNames(containerApplicationName, containerFileName, embeddedObject);
		}

		public ProjectFileComObject FileComObject
		{
			get
			{
				if (null == _fileComObject)
					_fileComObject = new ProjectFileComObject(this);

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
				lock (typeof(ComManager))
				{
					return _numberOfObjectsInUse;
				}
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
				lock (typeof(ComManager))
				{
					return _numberOfServerLocks;
				}
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
			root.DeleteSubKey("CLSID\\" + Marshal.GenerateGuidForType(typeof(GraphDocumentComObject)).ToString("B").ToUpperInvariant());
		}

		private void Register(RegistryKey root)
		{
			RegistryKey key = null;
			RegistryKey key2 = null;
			RegistryKey key3 = null;
			RegistryKey key4 = null;

			try
			{
				key = root.CreateSubKey(".axoprj");
				key.SetValue(null, "Altaxo.Project");

				key = root.CreateSubKey("Altaxo.Project"); // set ProgID
				key2 = key.CreateSubKey("NotInsertable");
				key2 = key.CreateSubKey("CLSID");
				var fileComObject_IID = Marshal.GenerateGuidForType(typeof(ProjectFileComObject)).ToString("B").ToUpperInvariant();
				key2.SetValue(null, fileComObject_IID);

				key = root.CreateSubKey("CLSID\\" + fileComObject_IID); // associate CLSID of FileComObject with Application
				key.SetValue(null, "Altaxo project");
				key2 = key.CreateSubKey("LocalServer32");
				key2.SetValue(null, System.Reflection.Assembly.GetEntryAssembly().Location);

				key = root.CreateSubKey("Altaxo.Graph.0");
				key.SetValue(null, "Altaxo Graph-Document");
				key2 = key.CreateSubKey("CLSID");
				key2.SetValue(null, Marshal.GenerateGuidForType(typeof(GraphDocumentComObject)).ToString("B").ToUpperInvariant());
				key2 = key.CreateSubKey("Insertable");

				key = root.CreateSubKey("CLSID\\" + Marshal.GenerateGuidForType(typeof(GraphDocumentComObject)).ToString("B").ToUpperInvariant());
				key.SetValue(null, "Altaxo Graph-Document");

				key2 = key.CreateSubKey("LocalServer32");
				key2.SetValue(null, System.Reflection.Assembly.GetEntryAssembly().Location);

				key2 = key.CreateSubKey("InprocHandler32");
				key2.SetValue(null, "OLE32.DLL"); // The entry InprocHandler32 is neccessary! Without this entry Word does not start the server. (Brockschmidt Inside Ole 2nd ed. says that it isn't neccessary).

				key2 = key.CreateSubKey("ProgID");
				key2.SetValue(null, "Altaxo.Graph.0");

				key2 = key.CreateSubKey("VersionIndependentProgID");
				key2.SetValue(null, "Altaxo.Graph");

				key2 = key.CreateSubKey("Insertable");

				key2 = key.CreateSubKey("DataFormats");
				key3 = key2.CreateSubKey("GetSet");
				key4 = key3.CreateSubKey("0");
				key4.SetValue(null, "3,9,32,1"); // Metafile on MFPICT in get-direction

				key4 = key3.CreateSubKey("1");
				key4.SetValue(null, "2,9,1,1"); // Bitmap on HGlobal in get-direction

				key2 = key.CreateSubKey("DefaultIcon");
				key2.SetValue(null, System.Reflection.Assembly.GetEntryAssembly().Location + ",0");

				key2 = key.CreateSubKey("verb");
				key3 = key2.CreateSubKey("0");
				key3.SetValue(null, "&Edit,0,2");

				key3 = key2.CreateSubKey("-1");
				key3.SetValue(null, "Show,0,0");

				key3 = key2.CreateSubKey("-2");
				key3.SetValue(null, "Open,0,0");

				key3 = key2.CreateSubKey("-3");
				key3.SetValue(null, "Hide,0,1");

				key2 = key.CreateSubKey("AuxUserType");
				key3 = key2.CreateSubKey("2");
				key3.SetValue(null, "Altaxo");

				key3 = key2.CreateSubKey("3");
				key3.SetValue(null, "Altaxo Graph Document");

				key2 = key.CreateSubKey("MiscStatus"); // see Brockschmidt, Inside Ole 2nd ed. page 832
				key2.SetValue(null, ((int)(OLEMISC.OLEMISC_CANTLINKINSIDE)).ToString(System.Globalization.CultureInfo.InvariantCulture)); // DEFAULT: OLEMISC_CANTLINKINSIDE

				key3 = key2.CreateSubKey(((int)DVASPECT.DVASPECT_CONTENT).ToString(System.Globalization.CultureInfo.InvariantCulture)); // For DVASPECT_CONTENT
				key3.SetValue(null, ((int)(OLEMISC.OLEMISC_RECOMPOSEONRESIZE | OLEMISC.OLEMISC_CANTLINKINSIDE | OLEMISC.OLEMISC_RENDERINGISDEVICEINDEPENDENT)).ToString(System.Globalization.CultureInfo.InvariantCulture));  // OLEMISC_RECOMPOSEONRESIZE | OLEMISC_CANTLINKINSIDE
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
				if (key != null)
					key.Close();
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

			// Initialize critical member variables.
			_numberOfObjectsInUse = 0;
			_numberOfServerLocks = 0;

			// Register the FileComObject
			_classFactoryOfFileComObject = new ClassFactory_ProjectFileComObject(this);
			_classFactoryOfFileComObject.ClassContext = (uint)CLSCTX.CLSCTX_LOCAL_SERVER;
			_classFactoryOfFileComObject.ClassId = Marshal.GenerateGuidForType(typeof(ProjectFileComObject));
			_classFactoryOfFileComObject.Flags = (uint)REGCLS.REGCLS_SINGLEUSE | (uint)REGCLS.REGCLS_SUSPENDED;
			_classFactoryOfFileComObject.RegisterClassObject();

			// Register the SimpleCOMObjectClassFactory.
			_classFactoryOfDocumentComObject = new ClassFactory_GraphDocumentComObject(this);
			_classFactoryOfDocumentComObject.ClassContext = (uint)CLSCTX.CLSCTX_LOCAL_SERVER;
			_classFactoryOfDocumentComObject.ClassId = Marshal.GenerateGuidForType(typeof(GraphDocumentComObject));
			_classFactoryOfDocumentComObject.Flags = (uint)REGCLS.REGCLS_SINGLEUSE | (uint)REGCLS.REGCLS_SUSPENDED;
			_classFactoryOfDocumentComObject.RegisterClassObject();
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

		public void StopLocalServer()
		{
			if (IsInvokeRequiredForGuiThread())
				InternalStopLocalServer();
			else
				FromGuiThreadExecute(InternalStopLocalServer);
		}

		private void InternalStopLocalServer()
		{
#if COMLOGGING
			Debug.ReportInfo("Stop local server");
#endif

			foreach (var co in _documentsComObject.Values)
			{
				co.Dispose();
			}
			_documentsComObject.Clear();

			if (null != _fileComObject)
			{
				_fileComObject = null;
			}

			if (null != _classFactoryOfDocumentComObject)
			{
				// Revoke the class factory immediately.
				// Don't wait until the thread has stopped before
				// we perform revokation.
				_classFactoryOfDocumentComObject.RevokeClassObject();
				_classFactoryOfDocumentComObject = null;
#if COMLOGGING
				Debug.ReportInfo("StopLocalServer: SimpleCOMObjectClassFactory Revoked.");
#endif
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

		#region Static helper functions

		public static string NormalStringToMonikerNameString(string rawString)
		{
			var stb = new StringBuilder(rawString.Length + 20);

			foreach (char c in rawString)
			{
				switch (c)
				{
					case '!':
						stb.Append("%21");
						break;

					case '%':
						stb.Append("%25");
						break;

					default:
						stb.Append(c);
						break;
				}
			}
			return stb.ToString();
		}

		public static string MonikerNameStringToNormalString(string rawString)
		{
			var stb = new StringBuilder(rawString.Length);

			int startIndex = 0;
			while (startIndex < rawString.Length)
			{
				int idx = rawString.IndexOf('%', startIndex);

				if (idx < 0)
					stb.Append(rawString.Substring(startIndex, rawString.Length - startIndex)); // no Escape sequence found -> we append the rest of the string
				else if ((idx - 1) > startIndex)
					stb.Append(rawString.Substring(startIndex, idx - 1 - startIndex)); // possible escape sequence found, -> we append until (but not including) the escape char

				int remainingChars = rawString.Length - idx;
				if (remainingChars >= 3)
				{
					string subString = rawString.Substring(idx, 3);
					switch (subString)
					{
						case "%21":
							stb.Append('!');
							startIndex += 3;
							break;

						case "%25":
							stb.Append('%');
							startIndex += 3;
							break;

						default:
							stb.Append(rawString[idx]);
							startIndex += 1;
							break;
					}
				}
				else // to less remaining chars
				{
					stb.Append(rawString[idx]);
					startIndex += 1;
				}
			}
			return stb.ToString();
		}

		#endregion Static helper functions
	}
}