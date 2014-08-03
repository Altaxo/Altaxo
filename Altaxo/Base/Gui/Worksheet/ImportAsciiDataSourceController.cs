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

using Altaxo.Serialization.Ascii;
using Altaxo.Worksheet.Commands.Analysis;
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

		string FileName { get; set; }

		event Action BrowseFileName;
	}

	[ExpectedTypeOfView(typeof(IImportAsciiDataSourceView))]
	[UserControllerForObject(typeof(AsciiImportDataSource))]
	public class RealFourierTransformation2DDataSourceController : MVCANControllerBase<AsciiImportDataSource, IImportAsciiDataSourceView>, IMVCSupportsApplyCallback
	{
		private IMVCANController _dataSourceOptionsController;
		private IMVCANController _importAsciiOptionsController;
		private IMVCANController _inputDataController;

		public event Action SuccessfullyApplied;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_dataSourceOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);
				_importAsciiOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.AsciiImportOptions }, typeof(IMVCANController), UseDocument.Directly);
				//		_inputDataController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc..InputData }, typeof(IMVCANController), UseDocument.Directly);
			}

			if (null != _view)
			{
				_view.SetImportOptionsControl(_dataSourceOptionsController.ViewObject);
				_view.SetAsciiImportOptionsControl(_importAsciiOptionsController.ViewObject);
				if (null != _inputDataController)
				{
					//_view.SetInputDataControl(_inputDataController.ViewObject);
				}
				_view.FileName = _doc.SourceFileName;
			}
		}

		public override bool Apply()
		{
			bool result;

			result = _dataSourceOptionsController.Apply();
			if (!result) return result;

			result = _importAsciiOptionsController.Apply();
			if (!result) return result;

			if (null != _inputDataController)
			{
				result = _inputDataController.Apply();
				if (!result) return result;
			}

			_doc.SourceFileName = _view.FileName;

			if (!object.ReferenceEquals(_originalDoc, _doc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			var ev = SuccessfullyApplied;
			if (null != ev)
			{
				ev();
			}

			return true;
		}

		protected override void AttachView()
		{
			base.AttachView();
			_view.BrowseFileName += EhBrowseFileName;
		}

		protected override void DetachView()
		{
			_view.BrowseFileName -= EhBrowseFileName;
			base.DetachView();
		}

		private void EhBrowseFileName()
		{
			var options = new OpenFileOptions();
			options.AddFilter("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)");
			options.AddFilter("*.*", "All files (*.*)");
			if (Current.Gui.ShowOpenFileDialog(options))
				_view.FileName = options.FileName;
		}
	}
}