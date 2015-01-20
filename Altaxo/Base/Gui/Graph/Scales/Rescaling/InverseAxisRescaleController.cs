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

#endregion Copyright

using Altaxo.Graph.Scales;
using Altaxo.Graph.Scales.Rescaling;
using Altaxo.Serialization;
using System;

namespace Altaxo.Gui.Graph.Scales.Rescaling
{
	/// <summary>
	/// Summary description for NumericAxisRescaleController.
	/// </summary>
	[UserControllerForObject(typeof(InverseAxisRescaleConditions), 1)]
	public class InverseAxisRescaleController
		:
		NumericAxisRescaleController
	{
		#region IOrgEndSpanControlEventReceiver Members

		public override bool EhValue1Changed(string txt)
		{
			if (!GUIConversion.IsDouble(txt))
				return true;

			double val;
			GUIConversion.IsDouble(txt, out val);
			if (!double.IsNaN(val))
			{
				_org = val;
				_orgChanged = true;
			}
			return double.IsNaN(val);
		}

		public override bool EhValue2Changed(string txt)
		{
			if (!GUIConversion.IsDouble(txt))
				return true;

			double val;
			GUIConversion.IsDouble(txt, out val);
			if (!double.IsNaN(val))
			{
				_end = val;
				_endChanged = true;
			}
			return double.IsNaN(val);
		}

		protected override void SetEnableState()
		{
			bool enableSpan = false;
			_view.EnableChoice1(!enableSpan);
			_view.EnableChoice2(!enableSpan);
			_view.EnableChoice3(false);

			_view.EnableValue1(!enableSpan);
			_view.EnableValue2(!enableSpan);
			_view.EnableValue3(enableSpan);
		}

		public override bool Apply(bool disposeController)
		{
			if (!(1 / _end > 1 / _org))
			{
				Current.Gui.ErrorMessageBox("Please note: 1/End must be greater than 1/Org.");
				return false;
			}

			_doc.SetOrgEndSpan(_orgRescaling, _org, _endRescaling, _end, _spanRescaling, _span, _minorTicks, _majorTick);

			if (null != _scale)
			{
				// if the user changed org or end, he maybe want to set the scale temporarily to the chosen values
				if (_orgRescaling == BoundaryRescaling.Auto && _endRescaling == BoundaryRescaling.Auto && (_orgChanged || _endChanged))
					_scale.SetScaleOrgEnd(_org, _end);
				else
					_scale.Rescale();
			}

			_orgChanged = _endChanged = false;

			Initialize(true);

			return ApplyEnd(true, disposeController);
		}

		#endregion IOrgEndSpanControlEventReceiver Members
	}
}