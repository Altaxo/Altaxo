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

using Altaxo.Collections;
using Altaxo.Serialization.Ascii;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Worksheet
{
	public interface IImportAsciiDataSourceView
	{
		void SetAsciiImportOptionsControl(object p);

		void SetImportOptionsControl(object p);

		SelectableListNodeList FileNames { set; }

		event Action BrowseSelectedFileName;

		event Action DeleteSelectedFileName;

		event Action MoveUpSelectedFileName;

		event Action MoveDownSelectedFileName;

		event Action AddNewFileName;

		event Action SortFileNamesAscending;
	}

	[ExpectedTypeOfView(typeof(IImportAsciiDataSourceView))]
	[UserControllerForObject(typeof(AsciiImportDataSource))]
	public class AsciiImportDataSourceController : MVCANControllerEditOriginalDocBase<AsciiImportDataSource, IImportAsciiDataSourceView>, IMVCSupportsApplyCallback
	{
		private IMVCANController _dataSourceOptionsController;
		private IMVCANController _importAsciiOptionsController;
		private SelectableListNodeList _fileNames;

		public event Action SuccessfullyApplied;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_dataSourceOptionsController, () => _dataSourceOptionsController = null);
			yield return new ControllerAndSetNullMethod(_importAsciiOptionsController, () => _importAsciiOptionsController = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				//_doc.SourceFileName

				_dataSourceOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);
				_importAsciiOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.AsciiImportOptions, new AsciiImportOptionsAnalysisDataProvider(this) }, typeof(IMVCANController), UseDocument.Directly);
				_fileNames = new SelectableListNodeList();
				foreach (var files in _doc.SourceFileNames)
				{
					_fileNames.Add(new SelectableListNode(files, files, false));
				}
			}

			if (null != _view)
			{
				_view.SetImportOptionsControl(_dataSourceOptionsController.ViewObject);
				_view.SetAsciiImportOptionsControl(_importAsciiOptionsController.ViewObject);
				_view.FileNames = _fileNames;
			}
		}

		public override bool Apply(bool disposeController)
		{
			bool result;

			result = _dataSourceOptionsController.Apply(disposeController);
			if (!result)
				return result;
			else
				_doc.ImportOptions = (Altaxo.Data.IDataSourceImportOptions)_dataSourceOptionsController.ModelObject;

			result = _importAsciiOptionsController.Apply(disposeController);
			if (!result)
				return result;
			else
				_doc.AsciiImportOptions = (AsciiImportOptions)_importAsciiOptionsController.ModelObject; // AsciiImportOptions is cloned in property set

			_doc.SourceFileNames = _fileNames.Select(x => (string)x.Tag);

			var ev = SuccessfullyApplied;
			if (null != ev)
			{
				ev();
			}

			return ApplyEnd(true, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.BrowseSelectedFileName += EhBrowseFileName;
			_view.DeleteSelectedFileName += EhDeleteFileName;
			_view.MoveUpSelectedFileName += EhMoveUpFileName;
			_view.MoveDownSelectedFileName += EhMoveDownFileName;
			_view.AddNewFileName += EhAddNewFileName;
			_view.SortFileNamesAscending += EhSortFileNamesAscending;
		}

		protected override void DetachView()
		{
			_view.BrowseSelectedFileName -= EhBrowseFileName;
			_view.DeleteSelectedFileName -= EhDeleteFileName;
			_view.MoveUpSelectedFileName -= EhMoveUpFileName;
			_view.MoveDownSelectedFileName -= EhMoveDownFileName;
			_view.AddNewFileName -= EhAddNewFileName;
			_view.SortFileNamesAscending -= EhSortFileNamesAscending;

			base.DetachView();
		}

		private class AsciiImportOptionsAnalysisDataProvider : Altaxo.Gui.Serialization.Ascii.IAsciiImportOptionsAnalysisDataProvider
		{
			private AsciiImportDataSourceController _parent;

			internal AsciiImportOptionsAnalysisDataProvider(AsciiImportDataSourceController parent)
			{
				_parent = parent;
			}

			public System.IO.Stream GetStreamForAnalysis()
			{
				try
				{
					var str = AsciiImporter.GetAsciiInputFileStream(_parent._doc.SourceFileName);
					return str;
				}
				catch (Exception)
				{
				}
				return null;
			}
		}

		/// <summary>
		/// Gets a file stream of the first file for analysis purposes. Returns null without throwing an exception if the file is not available or could not be opened.
		/// </summary>
		/// <returns></returns>
		private System.IO.Stream GetFileStreamForAnalysis()
		{
			try
			{
				var str = AsciiImporter.GetAsciiInputFileStream(_doc.SourceFileName);
				return str;
			}
			catch (Exception)
			{
			}
			return null;
		}

		private void EhDeleteFileName()
		{
			_fileNames.RemoveSelectedItems();
		}

		private void EhMoveUpFileName()
		{
			_fileNames.MoveSelectedItemsUp();
			_view.FileNames = _fileNames;
		}

		private void EhMoveDownFileName()
		{
			_fileNames.MoveSelectedItemsDown();
			_view.FileNames = _fileNames;
		}

		private void EhBrowseFileName()
		{
			var node = _fileNames.FirstSelectedNode;
			if (null == node)
				return;

			var options = new OpenFileOptions();
			options.AddFilter("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)");
			options.AddFilter("*.*", "All files (*.*)");
			options.InitialDirectory = System.IO.Path.GetDirectoryName((string)node.Tag);

			if (Current.Gui.ShowOpenFileDialog(options))
			{
				node.Tag = node.Text = options.FileName;
			}
		}

		private void EhAddNewFileName()
		{
			var node = _fileNames.Count > 0 ? _fileNames[_fileNames.Count - 1] : null;
			var options = new OpenFileOptions();
			options.AddFilter("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)");
			options.AddFilter("*.*", "All files (*.*)");
			if (null != node)
				options.InitialDirectory = System.IO.Path.GetDirectoryName((string)node.Tag);
			options.Multiselect = true;
			if (Current.Gui.ShowOpenFileDialog(options))
			{
				foreach (var filename in options.FileNames)
					_fileNames.Add(new SelectableListNode(filename, filename, false));
			}
		}

		private void EhSortFileNamesAscending()
		{
			var listOfNamesSorted = new List<string>(_fileNames.OrderBy(x => (string)x.Tag).Select(x => (string)x.Tag));
			_fileNames.Clear();
			foreach (var name in listOfNamesSorted)
				_fileNames.Add(new SelectableListNode(name, name, false));
		}
	}
}