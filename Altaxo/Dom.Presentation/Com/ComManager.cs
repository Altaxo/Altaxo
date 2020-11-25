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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Windows;
using Altaxo.Graph.Gdi;
using Microsoft.Win32;

namespace Altaxo.Com
{
  using Graph;
  using UnmanagedApi.Ole32;

  public class ComManager : Altaxo.Main.IComManager
  {
    public bool IsActive { get; private set; }

    /// <summary>The total number of objects in use.</summary>
    private int _numberOfObjectsInUse;

    /// <summary>The number of server locks.</summary>
    private int _numberOfServerLocks;

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

    public object EmbeddedObject { get { return _embeddedComObject?.Document; } }

    public bool ApplicationWasStartedWithEmbeddingArg { get; private set; }

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
      if (_embeddedComObject is not null)
        throw new InvalidOperationException("There is already an embedded object present in this application instance!");

      return new GraphDocumentEmbeddedComObject(this);
    }

    public GraphDocumentLinkedComObject GetDocumentsComObjectForGraphDocument(GraphDocument doc)
    {
      if (doc is null)
        throw new ArgumentNullException();

      ComDebug.ReportInfo("{0}.GetDocumentsComObjectForGraphDocument Name={1}", GetType().Name, doc.Name);

      if (doc is not null && _linkedDocumentsComObjects.ContainsKey(doc))
        return _linkedDocumentsComObjects[doc];

      // else we must create a new DocumentComObject
      var newComObject = new GraphDocumentLinkedComObject(doc, _fileComObject, this);
      _linkedDocumentsComObjects.Add(doc, newComObject);
      return newComObject;
    }

    public GraphDocumentDataObject GetDocumentsDataObjectForGraphDocument(GraphDocumentBase doc)
    {
      var newComObject = new GraphDocumentDataObject(doc, _fileComObject, this);
      _lastUsedDataObject = new WeakReference(newComObject);
      return newComObject;
    }

    public System.Runtime.InteropServices.ComTypes.IDataObject GetDocumentsComObjectForDocument(object obj)
    {
      if (obj is GraphDocument doc)
      {
        return GetDocumentsComObjectForGraphDocument(doc);
      }

      return null;
    }

    public System.Runtime.InteropServices.ComTypes.IDataObject GetDocumentsDataObjectForDocument(object obj)
    {
      if (obj is GraphDocumentBase doc)
      {
        return GetDocumentsDataObjectForGraphDocument(doc);
      }

      return null;
    }

