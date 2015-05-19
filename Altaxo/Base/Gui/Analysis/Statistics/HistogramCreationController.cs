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

		bool IsLowerBoundaryIsExclusive { get; set; }

		double LowerBoundary { get; set; }

		bool IgnoreValuesAboveUpperBoundary { get; set; }

		bool IsUpperBoundaryIsExclusive { get; set; }

		double UpperBoundary { get; set; }

		bool UseAutomaticBinning { get; set; }

		SelectableListNodeList BinningType { set; }

		object BinningView { set; }
	}

	public class HistogramCreationController : MVCANControllerEditOriginalDocBase<HistogramCreationInformation, IHistogramCreationView>
	{
		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
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
				_view.IsLowerBoundaryIsExclusive = !_doc.CreationOptions.IsLowerBoundaryInclusive;
				if (_doc.CreationOptions.LowerBoundaryToIgnore.HasValue)
					_view.LowerBoundary = _doc.CreationOptions.LowerBoundaryToIgnore.Value;

				_view.IgnoreValuesAboveUpperBoundary = !_doc.CreationOptions.UpperBoundaryToIgnore.HasValue;
				_view.IsUpperBoundaryIsExclusive = !_doc.CreationOptions.IsUpperBoundaryInclusive;
				if (_doc.CreationOptions.UpperBoundaryToIgnore.HasValue)
					_view.UpperBoundary = _doc.CreationOptions.UpperBoundaryToIgnore.Value;

				_view.UseAutomaticBinning = null == _doc.CreationOptions.UserDefinedBinning;
			}
		}
	}
}