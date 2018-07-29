#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using Altaxo.AddInItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Altaxo.Gui
{
  /// <summary>
  /// Simple command that toggles a <see cref="IsChecked"/> flag.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.SimpleCommand" />
  /// <seealso cref="Altaxo.AddInItems.ICheckableMenuCommand" />
  public abstract class SimpleCheckableCommand : SimpleCommand, ICheckableMenuCommand
  {
    public virtual bool IsChecked { get; set; }

    public override void Execute(object parameter)
    {
      IsChecked = !IsChecked;
    }

    event EventHandler ICheckableMenuCommand.IsCheckedChanged
    {
      add { Current.Gui.RegisterRequerySuggestedHandler(value); }
      remove { Current.Gui.UnregisterRequerySuggestedHandler(value); }
    }

    bool ICheckableMenuCommand.IsChecked(object parameter)
    {
      return this.IsChecked;
    }
  }
}
