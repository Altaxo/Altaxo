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


using Altaxo.Data;
using Altaxo.Collections;
using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Serialization;
using Altaxo.Calc;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
	#region Interfaces

	public interface ISpanTickSpacingView
	{
		double RelativePositionOfTick { get; set; }
		bool ShowEndOrgRatio { get; set; }

		double DivideBy { get;  set; }
		bool TransfoOperationIsMultiply { get; set; }
	}

	#endregion

	[UserControllerForObject(typeof(SpanTickSpacing))]
	[ExpectedTypeOfView(typeof(ISpanTickSpacingView))]
	public class SpanTickSpacingController : MVCANControllerBase<SpanTickSpacing, ISpanTickSpacingView>
	{
		protected override void Initialize(bool initData)
		{
			if (null != _view)
			{
				_view.RelativePositionOfTick = _doc.RelativeTickPosition;
				_view.ShowEndOrgRatio = _doc.ShowEndOrgRatioInsteadOfDifference;
				_view.DivideBy = _doc.TransformationDivider;
				_view.TransfoOperationIsMultiply = _doc.TransformationOperationIsMultiply;

			}
		}

		public override bool Apply()
		{
			_doc.RelativeTickPosition = _view.RelativePositionOfTick;
			_doc.ShowEndOrgRatioInsteadOfDifference = _view.ShowEndOrgRatio;

			_doc.TransformationDivider = _view.DivideBy;
			_doc.TransformationOperationIsMultiply = _view.TransfoOperationIsMultiply;

			CopyHelper.Copy(ref _originalDoc, _doc);
			return true;
		}
	}
}
