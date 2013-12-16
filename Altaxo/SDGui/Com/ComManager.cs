using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using System.Windows;

namespace Altaxo.Com
{
	using UnmanagedApi.Ole32;

	// Note that ComManager is NOT declared as public.
	// This is so that it will not be exposed to COM when we call regasm
	// or tlbexp.
	public static class ComManager
	{
		private static int _numberOfObjectsInUse;  // Keeps a count on the total number of objects alive.
		private static int _numberOfServerLocks;// Keeps a lock count on this application.

		private static ClassFactory_DocumentComObject _classFactoryOfDocumentComObject;

		private static ClassFactory_FileCOMObject _classFactoryOfFileComObject;

		private static GarbageCollector _garbageCollector;

		public static Dictionary<Altaxo.Graph.Gdi.GraphDocument, DocumentComObject> _documentsComObject = new Dictionary<Altaxo.Graph.Gdi.GraphDocument, DocumentComObject>();

		public static FileComObject _fileComObject;

		/// <summary>
		/// The application is in embedded mode. This does <b>not</b> mean that the application was started with the -embedding flag in the command line! (Since this happens also when the application is
		/// started by clicking a linked object). We can only be sure to be in embedding mode when IOleObject.SetHostNames is called on the DocumentComObject.
		/// </summary>
		public static bool IsInEmbeddedObjectMode { get; set; }

		public static string ContainerApp { get; set; }

		public static string ContainerObject { get; set; }

		public static bool ApplicationWasStartedWithEmbeddingArg { get; private set; }

		public static bool ApplicationShouldExitAfterProcessingArgs { get; private set; }

		public static AltaxoComApplicationAdapter ApplicationAdapter { get; set; }

		public static DocumentComObject GetDocumentsComObjectForDocument(Altaxo.Graph.Gdi.GraphDocument doc)
		{
			if (_documentsComObject.ContainsKey(doc))
				return _documentsComObject[doc];

			// else we must create a new DocumentComObject

			if (null == FileComObject)
				FileComObject = new FileComObject();

			var newComObject = new DocumentComObject(doc, _fileComObject);
			_documentsComObject.Add(doc, newComObject);
			return newComObject;
		}

		public static bool IsInEmbeddedMode
		{
			get { return IsInEmbeddedObjectMode; }
			set
			{
				IsInEmbeddedObjectMode |= value;
			}
		}

		public static void SetHostNames(string containerApplicationName, string containerFileName, bool isInEmbeddedMode)
		{
			// see Brockschmidt, Inside Ole 2nd ed. page 992
			// calling SetHostNames is the only sign that our object is embedded (and thus not linked)
			// this means that we have to switch the user interface from within this function

			ContainerApp = containerApplicationName;
			ContainerObject = containerFileName;
			IsInEmbeddedMode = isInEmbeddedMode;

			ApplicationAdapter.SetHostNames(containerApplicationName, containerFileName, isInEmbeddedMode);
		}

		public static FileComObject FileComObject
		{
			get
			{
				return _fileComObject;
			}
			set
			{
				if (null != _fileComObject && null != value)
					throw new ArgumentException("Trying to set ComObject, but another object is already present");

				_fileComObject = value;
			}
		}

		// This method performs a thread-safe incrementation of the objects count.
		public static int InterlockedIncrementObjectsCount()
		{
			// Increment the global count of objects.
			return Interlocked.Increment(ref _numberOfObjectsInUse);
		}

		// This method performs a thread-safe decrementation the objects count.
		public static int InterlockedDecrementObjectsCount()
		{
			// Decrement the global count of objects.
			return Interlocked.Decrement(ref _numberOfObjectsInUse);
		}

		// Returns the total number of objects alive currently.
		public static int ObjectsCount
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
		public static int InterlockedIncrementServerLockCount()
		{
			// Increment the global lock count of this server.
			return Interlocked.Increment(ref _numberOfServerLocks);
		}

		// This method performs a thread-safe decrementation the
		// server lock count.
		public static int InterlockedDecrementServerLockCount()
		{
			// Decrement the global lock count of this server.
			return Interlocked.Decrement(ref _numberOfServerLocks);
		}

