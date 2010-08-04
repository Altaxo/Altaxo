using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Common
{
	public partial class FreeTextComboBoxControl : UserControl, IFreeTextChoiceView
  {
    public event Action<int> SelectionChangeCommitted;
    public event Action<string, CancelEventArgs> TextValidating;
    
    public FreeTextComboBoxControl()
    {
      InitializeComponent();
    }

    public void SetDescription(string value)
    {
      SizeF size1, size2;
      using (System.Drawing.Graphics grfx = this.CreateGraphics())
      {
        size1 = grfx.MeasureString(value, _lblDescription.Font);
        size2 = grfx.MeasureString(value, _lblDescription.Font, _lblDescription.ClientSize.Width);
      }
      _lblDescription.Size = new Size(_cbChoice.Size.Width, (int)(_lblDescription.PreferredHeight * Math.Ceiling(size2.Height / size1.Height)));
      this._lblDescription.Text = value;
    }

    public void SetChoices(string[] values, int initialselection, bool allowFreeText)
    {
      _cbChoice.DropDownStyle = allowFreeText ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;

      this._cbChoice.Items.Clear();
      this._cbChoice.Items.AddRange(values);
      if (initialselection >= 0 && initialselection < values.Length)
        this._cbChoice.SelectedIndex = initialselection;
    }

    private void EhSelectionChangeCommitted(object sender, EventArgs e)
    {
      if (null != SelectionChangeCommitted)
        SelectionChangeCommitted(_cbChoice.SelectedIndex);
    }

    private void EhValidating(object sender, CancelEventArgs e)
    {
      if (null != TextValidating && _cbChoice.SelectedIndex<0)
        TextValidating(_cbChoice.Text, e);
    }
  }
}
