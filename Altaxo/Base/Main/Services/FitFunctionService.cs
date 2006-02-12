#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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

    /// <summary>
    /// List of user defined functions. The key is the filename (without path)
    /// </summary>
    SortedList<string, FitFunctionInformation> _userDefinedFunctions;

    /// <summary>
    /// Directory where the user defined functions reside.
    /// </summary>
    string _fitFunctionDirectory;
    System.IO.FileSystemWatcher _fitFunctionDirectoryWatcher;
    Queue<string> _filesToProcess = new Queue<string>();
    volatile bool _threadIsWorking;
    
    
    /// <summary>
    /// Creates the fit function service. Starts a thread that will walk through all items in the fit function directory.
    /// </summary>
    public FitFunctionService()
    {
      _userDefinedFunctions = new SortedList<string, FitFunctionInformation>();
      _filesToProcess = new Queue<string>();

      ICSharpCode.Core.Services.PropertyService propserv = (ICSharpCode.Core.Services.PropertyService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ICSharpCode.Core.Services.PropertyService));

      _fitFunctionDirectory = System.IO.Path.Combine(propserv.ConfigDirectory, "FitFunctionScripts");

      if (!Directory.Exists(_fitFunctionDirectory))
        System.IO.Directory.CreateDirectory(_fitFunctionDirectory);

      if(Directory.Exists(_fitFunctionDirectory))
      {
        _fitFunctionDirectoryWatcher = new FileSystemWatcher(_fitFunctionDirectory,"*,xml");
        
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

    void AddFitFunctionEntry(string category, string name, DateTime creationTime, string fullfilename)
    {
      FitFunctionInformation info = new FitFunctionInformation(category, name, creationTime, fullfilename);

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
      
      while (_filesToProcess.Count>0)
      {
        string fullfilename = _filesToProcess.Dequeue();
        try
        {
          string category = null;
          string name = null;
          DateTime creationTime = DateTime.MinValue;


          System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader(fullfilename);
          xmlReader.MoveToContent();
          while (xmlReader.Read())
          {
            if (xmlReader.NodeType == System.Xml.XmlNodeType.Element && xmlReader.LocalName == "Category")
            {
              category = xmlReader.ReadElementString();
              name = xmlReader.ReadElementString("Name");
              creationTime = System.Xml.XmlConvert.ToDateTime(xmlReader.ReadElementString("CreationTime"),System.Xml.XmlDateTimeSerializationMode.Local);
              break;
            }
          }
          xmlReader.Close();

          AddFitFunctionEntry(category, name, creationTime, fullfilename);

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
    public FitFunctionInformation[] GetUserDefinedFitFunctions()
    {
      if (null == _userDefinedFunctions)
        return new FitFunctionInformation[] { };

      while (_threadIsWorking)
        System.Threading.Thread.Sleep(100);

      FitFunctionInformation[] result = new FitFunctionInformation[_userDefinedFunctions.Count];
      
      int i=0;
      foreach (FitFunctionInformation info in _userDefinedFunctions.Values)
        result[i++] = info;

      return result;
    }

    /// <summary>
    /// Saves a user defined fit function in the user's application directory. The user is prompted
    /// by a message box if the function already exists.
    /// </summary>
    /// <param name="doc">The fit function script to save.</param>
    /// <returns>True if the function is saved, otherwise (error or user action) returns false.</returns>
    public bool SaveUserDefinedFitFunction(Altaxo.Scripting.FitFunctionScript doc)
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

      AddFitFunctionEntry(doc.FitFunctionCategory, doc.FitFunctionName, doc.CreationTime, fullfilename);

      return true;
    }


    /// <summary>
    /// This reads a fit function, that is stored in xml format onto disc.
    /// </summary>
    /// <param name="info">The fit function information (only the file name is used from it).</param>
    /// <returns>The fit function, or null if the fit function could not be read.</returns>
    public static IFitFunction ReadFileBasedFitFunction(Main.Services.FitFunctionInformation info)
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


  } // end of class FitFunctionService

 

}
