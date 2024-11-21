#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Altaxo.Gui.HelpViewing
{
  public partial class HiddenMainForm : Form
  {
    [DefaultValue(false)]
    public bool IsLoaded { get; private set; }

    public HiddenMainForm()
    {
      InitializeComponent();
      Hide();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      Hide();
      IsLoaded = true;
    }
  }
}
