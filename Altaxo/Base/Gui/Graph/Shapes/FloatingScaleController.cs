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

using Altaxo.Collections;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Graph.Gdi;
using Altaxo.Graph;

namespace Altaxo.Gui.Graph.Shapes
{
	public interface IFloatingScaleView
	{
		PointD2D DocPosition { get; set; }
		int ScaleNumber { get; set; }

		double ScaleSpan { get; set; }
		bool ScaleSpanIsPhysicalValue { get; set; }

		IXYAxisLabelStyleView LabelStyleView { get; }
	}

	[UserControllerForObject(typeof(FloatingScale), 110)]
	[ExpectedTypeOfView(typeof(IFloatingScaleView))]
	public class FloatingScaleController : MVCANControllerBase<FloatingScale, IFloatingScaleView>
	{
		XYAxisLabelStyleController _labelStyleController;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_labelStyleController = new XYAxisLabelStyleController();
				_labelStyleController.UseDocumentCopy = UseDocument.Directly;
				_labelStyleController.InitializeDocument(_doc.LabelStyle);
			}
			if (null != _view)
			{
				_view.DocPosition = _doc.Position;
				_view.ScaleNumber = _doc.ScaleNumber;
				_view.ScaleSpanIsPhysicalValue = _doc.IsScaleSpanPhysicalValue;
				_view.ScaleSpan = _doc.ScaleSpan;

				if (null == _labelStyleController.ViewObject)
					_labelStyleController.ViewObject = _view.LabelStyleView;
			}

		}

		public override bool Apply()
		{
			_doc.Position = _view.DocPosition;
			_doc.ScaleNumber = _view.ScaleNumber;
			_doc.IsScaleSpanPhysicalValue = _view.ScaleSpanIsPhysicalValue;
			_doc.ScaleSpan = _view.ScaleSpan;

			if (!_labelStyleController.Apply())
				return false;

			if (!object.ReferenceEquals(_doc, _originalDoc))
				_originalDoc.CopyFrom(_doc);

			return true;

		}
	}
}
