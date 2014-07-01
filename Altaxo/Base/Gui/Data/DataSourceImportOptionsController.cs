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

		double MinimumTimeIntervalBetweenUpdatesInSeconds { get; set; }

		double PollTimeIntervalInSeconds { get; set; }
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
				_view.MinimumTimeIntervalBetweenUpdatesInSeconds = _doc.MinimumTimeIntervalBetweenUpdatesInSeconds;
				_view.PollTimeIntervalInSeconds = _doc.PollTimeIntervalInSeconds;
			}
		}

		public override bool Apply()
		{
			_doc.DoNotSaveCachedTableData = _view.DoNotSaveTableData;
			_doc.ExecuteTableScriptAfterImport = _view.ExecuteScriptAfterImport;
			_doc.ImportTriggerSource = (Altaxo.Data.ImportTriggerSource)_triggerChoices.FirstSelectedNode.Tag;

			var minTime = _view.MinimumTimeIntervalBetweenUpdatesInSeconds;
			var pollTime = _view.PollTimeIntervalInSeconds;

			if (!(pollTime > 0))
			{
				Current.Gui.ErrorMessageBox("PollTime should be > 0");
				return false;
			}
			if (!(minTime >= 0))
			{
				Current.Gui.ErrorMessageBox("MinimumTimeIntervalBetweenUpdates should be >= 0");
				return false;
			}
			if (!(pollTime >= minTime))
			{
				Current.Gui.ErrorMessageBox("PollTime should be >= MinimumTimeIntervalBetweenUpdates");
				return false;
			}

			_doc.MinimumTimeIntervalBetweenUpdatesInSeconds = minTime;
			_doc.PollTimeIntervalInSeconds = pollTime;

			if (!object.ReferenceEquals(_doc, _originalDoc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}