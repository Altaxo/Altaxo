using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Graph
{
  public partial class ErrorBarPlotStyleControl : UserControl, IErrorBarPlotStyleView
  {
    public ErrorBarPlotStyleControl()
    {
      InitializeComponent();
    }

    #region IErrorBarPlotStyleView Members

    public bool IndependentColor
    {
      get
      {
        return _chkIndependentColor.Checked;
      }
      set
      {
        _chkIndependentColor.Checked = value;
      }
    }

    public Altaxo.Graph.Gdi.PenX StrokePen
    {
      get
      {
        return _strokePenGlue.Pen;
      }
      set
      {
        _strokePenGlue.Pen = value;
      }
    }

    public bool IndependentSize
    {
      get
      {
        return _chkIndependentSize.Checked;
      }
      set
      {
        _chkIndependentSize.Checked = value;
      }
    }

    public bool LineSymbolGap
    {
      get
      {
        return _chkLineSymbolGap.Checked;
      }
      set
      {
        _chkLineSymbolGap.Checked = value;
      }
    }


    public bool ShowEndBars
    {
      get
      {
        return _chkShowEndBars.Checked;
      }
      set
      {
        _chkShowEndBars.Checked = value;
      }
    }

    public bool DoNotShiftIndependentVariable
    {
      get
      {
        return this._chkDoNotShift.Checked;
      }
      set
      {
        this._chkDoNotShift.Checked = value;
      }
    }

    public bool IsHorizontalStyle
    {
      get
      {
        return this._chkIsHorizontal.Checked;
      }
      set
      {
        _chkIsHorizontal.Checked = value;
      }
    }

    public void InitializeSymbolSizeList(string[] names, int selection)
    {
      _cbSymbolSize.BeginUpdate();
      _cbSymbolSize.Items.Clear();
      foreach(string name in names)
        _cbSymbolSize.Items.Add(name);

      _cbSymbolSize.SelectedIndex = selection;
      _cbSymbolSize.EndUpdate();
    }

    public string SymbolSize
    {
      get 
      {
        return _cbSymbolSize.Text;
      }
    }

    public bool IndependentNegativeError
    {
      get
      {
        return _chkIndepNegErrorColumn.Checked;
      }
      set
      {
        _chkIndepNegErrorColumn.Checked = value;
        _btSelectNegErrorColumn.Enabled = value;
      }
    }

    public string PositiveError
    {
      get
      {
        return _edErrorColumn.Text;
      }
      set
      {
        _edErrorColumn.Text = value;
      }
    }

    public string NegativeError
    {
      get
      {
        return _edNegErrorColumn.Text;
      }
      set
      {
        _edNegErrorColumn.Text = value;
      }
    }

    public event EventHandler ChoosePositiveError;

    public event EventHandler ChooseNegativeError;

    public event EventHandler IndependentNegativeError_CheckChanged;

    public event CancelEventHandler VerifySymbolSize;

    #endregion

    private void _btSelectErrorColumn_Click(object sender, EventArgs e)
    {
      if (null != ChoosePositiveError)
        ChoosePositiveError(this, EventArgs.Empty);
    }

    private void _btSelectNegErrorColumn_Click(object sender, EventArgs e)
    {
      if (null != ChooseNegativeError)
        ChooseNegativeError(this, EventArgs.Empty);
    }

    private void _chkIndepNegErrorColumn_CheckedChanged(object sender, EventArgs e)
    {
      if (null != IndependentNegativeError_CheckChanged)
        IndependentNegativeError_CheckChanged(this, EventArgs.Empty);

      _btSelectNegErrorColumn.Enabled = _chkIndepNegErrorColumn.Checked;
    }

    private void _cbSymbolSize_Validating(object sender, CancelEventArgs e)
    {
      if (null != VerifySymbolSize)
        VerifySymbolSize(this, e);
    }
  }
}
