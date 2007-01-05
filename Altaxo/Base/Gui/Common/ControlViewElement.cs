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
using System.Collections;
using System.Text;

namespace Altaxo.Gui.Common
{
  public class ControlViewElement : ViewDescriptionElement, ICloneable
  {
    public IApplyController Controller;

    public ControlViewElement(ControlViewElement from)
      : base(from)
    {
      this.Controller = from.Controller;
    }

    public ControlViewElement(string title, IApplyController controller, object view)
      : base(title,view)
    {
      this.Controller = controller;
    }

    public ControlViewElement(string title, IMVCAController controller)
      : base(title,controller.ViewObject)
    {
      this.Controller = controller;
    }

    public new ControlViewElement Clone()
    {
      return new ControlViewElement(this);
    }

    #region ICloneable Members


    object ICloneable.Clone()
    {
      return new ControlViewElement(this);
    }

    #endregion
  }
}
