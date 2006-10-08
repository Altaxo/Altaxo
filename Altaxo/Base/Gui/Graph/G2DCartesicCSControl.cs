using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Graph
{
  public partial class G2DCartesicCSControl : UserControl, IG2DCartesicCSView
  {
    public G2DCartesicCSControl()
    {
      InitializeComponent();
    }

    #region IG2DCartesicCSView Members

    public bool ExchangeXY
    {
      get
      {
        return _chkExchangeXY.Checked;
      }
      set
      {
        _chkExchangeXY.Checked = value;
      }
    }

    public bool ReverseX
    {
      get
      {
        return _chkXReverse.Checked;
      }
      set
      {
        _chkXReverse.Checked = value;
      }
    }

    public bool ReverseY
    {
      get
      {
        return _chkYReverse.Checked;
      }
      set
      {
        _chkYReverse.Checked = value;
      }
    }

    #endregion
  }

  public interface IG2DCartesicCSView
  {
    bool ExchangeXY { get; set; }
    bool ReverseX { get; set; }
    bool ReverseY { get; set; }
  }
}
