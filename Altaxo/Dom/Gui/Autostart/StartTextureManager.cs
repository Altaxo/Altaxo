﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Altaxo.AddInItems;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Main;

namespace Altaxo.Gui.Autostart
{
  /// <summary>
  /// Responsible for building the textures given in the Addin file(s).
  /// </summary>
  /// <seealso cref="System.Windows.Input.ICommand" />
  public class StartTextureManager : ICommand
  {
    public event EventHandler CanExecuteChanged { add { } remove { } }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      AddInTree.GetTreeNode("/Altaxo/BuiltinTextures").BuildChildItems<object>(this);
    }
  }
}
