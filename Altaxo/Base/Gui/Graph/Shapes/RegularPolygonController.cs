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
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph.Shapes
{
	public interface IRegularPolygonView
	{
		IClosedPathShapeView ShapeGraphicView { get; }

		int Vertices { get; set; }

		double CornerRadiusPt { get; set; }
	}

	[UserControllerForObject(typeof(RegularPolygon), 110)]
	[ExpectedTypeOfView(typeof(IRegularPolygonView))]
	public class RegularPolygonController : MVCANControllerBase<RegularPolygon, IRegularPolygonView>
	{
		private ClosedPathShapeController _shapeCtrl;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_shapeCtrl = new ClosedPathShapeController() { UseDocumentCopy = UseDocument.Directly };
				_shapeCtrl.InitializeDocument(_doc);
			}
			if (null != _view)
			{
				if (null == _shapeCtrl.ViewObject)
					_shapeCtrl.ViewObject = _view.ShapeGraphicView;

				_view.Vertices = _doc.NumberOfVertices;
				_view.CornerRadiusPt = _doc.CornerRadius;
			}
		}

		public override bool Apply(bool disposeController)
		{
			if (!_shapeCtrl.Apply(disposeController))
				return false;

			_doc.CornerRadius = _view.CornerRadiusPt;
			_doc.NumberOfVertices = _view.Vertices;

			if (_useDocumentCopy)
				_originalDoc.CopyFrom(_doc);

			return true;
		}
	}
}