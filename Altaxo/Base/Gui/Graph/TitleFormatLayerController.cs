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
using System.Drawing;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Gui;
using Altaxo.Graph;

namespace Altaxo.Gui.Graph
{
  #region Interfaces

  public interface ITitleFormatLayerView 
  {
    bool ShowAxisLine { get; set; }
    bool ShowMajorLabels { get; set; }
    bool ShowMinorLabels { get; set; }

    event Action ShowAxisLineChanged;
		event Action ShowMajorLabelsChanged;
		event Action ShowMinorLabelsChanged;

    object LineStyleView { set; }

    string AxisTitle { get; set; }

    double PositionOffset { get; }

  }
  #endregion


  /// <summary>
  /// Summary description for TitleFormatLayerController.
  /// </summary>
  [UserControllerForObject(typeof(AxisStyle),90)]
  [ExpectedTypeOfView(typeof(ITitleFormatLayerView))]
  public class TitleFormatLayerController : MVCANDControllerBase<AxisStyle, ITitleFormatLayerView>
  {
    protected IMVCAController _axisLineStyleController;

    protected override void Initialize(bool bInit)
    {
      System.Collections.ArrayList arr = new System.Collections.ArrayList();

      if (bInit)
      {
				if (_doc.AxisLineStyle != null)
				{
					_axisLineStyleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _originalDoc.AxisLineStyle }, typeof(IMVCAController), UseDocument.Directly);
				}
				else
				{
					_axisLineStyleController = null;
				}
      }

      if (_view != null)
      {
        _view.AxisTitle = _originalDoc.TitleText;
        _view.ShowAxisLine = _originalDoc.ShowAxisLine;
        _view.ShowMajorLabels = _originalDoc.ShowMajorLabels;
        _view.ShowMinorLabels = _originalDoc.ShowMinorLabels;
        _view.LineStyleView = _axisLineStyleController == null ? null : _axisLineStyleController.ViewObject;
      }
    }

		public override bool Apply()
		{
			// read axis title
			_doc.TitleText = _view.AxisTitle;

			if (null != _axisLineStyleController)
			{
				if (!_axisLineStyleController.Apply())
					return false;
				else
					_doc.AxisLineStyle = (AxisLineStyle)_axisLineStyleController.ModelObject;
			}

			_doc.ShowMajorLabels = _view.ShowMajorLabels;
			_doc.ShowMinorLabels = _view.ShowMinorLabels;

			// if we have offset applying, create a brand new AxisStyle instance
			double offset = _view.PositionOffset / 100;
			if (0 != offset)
			{
				AxisStyle newDoc = new AxisStyle(CSLineID.FromIDandFirstLogicalOffset(_doc.StyleID, offset));
				newDoc.CopyWithoutIdFrom(_doc);
				_doc = newDoc;
			}

			if (!object.ReferenceEquals(_doc, _originalDoc))
				_originalDoc.CopyFrom(_doc);

			return true; // all ok
		}



		protected override void AttachView()
		{
			_view.ShowAxisLineChanged += EhShowAxisLineChanged;
			_view.ShowMajorLabelsChanged += EhShowMajorLabelsChanged;
			_view.ShowMinorLabelsChanged += EhShowMinorLabelsChanged;
		}

		protected override void DetachView()
		{
			_view.ShowAxisLineChanged -= EhShowAxisLineChanged;
			_view.ShowMajorLabelsChanged -= EhShowMajorLabelsChanged;
			_view.ShowMinorLabelsChanged -= EhShowMinorLabelsChanged;
		}

    private void EhShowAxisLineChanged()
    {
			var oldValue = _doc.ShowAxisLine;
      if (_view.ShowAxisLine && null==_originalDoc.AxisLineStyle)
      {
        _doc.ShowAxisLine = true;
        this._axisLineStyleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisLineStyle }, typeof(IMVCAController), UseDocument.Directly);
        _view.LineStyleView = _axisLineStyleController.ViewObject;
      }
			if (oldValue != _doc.ShowAxisLine)
				OnMadeDirty();
    }

		private void EhShowMajorLabelsChanged()
		{
			var oldValue = _doc.ShowMajorLabels;
			_doc.ShowMajorLabels = _view.ShowMajorLabels;

			if (oldValue != _doc.ShowMajorLabels)
				OnMadeDirty();
		}


		private void EhShowMinorLabelsChanged()
		{

			var oldValue = _doc.ShowMinorLabels;
			_doc.ShowMinorLabels = _view.ShowMinorLabels;

			if (oldValue != _doc.ShowMinorLabels)
				OnMadeDirty();
		}
  }
}
