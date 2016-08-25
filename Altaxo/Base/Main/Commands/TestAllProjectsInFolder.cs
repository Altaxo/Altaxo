#region Copyright

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
using System.Linq;
using System.Text;

namespace Altaxo.Main.Commands
{
	public class TestAllProjectsInFolderOptions : Altaxo.Main.ICopyFrom
	{
		public string FolderPaths { get; set; }

		public bool TestSavingAndReopening { get; set; }

		public string ProtocolFileName { get; set; }

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as TestAllProjectsInFolderOptions;
			if (null != from)
			{
				this.FolderPaths = from.FolderPaths;
				this.TestSavingAndReopening = from.TestSavingAndReopening;
				this.ProtocolFileName = from.ProtocolFileName;
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

		public class Reporter : Altaxo.Main.Services.OutputServiceBase
		{
			private System.IO.StreamWriter _wr;
			private Altaxo.Main.Services.IOutputService _previousOutputService;

			public Reporter(TestAllProjectsInFolderOptions testOptions, Altaxo.Main.Services.IOutputService previousOutputService)
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
				if (null != _wr)
				{
					_wr.Write(text);
					_wr.Flush();
				}

				_previousOutputService.Write(text);
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
				Current.ProjectService.AskForSavingOfProject(e);
				if (e.Cancel)
					return;
			}

			var testOptions = new TestAllProjectsInFolderOptions();
			if (!Current.Gui.ShowDialog(ref testOptions, "Test Altaxo project files on your disk", false))
				return;

			var monitor = new Altaxo.Main.Services.ExternalDrivenBackgroundMonitor();
			Current.Gui.ShowBackgroundCancelDialog(10, monitor, () => InternalVerifyOpeningOfDocumentsWithoutExceptionStart(testOptions, monitor));
		}

		private static void InternalVerifyOpeningOfDocumentsWithoutExceptionStart(TestAllProjectsInFolderOptions testOptions, Altaxo.Main.Services.ExternalDrivenBackgroundMonitor monitor)
		{
			var reporter = new Reporter(testOptions, Current.Console);
			var oldOutputService = Current.SetOutputService(reporter);

			var test = new TestAllProjectsInFolder(reporter, testOptions);

			Current.ProjectService.ProjectChanged += test.EhProjectChanged;

			try
			{
				test.InternalVerifyOpeningOfDocumentsWithoutException(testOptions, monitor);
			}
			catch (Exception ex)
			{
				reporter.WriteLine("Fatal error: Test has to close due to an unhandled exception. The exception details:");
				reporter.WriteLine(ex.ToString());
			}
			finally
			{
				Current.ProjectService.ProjectChanged += test.EhProjectChanged;

				reporter.WriteLine("----------------------- End of test ------------------------------------------");
				reporter.Close();
				Current.SetOutputService(oldOutputService);
			}
		}

		private void EhProjectChanged(object sender, ProjectEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.NewName) && e.ProjectEventKind != ProjectEventKind.ProjectDirtyChanged)
				_reporter.WriteLine("Project changed: Type: {0}; fileName: {1}", e.ProjectEventKind, e.NewName);
		}

		private void InternalVerifyOpeningOfDocumentsWithoutException(TestAllProjectsInFolderOptions testOptions, Altaxo.Main.Services.ExternalDrivenBackgroundMonitor monitor)
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

					++numberOfProjectsTested;
					Current.Gui.Execute(Current.ProjectService.OpenProject, filename, true);

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
					string tempFileName = System.IO.Path.GetTempFileName();
					try
					{
						monitor.ReportProgress(string.Format(
							"Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
							"Currently saving: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);

						Current.Gui.Execute(Current.ProjectService.SaveProject, tempFileName);

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
						Current.Gui.Execute(Current.ProjectService.CloseProject, true);
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

						Current.Gui.Execute(Current.ProjectService.OpenProject, tempFileName, true);

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
						Current.Gui.Execute(Current.ProjectService.CloseProject, true);
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
						Current.Gui.Execute(Current.ProjectService.CloseProject, true);
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