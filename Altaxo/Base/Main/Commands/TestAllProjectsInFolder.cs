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

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;
			var from = obj as TestAllProjectsInFolderOptions;
			if (null != from)
			{
				this.FolderPaths = from.FolderPaths;
				this.TestSavingAndReopening = from.TestSavingAndReopening;
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
		private static void GetAltaxoProjectFileNames(System.IO.DirectoryInfo dir, List<string> list)
		{
			try
			{
				var subdirs = dir.GetDirectories();

				foreach (var subdir in subdirs)
					GetAltaxoProjectFileNames(subdir, list);
			}
			catch (Exception ex)
			{
				Current.Console.WriteLine("Warning: unable to enumerate subfolders in folder {0}: {1}", dir.FullName, ex.Message);
			}

			try
			{
				var files = dir.GetFiles("*.axoprj");
				foreach (var file in files)
					list.Add(file.FullName);
			}
			catch (Exception ex)
			{
				Current.Console.WriteLine("Warning: unable to enumerate Altaxo project files in folder {0}: {1}", dir.FullName, ex.Message);
			}
		}

		private static List<string> GetAltaxoProjectFileNames(string pathsSeparatedBySemicolon)
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
					Current.Console.WriteLine("Error: directory {0} does not exist", path);
					continue;
				}

				var dir = new System.IO.DirectoryInfo(path);
				GetAltaxoProjectFileNames(dir, list);
			}

			if (list.Count == 0)
			{
				Current.Console.WriteLine("Warning: no files found in {0}", pathsSeparatedBySemicolon);
			}
			return list;
		}

		public static void VerifyOpeningOfDocumentsWithoutException()
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
			Current.Gui.ShowBackgroundCancelDialog(10, monitor, () => InternalVerifyOpeningOfDocumentsWithoutException(testOptions, monitor));
		}

		public static void InternalVerifyOpeningOfDocumentsWithoutException(TestAllProjectsInFolderOptions testOptions, Altaxo.Main.Services.ExternalDrivenBackgroundMonitor monitor)
		{
			Current.Gui.Execute(Current.ProjectService.CloseProject, true);

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

				if (monitor.CancellationPending)
					break;

				try
				{
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
				catch (Exception ex)
				{
					++numberOfProjectsFailedToLoad;
					monitor.ReportProgress(string.Format(
						"Successfully loaded: {0}, failed to load: {1}, total: {2}/{3} projects.\r\n" +
						"Failed to load: {4}", numberOfProjectsTested - numberOfProjectsFailedToLoad, numberOfProjectsFailedToLoad, numberOfProjectsTested, totalFilesToTest, filename), numberOfProjectsTested / totalFilesToTest);
					Current.Console.WriteLine("Error opening file {0}", filename);
				}

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
						Current.Console.WriteLine("Error closing file (after saving) {0}; Message: {1}", filename, ex.Message);
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
						Current.Console.WriteLine("Error re-opening file {0}", filename);
					}

					// Close the project now
					try
					{
						Current.Gui.Execute(Current.ProjectService.CloseProject, true);
						System.Threading.Thread.Sleep(1000);
					}
					catch (Exception ex)
					{
						Current.Console.WriteLine("Error closing file (after re-opening it) {0}; Message: {1}", filename, ex.Message);
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
						Current.Console.WriteLine("Error closing file {0}; Message: {1}", filename, ex.Message);
						Current.Console.WriteLine("Operation will be stopped here because of error on closing");
						return;
					}
				}
			}

			Current.Console.WriteLine("End of test. {0} projects tested, {1} projects failed to load", numberOfProjectsTested, numberOfProjectsFailedToLoad);
		}
	}
}