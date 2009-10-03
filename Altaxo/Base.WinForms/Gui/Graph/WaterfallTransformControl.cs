using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Graph
{
  public partial class WaterfallTransformControl : UserControl, IWaterfallTransformView
  {
    public WaterfallTransformControl()
    {
      InitializeComponent();
    }



    #region IWaterfallTransformView Members

    public string XScale
    {
      get
      {
        return _edXScale.Text;
      }
      set
      {
        _edXScale.Text = value;
      }
    }

    public string YScale
    {
      get
      {
        return _edYScale.Text;
      }
      set
      {
        _edYScale.Text = value;
      }
    }

    public bool UseClipping
    {
      get
      {
        return _chkClipValues.Checked;
      }
      set
      {
        _chkClipValues.Checked = value;
      }
    }

    #endregion
  }
}
