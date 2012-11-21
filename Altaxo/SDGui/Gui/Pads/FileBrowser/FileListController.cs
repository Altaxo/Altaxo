#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.IO;

using Altaxo.Collections;
namespace Altaxo.Gui.Pads.FileBrowser
{
	public interface IFileListView
	{
		void Initialize_FileListColumnNames(ICollection<string> names);
		void Initialize_FileList(SelectableListNodeList files);

		/// <summary>
		/// Occurs when the user activates the selected items (either by double-clicking on it, or by pressing Enter).
		/// </summary>
		event Action SelectedItemsActivated;

	}

	public class FileListController
	{

		#region FileItem

		class FileListItem : SelectableListNode
		{
			string _text1;
			string _text2;
			public string FullName { get { return (string)Tag; } }

			public FileListItem(string fullPath)
				: base(Path.GetFileName(fullPath),fullPath,false)
			{
				InternalUpdate(false);
			}

			public void Rename(string fullPath)
			{
				Tag = fullPath;
				Text = Path.GetFileName(fullPath);
				InternalUpdate(true);
			}

			public void Update()
			{
				InternalUpdate(true);
			}

			void InternalUpdate(bool triggerEvents)
			{
				FileInfo info = new FileInfo(FullName);

				try
				{
					_text1 = Math.Round((double)info.Length / 1024).ToString() + " KB";
					_text2 = info.LastWriteTime.ToString();
				}
				catch (IOException)
				{
					// ignore IO errors
				}
				if (triggerEvents)
				{
					OnPropertyChanged("Text1");
					OnPropertyChanged("Text2");
				}
			}



			public override string Text1
			{
				get
				{
					return _text1;
				}
			}

			public override string Text2
			{
				get
				{
					return _text2;
				}
			}

			public void SetText1(string value)
			{
				_text1 = value;
				OnPropertyChanged("Text1");
			}

				public void SetText2(string value)
			{
				_text2 = value;
				OnPropertyChanged("Text2");
			}

		}

		#endregion

		IFileListView _view;
		private FileSystemWatcher watcher;
		List<string> _columnNames;

		SelectableListNodeList _fileList = new SelectableListNodeList();

		public FileListController()
		{
			Initialize(true);
		}

		void Initialize(bool initData)
		{

			if (initData)
			{
				_columnNames = new List<string>();
				_columnNames.Add(Current.ResourceService.GetString("CompilerResultView.FileText"));
				_columnNames.Add(Current.ResourceService.GetString("MainWindow.Windows.FileScout.Size"));
				_columnNames.Add(Current.ResourceService.GetString("MainWindow.Windows.FileScout.LastModified"));


				try
				{
					watcher = new FileSystemWatcher();
				}
				catch { }

				if (watcher != null)
				{
					watcher.NotifyFilter = NotifyFilters.FileName;
					watcher.EnableRaisingEvents = false;

					watcher.Renamed += new RenamedEventHandler(fileRenamed);
					watcher.Deleted += new FileSystemEventHandler(fileDeleted);
					watcher.Created += new FileSystemEventHandler(fileCreated);
					watcher.Changed += new FileSystemEventHandler(fileChanged);
				}
			}

			if (null != _view)
			{
				_view.Initialize_FileListColumnNames(_columnNames);
				_view.Initialize_FileList(_fileList);
			}
		}

		#region FileSystemWatcher handlers

		void fileDeleted(object sender, FileSystemEventArgs e)
		{
			Action method = delegate
			{
				for(int i=_fileList.Count-1;i>=0;--i)
				{
					var fileItem = (FileListItem)_fileList[i];
					if (fileItem.FullName.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase))
					{
						_fileList.Remove(fileItem);
						break;
					}
				}
			};

			Current.Gui.Execute(method);
		}

		void fileChanged(object sender, FileSystemEventArgs e)
		{
			Action method = delegate
			{
				foreach (FileListItem fileItem in _fileList)
				{
					if (fileItem.FullName.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase))
					{

						fileItem.Update();
						break;
					}
				}
			};
			Current.Gui.Execute(method);
		}

		void fileCreated(object sender, FileSystemEventArgs e)
		{
			Action method = delegate
			{
				FileInfo info = new FileInfo(e.FullPath);
				_fileList.Add(new FileListItem(e.FullPath));
			};

			Current.Gui.Execute(method);
		}

		void fileRenamed(object sender, RenamedEventArgs e)
		{
			Action method = delegate
			{
				foreach (FileListItem fileItem in _fileList)
				{
					if (fileItem.FullName.Equals(e.OldFullPath, StringComparison.OrdinalIgnoreCase))
					{
						fileItem.Rename(e.FullPath);
						break;
					}
				}
			};

			Current.Gui.Execute(method);
		}

		#endregion

		#region User handlers

		public void EhView_ActivateSelectedItems()
		{
			foreach (FileListItem item in _fileList.Where(x=>x.IsSelected))
			{
				switch (Path.GetExtension(item.FullName).ToLower())
				{
					case ".axoprj":
					case ".axoprz":
						Current.ProjectService.OpenProject(item.FullName, false);
						break;
					case ".spc":
						if (Current.Workbench.ActiveViewContent is Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)
						{
							var ctrl = (Altaxo.Gui.Worksheet.Viewing.WorksheetController)Current.Workbench.ActiveViewContent;
							string[] files = new string[] { item.FullName };
							Altaxo.Serialization.Galactic.Import.ImportSpcFiles(files, ctrl.DataTable);
						}
						break;
					case ".dat":
					case ".txt":
					case ".csv":
						{
							Altaxo.Data.FileCommands.ImportAsciiToMultipleWorksheets(
								null,
								new string[] { item.FullName }, null);
						}
						break;
					default:
						ICSharpCode.SharpDevelop.FileService.OpenFile(item.FullName);
						break;
				}
			}
		}

		public void ShowFilesInPath(string path)
		{
			string[] files;
			_fileList.Clear();

			try
			{
				if (Directory.Exists(path))
				{
					files = Directory.GetFiles(path);
				}
				else
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}

			watcher.Path = path;
			watcher.EnableRaisingEvents = true;

			foreach (string file in files)
			{
				_fileList.Add(new FileListItem(file));
			}
		}

		#endregion


		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					_view.SelectedItemsActivated -= this.EhView_ActivateSelectedItems;
				}

				_view = value as IFileListView;

				if (null != _view)
				{
					Initialize(false);
					_view.SelectedItemsActivated += this.EhView_ActivateSelectedItems;
				}
			}
		}
	}
}
