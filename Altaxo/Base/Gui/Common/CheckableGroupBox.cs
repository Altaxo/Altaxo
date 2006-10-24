using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Common
{
  public class CheckableGroupBox : GroupBox
  {
    protected bool _checked;
    protected Rectangle _checkRectangle;
    public event EventHandler CheckedChanged;

    public bool Checked
    {
      get
      {
        return _checked;
      }
      set
      {
        bool oldvalue = _checked;
        _checked = value;

        if (oldvalue != value)
        {
          if (null != CheckedChanged)
            CheckedChanged(this, EventArgs.Empty);

          Invalidate();
        }
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);

      Size si = SystemInformation.MenuCheckSize;
      SizeF tsize = e.Graphics.MeasureString(Text, this.Font);
      _checkRectangle = new Rectangle(new Point(si.Width / 3 + (int)tsize.Width, 0), si);
      ControlPaint.DrawCheckBox(e.Graphics, _checkRectangle, _checked ? ButtonState.Checked : ButtonState.Normal);

    }

    protected override void OnMouseClick(MouseEventArgs e)
    {
      if (_checkRectangle.Contains(e.Location))
        Checked = !Checked;
      else
        base.OnMouseClick(e);
    }
    

  }
}