    public void NotifyDocumentOfDocumentsComObjectChanged(GraphDocumentEmbeddedComObject documentComObject, GraphDocumentBase oldDocument, GraphDocumentBase newDocument)
    {
      if (oldDocument is not null)
        throw new ArgumentException(nameof(oldDocument) + " should be null");
      if (newDocument is null)
        throw new ArgumentNullException(nameof(newDocument));

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

        {
          // Print out these info for debug purposes.
          var sb = new StringBuilder("");
          sb.AppendFormat("NumberOfObjectsInUse : {0}. NumberOfServerLocks : {1}", iObjsInUse, iServerLocks);
          ComDebug.ReportInfo(sb.ToString());
        }

        if ((iObjsInUse > 0) || (iServerLocks > 0))
        {
          ComDebug.ReportInfo("There are still referenced objects or the server lock count is non-zero.");
        }
        else
        {
          var wParam = new UIntPtr(0);
          var lParam = new IntPtr(0);

          // Stop the program now

          ApplicationAdapter.BeginClosingApplication();
        }
      }
    }

    public void RegisterApplicationForCom()
    {
      try
      {
        {
          // Test if we have write access to the registry
          // note: it was not helpful (Windows 8.1, 2013-01-18) to use "Registry.ClassesRoot.OpenSubKey("CLSID", RegistryKeyPermissionCheck.ReadWriteSubTree);" -> it simply threw no exception if logged on as normal user!!
          string testkeystring = "6F06713B-FFC4-46B7-BECF-7BC228AC9C0E";
          Registry.ClassesRoot.CreateSubKey(testkeystring, RegistryKeyPermissionCheck.ReadWriteSubTree).Close();
          Registry.ClassesRoot.DeleteSubKeyTree(testkeystring, false);
        }

        Register(Registry.LocalMachine, WOW_Mode.Reg32); // first register the special 32 bit mode, then
        Register(Registry.LocalMachine, WOW_Mode.Reg64); // the "any CPU" mode, so that for important keys the 64 bit mode wins
        return; // if it was successful to register the computer account, we return
      }
      catch (Exception)
      {
      }

      // if not successful to register into HKLM, we use the user's registry
      try
      {
        Register(Registry.CurrentUser, WOW_Mode.Reg32); // first register the special 32 bit mode, then
        Register(Registry.CurrentUser, WOW_Mode.Reg64); // the "any CPU" mode, so that for important keys the 64 bit mode wins
      }
      catch (Exception)
      {
      }
    }

    public void UnregisterApplicationForCom()
    {
      try
      {
        Unregister(Registry.LocalMachine, WOW_Mode.Reg32);
        Unregister(Registry.LocalMachine, WOW_Mode.Reg64);
      }
      catch (Exception ex)
      {
        Current.Console.WriteLine("Unregistering Altaxo has caused an exception. Details: ");
        Current.Console.WriteLine(ex.ToString());
      }

      // unregister also from the user's registry
      try
      {
        Unregister(Registry.CurrentUser, WOW_Mode.Reg32);
        Unregister(Registry.CurrentUser, WOW_Mode.Reg64);
      }
      catch (Exception ex)
      {
        Current.Console.WriteLine("Unregistering Altaxo has caused an exception. Details: ");
        Current.Console.WriteLine(ex.ToString());
      }
    }

    private void Unregister(RegistryKey rootKey, WOW_Mode wowMode)
    {
      var keySW = rootKey.CreateSubKey("Software", wowMode);
      var keyCR = keySW.CreateSubKey("Classes", wowMode);
      var keyCLSID = keyCR.CreateSubKey("CLSID", wowMode);
      var keyApp = keyCR.CreateSubKey("AppID", wowMode);

      keyCR.DeleteSubKeyTree(".axoprj", false);

      keyCR.DeleteSubKeyTree("Altaxo.Project", false);
      keyCLSID.DeleteSubKeyTree(typeof(ProjectFileComObject).GUID.ToString("B").ToUpperInvariant(), false);

      keyCR.DeleteSubKeyTree(GraphDocumentEmbeddedComObject.USER_TYPE, false);
      keyCLSID.DeleteSubKeyTree(typeof(GraphDocumentEmbeddedComObject).GUID.ToString("B").ToUpperInvariant(), false);
      keyApp.DeleteSubKeyTree(typeof(GraphDocumentEmbeddedComObject).GUID.ToString("B").ToUpperInvariant(), false);

      if (keyApp is not null)
        keyApp.Close();
      if (keyCLSID is not null)
        keyCLSID.Close();
      if (keyCR is not null)
        keyCR.Close();
      if (keySW is not null)
        keySW.Close();
    }

    private void Register(RegistryKey rootKey, WOW_Mode wowMode)
    {
      RegistryKey keySW = null;
      RegistryKey keyCR = null;
      RegistryKey keyCLSID = null;
      RegistryKey key1 = null;
      RegistryKey key2 = null;
      RegistryKey key3 = null;
      RegistryKey key4 = null;

      RegistryValueKind applicationFileNameKind = RegistryValueKind.String;
      string applicationFileName = System.Reflection.Assembly.GetEntryAssembly().Location;
      if (wowMode == WOW_Mode.Reg32)
      {
        var p = System.IO.Path.GetFileNameWithoutExtension(applicationFileName);
        applicationFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(applicationFileName), p + "32.exe");
      }

      string programFilesPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
      if (applicationFileName.ToUpperInvariant().StartsWith(programFilesPath.ToUpperInvariant()))
      {
        applicationFileNameKind = RegistryValueKind.ExpandString;
        applicationFileName = "%ProgramFiles%" + applicationFileName.Substring(programFilesPath.Length);
      }

      try
      {
        keySW = rootKey.CreateSubKey("Software", wowMode);
        keyCR = keySW.CreateSubKey("Classes", wowMode);
        keyCLSID = keyCR.CreateSubKey("CLSID", wowMode);

        {
          // Register the project file extension
          key1 = keyCR.CreateSubKey(".axoprj", wowMode);
          key1.SetValue(null, "Altaxo.Project");
        }

        {
          // Register the project file Com object
          key1 = keyCR.CreateSubKey("Altaxo.Project", wowMode); // set ProgID
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
          key1 = keyCLSID.CreateSubKey(typeof(ProjectFileComObject).GUID.ToString("B").ToUpperInvariant(), wowMode);
          key1.SetValue(null, "Altaxo project");
          key2 = key1.CreateSubKey("LocalServer32");
          key2.SetValue(null, applicationFileName, applicationFileNameKind);
        }

        {
          key1 = keyCR.CreateSubKey("AppID", wowMode);
          key2 = key1.CreateSubKey(typeof(GraphDocumentEmbeddedComObject).GUID.ToString("B").ToUpperInvariant(), wowMode);
          key2.SetValue(null, GraphDocumentEmbeddedComObject.USER_TYPE);
          key2.SetValue("PreferredServerBitness", 3, RegistryValueKind.DWord);
        }

        {
          // register the Graph document embedded object (note that this is an Altaxo mini project)
          key1 = keyCR.CreateSubKey(GraphDocumentEmbeddedComObject.USER_TYPE, wowMode);
          key1.SetValue(null, GraphDocumentEmbeddedComObject.USER_TYPE_LONG);
          key2 = key1.CreateSubKey("CLSID");
          key2.SetValue(null, typeof(GraphDocumentEmbeddedComObject).GUID.ToString("B").ToUpperInvariant());
          key2 = key1.CreateSubKey("Insertable");
        }

        {
          // publish CLSID of file GraphDocumentEmbeddedObject and associate it with the application
          key1 = keyCLSID.CreateSubKey(typeof(GraphDocumentEmbeddedComObject).GUID.ToString("B").ToUpperInvariant(), wowMode);
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
          key4.SetValue(null, "14,9,64,1"); // EnhMetafile on ENHMF in get-direction

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
        if (key4 is not null)
          key4.Close();
        if (key3 is not null)
          key3.Close();
        if (key2 is not null)
          key2.Close();
        if (key1 is not null)
          key1.Close();
        if (keyCLSID is not null)
          keyCLSID.Close();
        if (keyCR is not null)
          keyCR.Close();
        if (keySW is not null)
          keySW.Close();
      }
    }

    // ProcessArguments() will process the command-line arguments
    // of this application.
    // If the return value is true, we carry
    // on and start this application.
    // If the return value is false, we terminate
    // this application immediately.
    public bool ProcessStartupArguments(params string[] args)
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
            ComDebug.ReportError("Unknown argument: " + args[0] + "\nValid are : -register, -unregister and -embedding");
            break;
        }
      }

      ComDebug.ReportInfo("{0}.ProcessArguments Embedding={1} IsRunning64bit={2}", GetType().Name, ApplicationWasStartedWithEmbeddingArg, System.Environment.Is64BitProcess);

      return bRet;
    }

    public void StartLocalServer()
    {
      IsActive = true;

      ComDebug.ReportInfo("Starting local server");

      // if we are in 64 bit mode, make sure that the PreferredServerBitness flag is set in the registry

      {
        // Register the FileComObject
        _classFactoryOfFileComObject = new ClassFactory_ProjectFileComObject(this)
        {
          ClassContext = (uint)(CLSCTX.CLSCTX_LOCAL_SERVER),
          ClassId = Marshal.GenerateGuidForType(typeof(ProjectFileComObject)),
          Flags = (uint)REGCLS.REGCLS_SINGLEUSE | (uint)REGCLS.REGCLS_SUSPENDED
        };
        _classFactoryOfFileComObject.RegisterClassObject();
        ComDebug.ReportInfo("{0}.StartLocalServer Registered: {1}", GetType().Name, _classFactoryOfFileComObject.GetType().Name);
      }

      if (ApplicationWasStartedWithEmbeddingArg)
      {
        // Register the SimpleCOMObjectClassFactory.
        _classFactoryOfDocumentComObject = new ClassFactory_GraphDocumentEmbeddedComObject(this)
        {
          ClassContext = (uint)(CLSCTX.CLSCTX_LOCAL_SERVER),
          ClassId = Marshal.GenerateGuidForType(typeof(GraphDocumentEmbeddedComObject)),
          Flags = (uint)REGCLS.REGCLS_SINGLEUSE | (uint)REGCLS.REGCLS_SUSPENDED
        };
        _classFactoryOfDocumentComObject.RegisterClassObject();
        ComDebug.ReportInfo("{0}.StartLocalServer Registered: {1}", GetType().Name, _classFactoryOfDocumentComObject.GetType().Name);
      }

      ClassFactoryBase.ResumeClassObjects();

      // Start up the garbage collection thread.
      _garbageCollector = new GarbageCollector(1000);
      var GarbageCollectionThread = new Thread(new ThreadStart(_garbageCollector.GCWatch))
      {
        // Set the name of the thread object.
        Name = "GarbCollThread",
        IsBackground = true
      };

      // Start the thread.
      GarbageCollectionThread.Start();
    }

    private void ConvertCurrentClipboardToPermanentDataObject()
    {
      if (IsInvokeRequiredForGuiThread())
        throw new ApplicationException("This function requires to be in the Gui thread");

      DataObjectBase lastUsedDataObject = null;
      // convert the clipboard data object to a permanent .NET data object
      if (_lastUsedDataObject is not null && (lastUsedDataObject = _lastUsedDataObject.Target as DataObjectBase) is not null)
      {
        lastUsedDataObject.ConvertToNetDataObjectAndPutToClipboard();
      }
      lastUsedDataObject = null;
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
      if (_classFactoryOfDocumentComObject is not null)
      {
        _classFactoryOfDocumentComObject.RevokeClassObject();
        ComDebug.ReportInfo("{0}.EnterLinkedObjectMode Revoked: {1}", GetType().Name, _classFactoryOfDocumentComObject.GetType().Name);
        _classFactoryOfDocumentComObject = null;
      }
    }

    public void EnterEmbeddedObjectMode()
    {
      if (_classFactoryOfFileComObject is not null)
      {
        _classFactoryOfFileComObject.RevokeClassObject();
        ComDebug.ReportInfo("{0}.EnterEmbeddedObjectMode Revoked: {1}", GetType().Name, _classFactoryOfFileComObject.GetType().Name);
        _classFactoryOfFileComObject = null;
      }
    }

    public void StopLocalServer()
    {
      // First of all, if a Com data object is still on the clipboard,
      // convert it to a normal (permanent) data object
      if (IsInvokeRequiredForGuiThread())
      {
        InvokeGuiThread(ConvertCurrentClipboardToPermanentDataObject);
      }
      else // we are running in the Gui thread
      {
        ConvertCurrentClipboardToPermanentDataObject();
      }

      // now stop the local server, make sure
      // that we call this function in a thread that is
      // not the Gui thread
      if (IsInvokeRequiredForGuiThread())
      {
        InternalStopLocalServer();
      }
      else // we are running in the Gui thread
      {
        FromGuiThreadExecute(InternalStopLocalServer);
      }
    }

    private void InternalStopLocalServer()
    {
      ComDebug.ReportInfo("Stop local server");

      if (_embeddedComObject is not null)
      {
        _embeddedComObject.Dispose();
        _embeddedComObject = null;
      }

      foreach (var co in _linkedDocumentsComObjects.Values)
      {
        co.Dispose();
      }
      _linkedDocumentsComObjects.Clear();

      if (_fileComObject is not null)
      {
        _fileComObject.Dispose();
        _fileComObject = null;
      }

      if (_classFactoryOfDocumentComObject is not null)
      {
        _classFactoryOfDocumentComObject.RevokeClassObject();
        ComDebug.ReportInfo("{0}.StopLocalServer:{1} Revoked.", GetType().Name, _classFactoryOfDocumentComObject.GetType().Name);
        _classFactoryOfDocumentComObject = null;
      }

      if (_classFactoryOfFileComObject is not null)
      {
        _classFactoryOfFileComObject.RevokeClassObject();
        ComDebug.ReportInfo("{0}.StopLocalServer:{1} Revoked.", GetType().Name, _classFactoryOfFileComObject.GetType().Name);
        _classFactoryOfFileComObject = null;
      }

      if (_garbageCollector is not null)
      {
        // Now stop the Garbage Collector thread.
        _garbageCollector.StopThread();
        _garbageCollector.WaitForThreadToStop();
        _garbageCollector = null;
        ComDebug.ReportInfo("StopLocalServer: GarbageCollector thread stopped.");
      }

      IsActive = false;
    }
  }
}
