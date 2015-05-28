using Altaxo.Analysis.Statistics.Histograms;
using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Analysis.Statistics
{
	public interface ILinearBinningView
	{
		bool IsUserDefinedBinOffset { get; set; }

		double BinOffset { get; set; }

		bool IsUserDefinedBinWidth { get; set; }

		double BinWidth { get; set; }

		double ResultingBinCount { set; }
	}

	[UserControllerForObject(typeof(LinearBinning))]
	[ExpectedTypeOfView(typeof(ILinearBinningView))]
	public class LinearBinningController : MVCANControllerEditOriginalDocBase<LinearBinning, ILinearBinningView>
	{
		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
			}

			if (null != _view)
			{
				_view.IsUserDefinedBinOffset = _doc.IsUserDefinedBinOffset;
				_view.IsUserDefinedBinWidth = _doc.IsUserDefinedBinWidth;

				_view.BinOffset = _doc.BinOffset;
				_view.BinWidth = _doc.BinWidth;
				_view.ResultingBinCount = _doc.NumberOfBins;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.IsUserDefinedBinOffset = _view.IsUserDefinedBinOffset;
			_doc.IsUserDefinedBinWidth = _view.IsUserDefinedBinWidth;

			if (_doc.IsUserDefinedBinOffset)
				_doc.BinOffset = _view.BinOffset;

			if (_doc.IsUserDefinedBinWidth)
				_doc.BinWidth = _view.BinWidth;

			return ApplyEnd(true, disposeController);
		}
	}
}