		// Returns the current server lock count.
		public static int ServerLockCount
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
		public static void AttemptToTerminateServer()
		{
			lock (typeof(ComManager))
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


		private static void UnRegister(RegistryKey root)
		{
				root.DeleteSubKey(".axoprj");
				root.DeleteSubKey("Altaxo.Project");
				root.DeleteSubKey("Altaxo.Graph.0");
				root.DeleteSubKey("CLSID\\" + Marshal.GenerateGuidForType(typeof(DocumentComObject)).ToString("B").ToUpperInvariant());
		}

		private static void Register(RegistryKey root)
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
				var fileComObject_IID = Marshal.GenerateGuidForType(typeof(FileComObject)).ToString("B").ToUpperInvariant();
				key2.SetValue(null, fileComObject_IID);

				key = root.CreateSubKey("CLSID\\" + fileComObject_IID); // associate CLSID of FileComObject with Application
				key.SetValue(null, "Altaxo project");
				key2 = key.CreateSubKey("LocalServer32");
				key2.SetValue(null, System.Reflection.Assembly.GetEntryAssembly().Location);

				key = root.CreateSubKey("Altaxo.Graph.0");
				key.SetValue(null, "Altaxo Graph-Document");
				key2 = key.CreateSubKey("CLSID");
				key2.SetValue(null, Marshal.GenerateGuidForType(typeof(DocumentComObject)).ToString("B").ToUpperInvariant());
				key2 = key.CreateSubKey("Insertable");

				key = root.CreateSubKey("CLSID\\" + Marshal.GenerateGuidForType(typeof(DocumentComObject)).ToString("B").ToUpperInvariant());
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

				key2 = key.CreateSubKey("MiscStatus");
				key2.SetValue(null, "16");

				key3 = key2.CreateSubKey("1");
				key3.SetValue(null, "17");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error while registering the server:\n" + ex.ToString());
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
		public static bool ProcessArguments(string[] args)
		{
			bool bRet = true;

			if (args.Length > 0)
			{
				RegistryKey key = null;
				RegistryKey key2 = null;

				switch (args[0].ToLower())
				{
					case "-embedding":
						ApplicationWasStartedWithEmbeddingArg = true;
						break;

					case "-register":
					case "/register":
						ApplicationShouldExitAfterProcessingArgs = true;
						bRet = false;
						try
						{
							Register(Registry.ClassesRoot);
							break;
						}
						catch (Exception)
						{

						}
						try
						{
							var sf = Registry.CurrentUser.OpenSubKey("Software", true);
							var cl = sf.OpenSubKey("Classes", true);
							Register(cl);
							cl.Close();
							sf.Close();
						}
						catch (Exception)
						{

						}
						break;

					case "-unregister":
					case "/unregister":
						try
						{
							UnRegister(Registry.ClassesRoot);
							break;
						}
						catch (Exception)
						{

						}
						try
						{
							var sf = Registry.CurrentUser.OpenSubKey("Software", true);
							var cl = sf.OpenSubKey("Classes", true);
							UnRegister(cl);
							cl.Close();
							sf.Close();
						}
						catch (Exception)
						{

						}


						try
						{
							key = Registry.ClassesRoot.OpenSubKey("CLSID\\" + Marshal.GenerateGuidForType(typeof(DocumentComObject)).ToString("B"), true);
							key.DeleteSubKey("LocalServer32");
						}
						catch (Exception ex)
						{
							MessageBox.Show("Error while unregistering the server:\n" + ex.ToString());
						}
						finally
						{
							if (key != null)
								key.Close();
							if (key2 != null)
								key2.Close();
						}
						bRet = false;
						ApplicationShouldExitAfterProcessingArgs = true;
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

		public static void StartLocalServer()
		{
#if COMLOGGING
			Debug.ReportInfo("Starting local server");
#endif

			// Initialize critical member variables.
			_numberOfObjectsInUse = 0;
			_numberOfServerLocks = 0;

			// Register the FileComObject
			_classFactoryOfFileComObject = new ClassFactory_FileCOMObject();
			_classFactoryOfFileComObject.ClassContext = (uint)CLSCTX.CLSCTX_LOCAL_SERVER;
			_classFactoryOfFileComObject.ClassId = Marshal.GenerateGuidForType(typeof(FileComObject));
			_classFactoryOfFileComObject.Flags = (uint)REGCLS.REGCLS_SINGLEUSE | (uint)REGCLS.REGCLS_SUSPENDED;
			_classFactoryOfFileComObject.RegisterClassObject();

			// Register the SimpleCOMObjectClassFactory.
			_classFactoryOfDocumentComObject = new ClassFactory_DocumentComObject();
			_classFactoryOfDocumentComObject.ClassContext = (uint)CLSCTX.CLSCTX_LOCAL_SERVER;
			_classFactoryOfDocumentComObject.ClassId = Marshal.GenerateGuidForType(typeof(DocumentComObject));
			_classFactoryOfDocumentComObject.Flags = (uint)REGCLS.REGCLS_SINGLEUSE | (uint)REGCLS.REGCLS_SUSPENDED;
			_classFactoryOfDocumentComObject.RegisterClassObject();
			ClassFactoryBase.ResumeClassObjects();

			// Start up the garbage collection thread.
			_garbageCollector = new GarbageCollector(1000);
			Thread GarbageCollectionThread = new Thread(new ThreadStart(_garbageCollector.GCWatch));

			// Set the name of the thread object.
			GarbageCollectionThread.Name = "Garbage Collection Thread";
			GarbageCollectionThread.IsBackground = true;

			// Start the thread.
			GarbageCollectionThread.Start();
		}

		public static void StopLocalServer()
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

		#endregion
	}
}