#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Handles the storage and retrieving of user defined fit function scripts.
  /// </summary>
  public class FitFunctionService : Altaxo.Main.Services.IFitFunctionService
  {
    FileBasedFitFunctionService _userFunctionService;
    FileBasedFitFunctionService _applicationFunctionService;
    BuiltinFitFunctionService _builtinFunctionService;

    public FitFunctionService()
    {
     
     // ICSharpCode.Core.Services.PropertyService propserv = (ICSharpCode.Core.Services.PropertyService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ICSharpCode.Core.Services.PropertyService));
      string userFitFunctionDirectory = System.IO.Path.Combine(ICSharpCode.Core.PropertyService.ConfigDirectory, "FitFunctionScripts");
      _userFunctionService = new FileBasedFitFunctionService(userFitFunctionDirectory);

      string appdir = System.Configuration.ConfigurationManager.AppSettings.Get("ApplicationFitFunctionDirectory");
     
      _applicationFunctionService = new FileBasedFitFunctionService(appdir, true);

      _builtinFunctionService = new BuiltinFitFunctionService();
    }

    /// <summary>
    /// This will get all user defined fit functions.
    /// </summary>
    /// <returns>Array of information about the user defined fit functions.</returns>
    public FileBasedFitFunctionInformation[] GetUserDefinedFitFunctions()
    {
      return _userFunctionService.GetFitFunctions();
    }

    /// <summary>
    /// Saves a user defined fit function in the user's application directory. The user is prompted
    /// by a message box if the function already exists.
    /// </summary>
    /// <param name="doc">The fit function script to save.</param>
    /// <returns>True if the function is saved, otherwise (error or user action) returns false.</returns>
    public bool SaveUserDefinedFitFunction(Altaxo.Scripting.FitFunctionScript doc)
    {
      return _userFunctionService.SaveFitFunction(doc);
    }

    /// <summary>
    /// This will get all application wide defined fit functions.
    /// </summary>
    /// <returns></returns>
    public FileBasedFitFunctionInformation[] GetApplicationFitFunctions()
    {
      return _applicationFunctionService.GetFitFunctions();
    }

    /// <summary>
    /// This will get all builtin fit functions.
    /// </summary>
    /// <returns>Array of information about the builtin fit functions.</returns>
    public BuiltinFitFunctionInformation[] GetBuiltinFitFunctions()
    {
      return _builtinFunctionService.GetFitFunctions();
    }

    /// <summary>
    /// This will get all current document fit functions.
    /// </summary>
    /// <returns>Array of document fit function informations.</returns>
    public DocumentFitFunctionInformation[] GetDocumentFitFunctions()
    {
      ArrayList arr = new ArrayList();
      foreach (Altaxo.Scripting.FitFunctionScript func in Current.Project.FitFunctionScripts)
      {
        arr.Add(new DocumentFitFunctionInformation(func));
      }

      return (DocumentFitFunctionInformation[])arr.ToArray(typeof(DocumentFitFunctionInformation));
    }

    /// <summary>
    /// This reads a fit function, that is stored in xml format onto disc.
    /// </summary>
    /// <param name="info">The fit function information (only the file name is used from it).</param>
    /// <returns>The fit function, or null if the fit function could not be read.</returns>
    public static IFitFunction ReadUserDefinedFitFunction(Main.Services.FileBasedFitFunctionInformation info)
    {
      return FileBasedFitFunctionService.ReadFileBasedFitFunction(info);
    }

    /// <summary>
    /// This removes a fit function that is stored in xml format onto disc.
    /// </summary>
    /// <param name="info">The fit function information (only the file name is used from it).</param>
    public void RemoveUserDefinedFitFunction(Main.Services.FileBasedFitFunctionInformation info)
    {
      _userFunctionService.RemoveFileBasedFitFunction(info);
    }

    #region Inner classes

    class BuiltinFitFunctionService
    {
      BuiltinFitFunctionInformation[] _fitFunctions;

      void Initialize()
      {
        IEnumerable<Type> classentries = Altaxo.Main.Services.ReflectionService.GetUnsortedClassTypesHavingAttribute(typeof(FitFunctionClassAttribute),true);

        SortedList<FitFunctionCreatorAttribute,System.Reflection.MethodInfo> list = new SortedList<FitFunctionCreatorAttribute,System.Reflection.MethodInfo>();

        foreach (Type definedtype in classentries)
        {
          System.Reflection.MethodInfo[] methods = definedtype.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
          foreach (System.Reflection.MethodInfo method in methods)
          {
            if (method.IsStatic && method.ReturnType != typeof(void) && method.GetParameters().Length == 0)
            {
              object[] attribs = method.GetCustomAttributes(typeof(FitFunctionCreatorAttribute), false);
              foreach (FitFunctionCreatorAttribute creatorattrib in attribs)
                list.Add(creatorattrib, method);
            }
          }
        }


        _fitFunctions = new BuiltinFitFunctionInformation[list.Count];
        int j = 0;
        foreach (KeyValuePair<FitFunctionCreatorAttribute,System.Reflection.MethodInfo> entry in list)
        {
          _fitFunctions[j++] = new BuiltinFitFunctionInformation(entry.Key, entry.Value); 
        }


      }

      /// <summary>
      /// This will get all builtin fit functions.
      /// </summary>
      /// <returns></returns>
      public BuiltinFitFunctionInformation[] GetFitFunctions()
      {
        if (_fitFunctions == null)
          Initialize();

        return _fitFunctions;
      }
    }
    

    /// <summary>
    /// Handles the storage and retrieving of user defined fit function scripts.
    /// </summary>
    class FileBasedFitFunctionService
    {

      /// <summary>
      /// List of user defined functions. The key is the filename (without path)
      /// </summary>
      SortedList<string, FileBasedFitFunctionInformation> _userDefinedFunctions;

      /// <summary>
      /// Directory where the user defined functions reside.
      /// </summary>
      string _fitFunctionDirectory;
      System.IO.FileSystemWatcher _fitFunctionDirectoryWatcher;
      Queue<string> _filesToProcess = new Queue<string>();
      volatile bool _threadIsWorking;

      
      /// <summary>
      /// Creates the fit function file service (read/write). Starts a thread that will walk through all items in the fit function directory.
      /// </summary>
      public FileBasedFitFunctionService(string fitFunctionDirectory)
        : this(fitFunctionDirectory,false)
      {
      }

      /// <summary>
      /// Creates the fit function service. Starts a thread that will walk through all items in the fit function directory.
      /// </summary>
      /// <param name="fitFunctionDirectory">Directory where the fit functions reside.</param>
      /// <param name="isReadOnly">If true, only read access to that directory is allowed.</param>
      public FileBasedFitFunctionService(string fitFunctionDirectory, bool isReadOnly)
      {
        _userDefinedFunctions = new SortedList<string, FileBasedFitFunctionInformation>();
        _filesToProcess = new Queue<string>();
        _fitFunctionDirectory = fitFunctionDirectory;

        if (!Directory.Exists(_fitFunctionDirectory) && !isReadOnly )
          System.IO.Directory.CreateDirectory(_fitFunctionDirectory);

        if (Directory.Exists(_fitFunctionDirectory))
        {
          _fitFunctionDirectoryWatcher = new FileSystemWatcher(_fitFunctionDirectory, "*,xml");

          _fitFunctionDirectoryWatcher.Changed += new FileSystemEventHandler(EhChanged);
          _fitFunctionDirectoryWatcher.Created += new FileSystemEventHandler(EhChanged);
          _fitFunctionDirectoryWatcher.Deleted += new FileSystemEventHandler(EhDeleted);
          _fitFunctionDirectoryWatcher.Renamed += new RenamedEventHandler(EhRenamed);

          // Begin watching.
          _fitFunctionDirectoryWatcher.EnableRaisingEvents = true;

          // for the first, enqueue all files in this directory for processing
          string[] names = Directory.GetFiles(_fitFunctionDirectory, "*.xml", SearchOption.TopDirectoryOnly);
          foreach (string name in names)
            _filesToProcess.Enqueue(name);

          // Start the thread
          _threadIsWorking = true;
          System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(this.ProcessFiles));
        }
      }

      void EhChanged(object sender, FileSystemEventArgs e)
      {
        _filesToProcess.Enqueue(e.FullPath);
        if (!_threadIsWorking)
          ProcessFiles(null);
      }
      void EhDeleted(object sender, FileSystemEventArgs e)
      {
        RemoveFitFunctionEntry(e.FullPath);
      }
      void EhRenamed(object sender, RenamedEventArgs e)
      {
        RemoveFitFunctionEntry(e.OldFullPath);
        _filesToProcess.Enqueue(e.FullPath);
        if (!_threadIsWorking)
          ProcessFiles(null);
      }




      /// <summary>
      /// Removes an entry with the same filename like the argument.
      /// </summary>
      /// <param name="fullfilename">The filename to delete (full form).</param>
      void RemoveFitFunctionEntry(string fullfilename)
      {
        string filename = System.IO.Path.GetFileName(fullfilename);
        if (_userDefinedFunctions.ContainsKey(filename))
          _userDefinedFunctions.Remove(filename);
      }

      void AddFitFunctionEntry(string category, string name, DateTime creationTime, string description, string fullfilename)
      {
        FileBasedFitFunctionInformation info = new FileBasedFitFunctionInformation(category, name, creationTime, description, fullfilename);

        string filename = System.IO.Path.GetFileName(fullfilename);


        if (_userDefinedFunctions.ContainsKey(filename))
          _userDefinedFunctions.Remove(filename);

        _userDefinedFunctions.Add(filename, info);
      }

      /// <summary>
      /// This is the worker thread.
      /// </summary>
      /// <param name="stateInfo">Not used.</param>
      void ProcessFiles(object stateInfo)
      {

        System.Text.StringBuilder stb = null;

        while (_filesToProcess.Count > 0)
        {
          string fullfilename = _filesToProcess.Dequeue();
          try
          {
            string category = null;
            string name = null;
            DateTime creationTime = DateTime.MinValue;
            string description = string.Empty;

            System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(fullfilename);
            xmlReader.MoveToContent();
            while (xmlReader.Read())
            {
              if (xmlReader.NodeType == System.Xml.XmlNodeType.Element && xmlReader.LocalName == "Category")
              {
                category = xmlReader.ReadElementString();
                name = xmlReader.ReadElementString("Name");
                creationTime = System.Xml.XmlConvert.ToDateTime(xmlReader.ReadElementString("CreationTime"), System.Xml.XmlDateTimeSerializationMode.Local);
                if (xmlReader.LocalName == "Description")
                  description = xmlReader.ReadElementString("Description");
                break;
              }
            }
            xmlReader.Close();

            AddFitFunctionEntry(category, name, creationTime, description, fullfilename);

          }
          catch (Exception ex)
          {
            if (stb == null)
              stb = new StringBuilder();

            stb.AppendLine(ex.ToString());
          }
        }

        if (stb != null)
        {
          Current.Console.WriteLine("Exception(s) thrown in " + this.GetType().ToString() + " during parsing of fit functions, details will follow:");
          Current.Console.WriteLine(stb.ToString());
        }
        _threadIsWorking = false;
      }


      /// <summary>
      /// This will get all user defined fit functions.
      /// </summary>
      /// <returns></returns>
      public FileBasedFitFunctionInformation[] GetFitFunctions()
      {
        if (null == _userDefinedFunctions)
          return new FileBasedFitFunctionInformation[] { };

        while (_threadIsWorking)
          System.Threading.Thread.Sleep(100);

        FileBasedFitFunctionInformation[] result = new FileBasedFitFunctionInformation[_userDefinedFunctions.Count];

        int i = 0;
        foreach (FileBasedFitFunctionInformation info in _userDefinedFunctions.Values)
          result[i++] = info;

        return result;
      }

      /// <summary>
      /// Saves a user defined fit function in the user's application directory. The user is prompted
      /// by a message box if the function already exists.
      /// </summary>
      /// <param name="doc">The fit function script to save.</param>
      /// <returns>True if the function is saved, otherwise (error or user action) returns false.</returns>
      public bool SaveFitFunction(Altaxo.Scripting.FitFunctionScript doc)
      {
        if (doc.ScriptObject == null)
        {
          Current.Gui.ErrorMessageBox("Only a successfully compiled fit function can be saved in the user fit function directory!");
          return false;
        }



        string filename = Altaxo.Serialization.FileIOHelper.GetValidFileName(doc.FitFunctionCategory + "-" + doc.FitFunctionName + ".xml");
        string fullfilename = System.IO.Path.Combine(this._fitFunctionDirectory, filename);

        if (System.IO.File.Exists(fullfilename))
        {
          if (!Current.Gui.YesNoMessageBox(string.Format("The file {0} already exists. Do you really want to overwrite the file?", filename), "Overwrite?", false))
            return false; // Cancel the end of dialog
        }

        System.IO.Stream stream = new System.IO.FileStream(fullfilename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
        info.BeginWriting(stream);
        info.AddValue("FitFunctionScript", doc);
        info.EndWriting();
        stream.Close();

        AddFitFunctionEntry(doc.FitFunctionCategory, doc.FitFunctionName, doc.CreationTime, doc.FitFunctionDescription, fullfilename);

        return true;
      }


      /// <summary>
      /// This reads a fit function, that is stored in xml format onto disc.
      /// </summary>
      /// <param name="info">The fit function information (only the file name is used from it).</param>
      /// <returns>The fit function, or null if the fit function could not be read.</returns>
      public static IFitFunction ReadFileBasedFitFunction(Main.Services.FileBasedFitFunctionInformation info)
      {
        IFitFunction func = null;
        try
        {
          Altaxo.Serialization.Xml.XmlStreamDeserializationInfo str = new Altaxo.Serialization.Xml.XmlStreamDeserializationInfo();
          str.BeginReading(new FileStream(info.FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
          func = (IFitFunction)str.GetValue(null);
          str.EndReading();
          return func;
        }
        catch (Exception ex)
        {
          Current.Console.WriteLine("Error reading fit function from file {0}, error details: {1}", info.FileName, ex.ToString());
        }
        return null;
      }


      /// <summary>
      /// This removes a fit function that is stored in xml format onto disc.
      /// </summary>
      /// <param name="info">The fit function information (only the file name is used from it).</param>
      public void RemoveFileBasedFitFunction(Main.Services.FileBasedFitFunctionInformation info)
      {
        

        try
        {
          System.IO.File.Delete(info.FileName);
          this.RemoveFitFunctionEntry(info.FileName);
        }
        catch (Exception ex)
        {
          Current.Console.WriteLine("Error deleting fit function file {0}, error details: {1}", info.FileName, ex.ToString());
        }
        
      }


    } // end of class FitFunctionService
    #endregion

  }

}
