#region Copyright

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.AddInItems;
using Altaxo.Collections;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  public abstract class ProjectBrowseControllerCommand : SimpleCommand
  {
    private ProjectBrowseController _controller;

    protected abstract void Run(ProjectBrowseController ctrl);

    public override void Execute(object parameter)
    {
      _controller = parameter as ProjectBrowseController;

      Run((ProjectBrowseController)parameter);
    }

    public override bool CanExecute(object parameter)
    {
      _controller = parameter as ProjectBrowseController;
      return base.CanExecute(parameter);
    }

    protected ProjectBrowseController Ctrl
    {
      get
      {
        return _controller;
      }
    }
  }
}
