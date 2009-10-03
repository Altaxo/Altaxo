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
  public partial class EnumFlagControl : UserControl, IEnumFlagView
  {
    public EnumFlagControl()
    {
      InitializeComponent();
    }

    #region IEnumFlagView Members

    public void SetNames(string[] names)
    {
      _layoutPanel.Controls.Clear();

      for(int i=0;i<names.Length;i++)
      {
        var checkBox = new CheckBox();
				checkBox.AutoSize = true;
        checkBox.Text = names[i];
        checkBox.Tag = i;
				
        checkBox.CheckedChanged += EhCheckedChanged;
        _layoutPanel.Controls.Add(checkBox);
      }
    }

    public void SetChecks(bool[] checks)
    {
			_checkEventEnabled = false;
      int len = Math.Min(checks.Length, _layoutPanel.Controls.Count);
      for (int i = 0; i < len; i++)
      {
        ((CheckBox)_layoutPanel.Controls[i]).Checked = checks[i];
      }
			_checkEventEnabled = true;
    }

    public event Action<int, bool> CheckChanged;
		bool _checkEventEnabled = false;
    void EhCheckedChanged(object sender, EventArgs e)
    {
      if (null != CheckChanged && _checkEventEnabled)
      {
        CheckBox c = (CheckBox)sender;
        int idx = (int)(c.Tag);
        CheckChanged(idx, c.Checked);
      }
    }

    #endregion

   
  }
}
