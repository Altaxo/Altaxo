﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Altaxo.Main.Services;
using Altaxo.Main.Services.ExceptionHandling;

namespace Altaxo.Main.Commands
{
  public class TestAllProjectsInFolderOptions : Altaxo.Main.ICopyFrom
  {
    public string FolderPaths { get; set; }

    public bool TestSavingAndReopening { get; set; }

    public string ProtocolFileName { get; set; }

    public TestAllProjectsInFolderOptions()
    {
      FolderPaths = @"C:\Temp";
      TestSavingAndReopening = false;
      ProtocolFileName = @"C:\Temp\AltaxoTestOpeningLog.txt";
    }

    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;
      var from = obj as TestAllProjectsInFolderOptions;
      if (from is not null)
      {
        FolderPaths = from.FolderPaths;
        TestSavingAndReopening = from.TestSavingAndReopening;
        ProtocolFileName = from.ProtocolFileName;
        return true;
      }
      return false;
    }

    public object Clone()
    {
      var r = new TestAllProjectsInFolderOptions();
      r.CopyFrom(this);
      return r;
    }
  }

  public class TestAllProjectsInFolder
  {
    #region Internal class Reporter

    public class Reporter : Altaxo.Main.Services.TextOutputServiceBase
    {
      private System.IO.StreamWriter? _wr;
      private Altaxo.Main.Services.ITextOutputService _previousOutputService;

      public Reporter(TestAllProjectsInFolderOptions testOptions, Altaxo.Main.Services.ITextOutputService previousOutputService)
      {
        _previousOutputService = previousOutputService;

        if (!string.IsNullOrEmpty(testOptions.ProtocolFileName))
        {
          var stream = new System.IO.FileStream(testOptions.ProtocolFileName, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read);
          _wr = new System.IO.StreamWriter(stream, Encoding.UTF8);

          _wr.WriteLine();
          _wr.WriteLine("******************************************************************************");
          _wr.WriteLine("{0}: Test of Altaxo project files. Folder(s) to test: {1}, SaveAndReopeningProjects: {2}", DateTime.Now, testOptions.FolderPaths, testOptions.TestSavingAndReopening);
          _wr.WriteLine("------------------------------------------------------------------------------");
        }
      }

      public void Close()
      {
        _wr?.Close();
        _wr = null;
      }

      protected override void InternalWrite(string text)
      {
        if (_wr is not null)
        {
          _wr.Write(text);
          _wr.Flush();
        }

        _previousOutputService.Write(text);
      }
    }

    public class UnhandledExceptionHandler : IUnhandledExceptionHandler
    {
      private Reporter _reporter;

      public int NumberOfExceptionsEncountered { get; private set; }

      public UnhandledExceptionHandler(Reporter reporter)
      {
        _reporter = reporter;
      }

      public void EhCurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
        ++NumberOfExceptionsEncountered;
        _reporter.WriteLine($"Unhandled exception in current domain. IsTerminating: {e.IsTerminating}, Exception: {e.ExceptionObject}");
      }

      public void EhWindowsFormsApplication_ThreadException(object sender, ThreadExceptionEventArgs e)
      {
        ++NumberOfExceptionsEncountered;
        _reporter.WriteLine($"Unhandled exception in WindowsForms. Exception: {e.Exception}");
      }

      public void EhWpfDispatcher_UnhandledException(object sender, object dispatcher, Exception exception)
      {
        ++NumberOfExceptionsEncountered;
        _reporter.WriteLine($"Unhandled exception in Wpf. Exception: {exception}");
      }
    }

    #endregion Internal class Reporter

    private Reporter _reporter;

    private TestAllProjectsInFolderOptions _testOptions;

    private TestAllProjectsInFolder(Reporter reporter, TestAllProjectsInFolderOptions testOptions)
    {
      _reporter = reporter;
      _testOptions = testOptions;
    }

    private void GetAltaxoProjectFileNames(System.IO.DirectoryInfo dir, List<string> list)
    {
      try
      {
        var subdirs = dir.GetDirectories();

        foreach (var subdir in subdirs)
          GetAltaxoProjectFileNames(subdir, list);
      }
      catch (Exception ex)
      {
        _reporter.WriteLine("Warning: unable to enumerate subfolders in folder {0}: {1}", dir.FullName, ex.Message);
      }

      try
      {
        var files = dir.GetFiles("*.axoprj");
        foreach (var file in files)
          list.Add(file.FullName);
      }
      catch (Exception ex)
      {
        _reporter.WriteLine("Warning: unable to enumerate Altaxo project files in folder {0}: {1}", dir.FullName, ex.Message);
      }
    }

    private List<string> GetAltaxoProjectFileNames(string pathsSeparatedBySemicolon)
    {
      var list = new List<string>();

      var paths = pathsSeparatedBySemicolon.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

      for (int i = 0; i < paths.Length; ++i)
      {
        var path = paths[i].Trim();
        if (string.IsNullOrEmpty(path))
          continue;

        if (!System.IO.Directory.Exists(path))
        {
          _reporter.WriteLine("Error: directory {0} does not exist", path);
          continue;
        }

        var dir = new System.IO.DirectoryInfo(path);
        GetAltaxoProjectFileNames(dir, list);
      }

      if (list.Count == 0)
      {
        _reporter.WriteLine("Warning: no files found in {0}", pathsSeparatedBySemicolon);
      }
      return list;
    }

    public static void ShowDialogToVerifyOpeningOfDocumentsWithoutException()
    {
      if (Current.Project.IsDirty)
      {
        var e = new System.ComponentModel.CancelEventArgs();
        Current.IProjectService.AskForSavingOfProject(e);
        if (e.Cancel)
          return;
      }

      var testOptions = new TestAllProjectsInFolderOptions();
      if (!Current.Gui.ShowDialog(ref testOptions, "Test Altaxo project files on your disk", false))
        return;

      Current.Gui.ExecuteAsUserCancellable(10, (reporter) => InternalVerifyOpeningOfDocumentsWithoutExceptionStart(testOptions, reporter));
    }

    private static void InternalVerifyOpeningOfDocumentsWithoutExceptionStart(TestAllProjectsInFolderOptions testOptions, IProgressReporter monitor)
    {
      var reporter = new Reporter(testOptions, Current.Console);
      var oldOutputService = Current.GetRequiredService<Services.ITextOutputService>();
      Current.RemoveService<Services.ITextOutputService>();
      Current.AddService<Services.ITextOutputService>(reporter);
      if (!object.ReferenceEquals(Current.Console, reporter))
        throw new InvalidProgramException("Current console now should be the reporter! Please debug.");


      var test = new TestAllProjectsInFolder(reporter, testOptions);

      Current.IProjectService.ProjectChanged += test.EhProjectChanged;

      var unhandledExceptionHandler = new UnhandledExceptionHandler(reporter);
      Current.GetRequiredService<IUnhandledExceptionHandlerService>().AddHandler(unhandledExceptionHandler, true);

      try
      {
        test.InternalVerifyOpeningOfDocumentsWithoutException(testOptions, monitor, unhandledExceptionHandler, reporter);
      }
      catch (Exception ex)
      {
        reporter.WriteLine("Fatal error: Test has to close due to an unhandled exception. The exception details:");
        reporter.WriteLine(ex.ToString());
      }
      finally
      {
        Current.IProjectService.ProjectChanged += test.EhProjectChanged;

        reporter.WriteLine("----------------------- End of test ------------------------------------------");
        reporter.Close();

        Current.RemoveService<Services.ITextOutputService>();
        Current.AddService<Services.ITextOutputService>(oldOutputService);
        Current.GetRequiredService<IUnhandledExceptionHandlerService>().RemoveHandler(unhandledExceptionHandler);
      }
    }

    private void EhProjectChanged(object sender, ProjectEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.NewName) && e.ProjectEventKind != ProjectEventKind.ProjectDirtyChanged)
        _reporter.WriteLine("Project changed: Type: {0}; fileName: {1}", e.ProjectEventKind, e.NewName);
    }

    private void InternalVerifyOpeningOfDocumentsWithoutException(TestAllProjectsInFolderOptions testOptions, IProgressReporter monitor, UnhandledExceptionHandler unhandledExceptionHandler, Reporter reporter)
    {
      monitor.ReportProgress("Searching Altaxo project files ...", 0);
      var path = testOptions.FolderPaths;
      Current.Console.WriteLine("Begin of test. Search path(s): {0}", path);

      var filelist = GetAltaxoProjectFileNames(path);

      int numberOfProjectsTested = 0;
      int numberOfProjectsFailedToLoad = 0;

      double totalFilesToTest = filelist.Count;

      monitor.ReportProgress(string.Format("Searching done, {0} Altaxo project files found.", totalFilesToTest));

      foreach (var filename in filelist)
      {
        if (monitor.CancellationPending)
          break;

        try
        {
          System.Diagnostics.Debug.WriteLine(string.Format("Begin opening Altaxo project file {0}", filename));

          monitor.ReportProgress(string.Format(
            "Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
            "Currently opening: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);
          reporter.WriteLine($"---------- {filename} ----------");
          ++numberOfProjectsTested;
          var unhandledExceptionsBefore = unhandledExceptionHandler.NumberOfExceptionsEncountered;
          Current.Dispatcher.InvokeIfRequired(Current.IProjectService.OpenProject, new Services.FileName(filename), false);
          if (unhandledExceptionHandler.NumberOfExceptionsEncountered != unhandledExceptionsBefore)
            ++numberOfProjectsFailedToLoad;

          monitor.ReportProgress(string.Format(
            "Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
            "Loaded successfully: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);

          System.Threading.Thread.Sleep(1000);
        }
        catch (Exception)
        {
          ++numberOfProjectsFailedToLoad;
          monitor.ReportProgress(string.Format(
            "Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
            "Failed to load: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);
          Current.Console.WriteLine("Error opening file {0}", filename);
        }

        // Project is now opened from the original file

#if DEBUG && TRACEDOCUMENTNODES

				{
					GC.Collect();
					System.Threading.Thread.Sleep(500);
					bool areThereAnyProblems = false;
					areThereAnyProblems |= Main.SuspendableDocumentNodeBase.ReportNotConnectedDocumentNodes(false);
					areThereAnyProblems |= Main.SuspendableDocumentNode.ReportChildListProblems();
					areThereAnyProblems |= Main.SuspendableDocumentNode.ReportWrongChildParentRelations();

					if (areThereAnyProblems)
					{
						Current.Console.WriteLine("Above listed problems were detected after opening the file {0}", filename);
						Current.Console.WriteLine();
					}
				}
#endif

        if (testOptions.TestSavingAndReopening)
        {
          // Test saving of the project (now with the current version of Altaxo)
          var tempFileName = new FileName(System.IO.Path.GetTempFileName());
          try
          {
            monitor.ReportProgress(string.Format(
              "Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
              "Currently saving: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);

            var unhandledExceptionsBefore = unhandledExceptionHandler.NumberOfExceptionsEncountered;
            Current.Dispatcher.InvokeIfRequired(Current.IProjectService.SaveProject, tempFileName);
            if (unhandledExceptionHandler.NumberOfExceptionsEncountered != unhandledExceptionsBefore)
              ++numberOfProjectsFailedToLoad;

            monitor.ReportProgress(string.Format(
              "Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
              "Saved successfully: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);
          }
          catch (Exception)
          {
            ++numberOfProjectsFailedToLoad;
            monitor.ReportProgress(string.Format(
              "Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
              "Failed to save: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);
            Current.Console.WriteLine("Error saving file {0}", filename);
          }

          // Close the project now
          try
          {
            Current.Dispatcher.InvokeIfRequired(() => Current.IProjectService.CloseProject(true));
            System.Threading.Thread.Sleep(1000);
          }
          catch (Exception ex)
          {
            Current.Console.WriteLine("Error closing file (after saving) {0}; Message: {1}", filename, ex);
            Current.Console.WriteLine("Operation will be stopped here because of error on closing");
            return;
          }

          // Re-Open the project
          try
          {
            monitor.ReportProgress(string.Format(
              "Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
              "Currently re-opening: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);

            var unhandledExceptionsBefore = unhandledExceptionHandler.NumberOfExceptionsEncountered;
            Current.Dispatcher.InvokeIfRequired(Current.IProjectService.OpenProject, new Services.FileName(tempFileName), false);
            if (unhandledExceptionHandler.NumberOfExceptionsEncountered != unhandledExceptionsBefore)
              ++numberOfProjectsFailedToLoad;

            monitor.ReportProgress(string.Format(
              "Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
              "Re-opened successfully: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);

            System.Threading.Thread.Sleep(1000);
          }
          catch (Exception ex)
          {
            ++numberOfProjectsFailedToLoad;
            monitor.ReportProgress(string.Format(
              "Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
              "Failed to re-open: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);
            Current.Console.WriteLine("Error re-opening file {0}, Message: {1}", filename, ex);
          }

#if DEBUG && TRACEDOCUMENTNODES

					{
						GC.Collect();
						System.Threading.Thread.Sleep(500);
						bool areThereAnyProblems = false;
						areThereAnyProblems |= Main.SuspendableDocumentNodeBase.ReportNotConnectedDocumentNodes(false);
						areThereAnyProblems |= Main.SuspendableDocumentNode.ReportChildListProblems();
						areThereAnyProblems |= Main.SuspendableDocumentNode.ReportWrongChildParentRelations();

						if (areThereAnyProblems)
						{
							Current.Console.WriteLine("Above listed problems were detected after saving and reopening project {0}", filename);
							Current.Console.WriteLine();
						}
					}

#endif

          // Close the project now
          try
          {
            Current.Dispatcher.InvokeIfRequired(() => Current.IProjectService.CloseProject(true));
            System.Threading.Thread.Sleep(1000);
          }
          catch (Exception ex)
          {
            Current.Console.WriteLine("Error closing file (after re-opening it) {0}; Message: {1}", filename, ex);
            Current.Console.WriteLine("Operation will be stopped here because of error on closing");
            return;
          }

          // delete the temporary project
          try
          {
            System.IO.File.Delete(tempFileName);
          }
          catch (Exception ex)
          {
            Current.Console.WriteLine("Error deleting temporary Altaxo project file {0}, original from file {1}; Message: {2}", tempFileName, filename, ex.Message);
          }
        }
        else
        {
          try
          {
            Current.Dispatcher.InvokeIfRequired(() => Current.IProjectService.CloseProject(true));
            System.Threading.Thread.Sleep(1000);
          }
          catch (Exception ex)
          {
            Current.Console.WriteLine("Error closing file {0}; Message: {1}", filename, ex);
            Current.Console.WriteLine("Operation will be stopped here because of error on closing");
            return;
          }
        }
      }

      Current.Console.WriteLine("End of test. {0} projects tested, {1} projects failed to load", numberOfProjectsTested, numberOfProjectsFailedToLoad);
    }
  }
}
