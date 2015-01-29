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

using Altaxo.Graph;
using Altaxo.Graph.Gdi.Shapes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph.Shapes
{
	public interface IOpenCardinalSplineView
	{
		IOpenPathShapeView LineGraphicView { get; }

		ICardinalSplinePointsView SplinePointsView { get; }
	}

	[UserControllerForObject(typeof(OpenCardinalSpline), 110)]
	[ExpectedTypeOfView(typeof(IOpenCardinalSplineView))]
	public class OpenCardinalSplineController : MVCANControllerEditOriginalDocBase<OpenCardinalSpline, IOpenCardinalSplineView>
	{
		private OpenPathShapeController _lineCtrl;
		private CardinalSplinePointsController _splinePointsCtrl;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_lineCtrl, () => _lineCtrl = null);
			//yield return new ControllerAndSetNullMethod(_splinePointsCtrl, () => _splinePointsCtrl = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_lineCtrl = new OpenPathShapeController() { UseDocumentCopy = UseDocument.Directly };
				_lineCtrl.InitializeDocument(_doc);
			}
			if (null != _view)
			{
				if (null == _lineCtrl.ViewObject)
					_lineCtrl.ViewObject = _view.LineGraphicView;

				_splinePointsCtrl = new CardinalSplinePointsController(_view.SplinePointsView, _doc.CurvePoints, _doc.Tension);
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (!_lineCtrl.Apply(disposeController))
				return false;

			List<PointD2D> list;
			double tension;

			if (_splinePointsCtrl.Apply(out list, out tension))
			{
				if (!(list.Count >= 2))
				{
					Current.Gui.ErrorMessageBox("At least two points are required for the open cardinal spline. Please enter more points!");
					return false;
				}

				_doc.CurvePoints = list;
				_doc.Tension = tension;
			}
			else
			{
				return false;
			}

			return ApplyEnd(true, disposeController);
		}
	}
}