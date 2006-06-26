using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Altaxo.Graph;

namespace Altaxo.Gui.Common.Drawing
{
  public partial class PenAllPropertiesControl : UserControl, Altaxo.Main.GUI.IMVCAController
  {
    PenHolder _doc;

    public PenAllPropertiesControl()
    {
      InitializeComponent();
    }

    public PenHolder Pen
    {
      get { return _doc; }
      set
      { 
        _doc = value;
        InitializeValues();
      }
    }

    void InitializeValues()
    {
      _cbColor.Color = _doc.Color;
      _cbDashCap.DashCap = _doc.DashCap;
      _cbDashStyle.DashStyleEx = _doc.DashStyleEx;
      _cbEndCap.LineCapEx = _doc.EndCap;
      _cbStartCap.LineCapEx = _doc.StartCap;
      _cbThickness.Thickness = _doc.Width;
    }

    #region IMVCController Members

    public object ViewObject
    {
      get
      {
        return this;
      }
      set
      {
        throw new Exception("The method or operation is not implemented.");
      }
    }

    public object ModelObject
    {
      get { return _doc; }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      _doc.Color = _cbColor.Color;
      _doc.DashStyleEx = _cbDashStyle.DashStyleEx;
      _doc.Width = _cbThickness.Thickness;
      _doc.StartCap = _cbStartCap.LineCapEx;
      _doc.EndCap = _cbEndCap.LineCapEx;

      return true;
    }

    #endregion
  }
}
