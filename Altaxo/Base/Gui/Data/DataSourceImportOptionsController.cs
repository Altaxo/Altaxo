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

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Data
{
	public interface IDataSourceImportOptionsView
	{
		bool DoNotSaveTableData { get; set; }

		bool ExecuteScriptAfterImport { get; set; }

		void InitializeTriggerSource(Altaxo.Collections.SelectableListNodeList list);

		double MinimumWaitingTimeAfterUpdateInSeconds { get; set; }

		double MaximumWaitingTimeAfterUpdateInSeconds { get; set; }

		double MinimumWaitingTimeAfterFirstTriggerInSeconds { get; set; }

		double MaximumWaitingTimeAfterFirstTriggerInSeconds { get; set; }

		double MinimumWaitingTimeAfterLastTriggerInSeconds { get; set; }
	}

	[ExpectedTypeOfView(typeof(IDataSourceImportOptionsView))]
	[UserControllerForObject(typeof(IDataSourceImportOptions))]
	public class DataSourceImportOptionsController : MVCANControllerBase<DataSourceImportOptions, IDataSourceImportOptionsView>
	{
		private Altaxo.Collections.SelectableListNodeList _triggerChoices;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_triggerChoices = new Collections.SelectableListNodeList(_doc.ImportTriggerSource);
			}
			if (null != _view)
			{
				_view.InitializeTriggerSource(_triggerChoices);
				_view.DoNotSaveTableData = _doc.DoNotSaveCachedTableData;
				_view.ExecuteScriptAfterImport = _doc.ExecuteTableScriptAfterImport;
				_view.MinimumWaitingTimeAfterUpdateInSeconds = _doc.MinimumWaitingTimeAfterUpdateInSeconds;
				_view.MaximumWaitingTimeAfterUpdateInSeconds = _doc.MaximumWaitingTimeAfterUpdateInSeconds;
				_view.MinimumWaitingTimeAfterFirstTriggerInSeconds = _doc.MinimumWaitingTimeAfterFirstTriggerInSeconds;
				_view.MaximumWaitingTimeAfterFirstTriggerInSeconds = _doc.MaximumWaitingTimeAfterFirstTriggerInSeconds;
				_view.MinimumWaitingTimeAfterLastTriggerInSeconds = _doc.MinimumWaitingTimeAfterLastTriggerInSeconds;
			}
		}

		public override bool Apply()
		{
			_doc.DoNotSaveCachedTableData = _view.DoNotSaveTableData;
			_doc.ExecuteTableScriptAfterImport = _view.ExecuteScriptAfterImport;
			_doc.ImportTriggerSource = (Altaxo.Data.ImportTriggerSource)_triggerChoices.FirstSelectedNode.Tag;

			var minUpdate = _view.MinimumWaitingTimeAfterUpdateInSeconds;
			var maxUpdate = _view.MaximumWaitingTimeAfterUpdateInSeconds;
			var minFirstTrig = _view.MinimumWaitingTimeAfterFirstTriggerInSeconds;
			var maxFirstTrig = _view.MaximumWaitingTimeAfterFirstTriggerInSeconds;
			var minLastTrig = _view.MinimumWaitingTimeAfterLastTriggerInSeconds;

			if (!(maxUpdate > 0))
			{
				Current.Gui.ErrorMessageBox("MaximumWaitingTimeAfterUpdate should be > 0");
				return false;
			}
			if (!(minUpdate >= 0))
			{
				Current.Gui.ErrorMessageBox("MinimumWaitingTimeAfterUpdate should be >= 0");
				return false;
			}
			if (!(maxUpdate >= minUpdate))
			{
				Current.Gui.ErrorMessageBox("MaximumWaitingTimeAfterUpdate should be >= MinimumWaitingTimeAfterUpdate");
				return false;
			}

			if (!(minFirstTrig >= 0))
			{
				Current.Gui.ErrorMessageBox("MinimumWaitingTimeAfterFirstTrigger should be > 0");
				return false;
			}

			if (!(maxFirstTrig >= 0))
			{
				Current.Gui.ErrorMessageBox("MaximumWaitingTimeAfterFirstTrigger should be > 0");
				return false;
			}

			if (!(minLastTrig >= 0))
			{
				Current.Gui.ErrorMessageBox("MinimumWaitingTimeAfterLastTrigger should be > 0");
				return false;
			}

			if (!(maxFirstTrig >= minFirstTrig))
			{
				Current.Gui.ErrorMessageBox("MaximumWaitingTimeAfterFirstTrigger should be >= MinimumWaitingTimeAfterFirstTrigger");
				return false;
			}

			_doc.MinimumWaitingTimeAfterUpdateInSeconds = minUpdate;
			_doc.MaximumWaitingTimeAfterUpdateInSeconds = maxUpdate;
			_doc.MinimumWaitingTimeAfterFirstTriggerInSeconds = minFirstTrig;
			_doc.MinimumWaitingTimeAfterLastTriggerInSeconds = minLastTrig;
			_doc.MaximumWaitingTimeAfterFirstTriggerInSeconds = maxFirstTrig;

			if (!object.ReferenceEquals(_doc, _originalDoc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}