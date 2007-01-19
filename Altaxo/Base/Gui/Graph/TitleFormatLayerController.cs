#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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

    event EventHandler ShowAxisLineChanged;

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
  public class TitleFormatLayerController : IMVCANController
  {
    protected ITitleFormatLayerView m_View;
    protected AxisStyle _doc;

    protected bool    m_ShowAxis;
    protected bool    m_Original_ShowAxis;

    protected string  m_Title;
    protected string  m_Original_Title;

    protected int     m_AxisPosition;
    protected int     m_Original_AxisPosition;

    protected string  m_AxisPositionValue;
    protected string  m_Original_AxisPositionValue;

    protected bool    m_AxisPositionValueEnabled = true;

    protected IMVCAController _axisLineStyleController;

    public bool InitializeDocument(params object[] args)
    {
      if (args.Length == 0 || !(args[0] is AxisStyle))
        return false;
      _doc = (AxisStyle)args[0];
      this.SetElements(true);
      return true;
    }

    public UseDocument UseDocumentCopy { set { } }

    public void SetElements(bool bInit)
    {
      System.Collections.ArrayList arr = new System.Collections.ArrayList();

      if (bInit)
      {
        if (_doc.AxisLineStyle != null)
          _axisLineStyleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisLineStyle }, typeof(IMVCAController), UseDocument.Directly);
        else
        {
          _axisLineStyleController = null;
        }

      }

      if (m_View != null)
      {
        m_View.AxisTitle = _doc.TitleText;
        m_View.ShowAxisLine = _doc.ShowAxisLine;
        m_View.ShowMajorLabels = _doc.ShowMajorLabels;
        m_View.ShowMinorLabels = _doc.ShowMinorLabels;
        m_View.LineStyleView = _axisLineStyleController == null ? null : _axisLineStyleController.ViewObject;
      }
    }

    private void EhShowAxisLineChanged(object sender, EventArgs e)
    {
      if (m_View.ShowAxisLine && null==_doc.AxisLineStyle)
      {
        _doc.ShowAxisLine = true;
        this._axisLineStyleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc.AxisLineStyle }, typeof(IMVCAController));
        m_View.LineStyleView = _axisLineStyleController.ViewObject;
      }
    }
  


    #region ITitleFormatLayerController Members

    public object ViewObject
    {
      get { return m_View; }
      set 
      {
        ITitleFormatLayerView oldvalue = m_View;
        m_View = value as ITitleFormatLayerView;

        if(!object.ReferenceEquals(oldvalue,value))
        {
          if (null != oldvalue)
            oldvalue.ShowAxisLineChanged -= EhShowAxisLineChanged;
          if (null != value)
            m_View.ShowAxisLineChanged += EhShowAxisLineChanged;

        SetElements(false);
        }
      }
    }

    
    public object ModelObject
    {
      get { return this._doc; }
    }

   

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
        // read axis title
        _doc.TitleText = m_View.AxisTitle;

        if (null != _axisLineStyleController)
        {
          if (!_axisLineStyleController.Apply())
            return false;
          else
            _doc.AxisLineStyle = (AxisLineStyle)_axisLineStyleController.ModelObject;
        }

        _doc.ShowMajorLabels = m_View.ShowMajorLabels;
        _doc.ShowMinorLabels = m_View.ShowMinorLabels;

        // if we have offset applying, create a brand new AxisStyle instance
        double offset = m_View.PositionOffset/100;
        if (0 != offset)
        {
          AxisStyle newDoc = new AxisStyle(CSLineID.FromIDandFirstLogicalOffset(_doc.StyleID, offset));
          newDoc.CopyWithoutIdFrom(_doc);
          _doc = newDoc;
        }


      return true; // all ok
    }

    #endregion
  }
}
