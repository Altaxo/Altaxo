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
