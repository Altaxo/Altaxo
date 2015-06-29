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

using Altaxo.Graph.Gdi.Plot.Groups;
using Altaxo.Units;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph
{
	#region Interfaces

	public interface IWaterfallTransformView
	{
		void SetXScaleUnitEnvironment(QuantityWithUnitGuiEnvironment environment);

		DimensionfulQuantity XScale { get; set; }

		void SetYScaleUnitEnvironment(QuantityWithUnitGuiEnvironment environment);

		DimensionfulQuantity YScale { get; set; }

		bool UseClipping { get; set; }
	}

	#endregion Interfaces

	[UserControllerForObject(typeof(WaterfallTransform))]
	[ExpectedTypeOfView(typeof(IWaterfallTransformView))]
	public class WaterfallTransformController : MVCANControllerEditOriginalDocBase<WaterfallTransform, IWaterfallTransformView>
	{
		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield break;
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (_view != null)
			{
				_view.SetXScaleUnitEnvironment(RelationEnvironment.Instance);
				_view.XScale = new DimensionfulQuantity(_doc.XScale, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelationEnvironment.Instance.DefaultUnit);
				_view.SetYScaleUnitEnvironment(RelationEnvironment.Instance);
				_view.YScale = new DimensionfulQuantity(_doc.YScale, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelationEnvironment.Instance.DefaultUnit);
				_view.UseClipping = _doc.UseClipping;
			}
		}

		public override bool Apply(bool disposeController)
		{
			try
			{
				_doc.UseClipping = _view.UseClipping;
				_doc.XScale = _view.XScale.AsValueInSIUnits;
				_doc.YScale = _view.YScale.AsValueInSIUnits;
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox(ex.Message, "Error while applying..");
				return ApplyEnd(false, disposeController);
			}

			return ApplyEnd(true, disposeController);
		}
	}
}