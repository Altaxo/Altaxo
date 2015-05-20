#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Analysis.Statistics
{
	public interface IHistogramCreationView
	{
		IEnumerable<string> Errors { set; }

		IEnumerable<string> Warnings { set; }

		double TotalNumberOfValues { set; }

		double NumberOfNaNValues { set; }

		double NumberOfInfiniteValues { set; }

		double MinimumValue { set; }

		double MaximumValue { set; }

		bool IgnoreNaNValues { get; set; }

		bool IgnoreInfiniteValues { get; set; }

		bool IgnoreValuesBelowLowerBoundary { get; set; }

		bool IsLowerBoundaryInclusive { get; set; }

		double LowerBoundary { get; set; }

		bool IgnoreValuesAboveUpperBoundary { get; set; }

		bool IsUpperBoundaryInclusive { get; set; }

		double UpperBoundary { get; set; }

		bool UseAutomaticBinning { get; set; }

		SelectableListNodeList BinningType { set; }

		object BinningView { set; }
	}

	[UserControllerForObject(typeof(HistogramCreationInformation))]
	[ExpectedTypeOfView(typeof(IHistogramCreationView))]
	public class HistogramCreationController : MVCANControllerEditOriginalDocBase<HistogramCreationInformation, IHistogramCreationView>
	{
		private IMVCANController _binningController;
		private SelectableListNodeList _binningTypes = new SelectableListNodeList();

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			if (null != _binningController)
				yield return new ControllerAndSetNullMethod(_binningController, () => _binningController = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_binningTypes.Clear();

				var binningTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IBinningDefinition));

				foreach (var type in binningTypes)
					_binningTypes.Add(new SelectableListNode(type.ToString(), type, type == _doc.CreationOptions.Binning.GetType()));
			}
			if (null != _view)
			{
				_view.TotalNumberOfValues = _doc.NumberOfValues;
				_view.NumberOfNaNValues = _doc.NumberOfNaNValues;
				_view.NumberOfInfiniteValues = _doc.NumberOfInfiniteValues;
				_view.MinimumValue = _doc.MinimumValue;
				_view.MaximumValue = _doc.MaximumValue;

				_view.IgnoreNaNValues = _doc.CreationOptions.IgnoreNaN;
				_view.IgnoreInfiniteValues = _doc.CreationOptions.IgnoreInfinity;

				_view.IgnoreValuesBelowLowerBoundary = !_doc.CreationOptions.LowerBoundaryToIgnore.HasValue;
				_view.IsLowerBoundaryInclusive = _doc.CreationOptions.IsLowerBoundaryInclusive;
				if (_doc.CreationOptions.LowerBoundaryToIgnore.HasValue)
					_view.LowerBoundary = _doc.CreationOptions.LowerBoundaryToIgnore.Value;

				_view.IgnoreValuesAboveUpperBoundary = !_doc.CreationOptions.UpperBoundaryToIgnore.HasValue;
				_view.IsUpperBoundaryInclusive = _doc.CreationOptions.IsUpperBoundaryInclusive;
				if (_doc.CreationOptions.UpperBoundaryToIgnore.HasValue)
					_view.UpperBoundary = _doc.CreationOptions.UpperBoundaryToIgnore.Value;

				_view.BinningType = _binningTypes;

				_view.UseAutomaticBinning = !_doc.CreationOptions.IsUserDefinedBinningType;
			}
		}

		public override bool Apply(bool disposeController)
		{
			_doc.CreationOptions.IgnoreNaN = _view.IgnoreNaNValues;
			_doc.CreationOptions.IgnoreInfinity = _view.IgnoreInfiniteValues;

			if (_view.IgnoreValuesBelowLowerBoundary)
			{
				_doc.CreationOptions.IsLowerBoundaryInclusive = _view.IsLowerBoundaryInclusive;
				_doc.CreationOptions.LowerBoundaryToIgnore = _view.LowerBoundary;
			}
			else
			{
				_doc.CreationOptions.IsLowerBoundaryInclusive = true;
				_doc.CreationOptions.LowerBoundaryToIgnore = null;
			}

			if (_view.IgnoreValuesAboveUpperBoundary)
			{
				_doc.CreationOptions.IsUpperBoundaryInclusive = _view.IsUpperBoundaryInclusive;
				_doc.CreationOptions.UpperBoundaryToIgnore = _view.UpperBoundary;
			}
			else
			{
				_doc.CreationOptions.IsUpperBoundaryInclusive = true;
				_doc.CreationOptions.UpperBoundaryToIgnore = null;
			}

			_doc.CreationOptions.IsUserDefinedBinningType = !_view.UseAutomaticBinning;

			if (!_binningController.Apply(disposeController))
				return ApplyEnd(false, disposeController);

			return ApplyEnd(true, disposeController);
		}
	}
}