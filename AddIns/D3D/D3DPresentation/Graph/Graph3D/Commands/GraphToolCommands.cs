﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Altaxo.Collections;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Graph3D.Viewing;
using Altaxo.Gui.Scripting;
using Altaxo.Main;
using Altaxo.Scripting;

namespace Altaxo.Graph.Graph3D.Commands
{
  /// <summary>
  /// This class is intented to be used for commands into the graph tools toolbar. Commands derived
  /// from it will update the toolbar whenever its state changed.
  /// </summary>
  public abstract class AbstractGraphToolsCommand : AbstractCheckableGraphControllerCommand
  {
    private Graph3DController? _myCurrentGraphController;
    private GraphToolType _graphToolType;

    protected AbstractGraphToolsCommand(GraphToolType toolType)
    {
      _graphToolType = toolType;
      if (Current.Workbench is not null)
      {
        Current.Workbench.ActiveViewContentChanged += new WeakEventHandler(EhWorkbenchContentChanged, Current.Workbench, nameof(Current.Workbench.ActiveViewContentChanged));
        EhWorkbenchContentChanged(this, EventArgs.Empty);
      }
    }

    protected void EhWorkbenchContentChanged(object? o, System.EventArgs e)
    {
      if (!object.ReferenceEquals(Controller, _myCurrentGraphController))
      {
        if (_myCurrentGraphController is not null)
        {
          lock (this)
          {
            _myCurrentGraphController.CurrentGraphToolChanged -= new EventHandler(EhGraphToolChanged);
            _myCurrentGraphController = null;
          }
        }
        if (Controller is not null)
        {
          lock (this)
          {
            _myCurrentGraphController = Controller;
            _myCurrentGraphController.CurrentGraphToolChanged += new EventHandler(EhGraphToolChanged);
          }
        }
        OnPropertyChanged("IsChecked");
      }
    }

    protected void EhGraphToolChanged(object? o, EventArgs e)
    {
      OnPropertyChanged("IsChecked");
    }

    public override bool IsChecked
    {
      get
      {
        return Controller is null ? false : _graphToolType == Controller.CurrentGraphTool;
      }
      set
      {
        if (value == true && Controller is not null)
        {
          Controller.CurrentGraphTool = _graphToolType;
        }
        OnPropertyChanged("IsChecked");
      }
    }
  }

  /// <summary>
  /// Test class for a selected item
  /// </summary>
  public class SelectPointerTool : AbstractGraphToolsCommand
  {
    public SelectPointerTool()
      : base(GraphToolType.ObjectPointer)
    {
    }
  }

  /// <summary>
  /// Test class for a selected item
  /// </summary>
  public class SelectTextTool : AbstractGraphToolsCommand
  {
    public SelectTextTool()
      : base(GraphToolType.TextDrawing)
    {
    }
  }

  /// <summary>
  /// Drawing an ellipse on the graph.
  /// </summary>
  public class EllipseDrawingTool : AbstractGraphToolsCommand
  {
    public EllipseDrawingTool()
      : base(GraphToolType.EllipseDrawing)
    {
    }
  }

  /// <summary>
  /// Drawing a single straight line on the graph.
  /// </summary>
  public class SingleLineDrawingTool : AbstractGraphToolsCommand
  {
    public SingleLineDrawingTool()
      : base(GraphToolType.SingleLineDrawing)
    {
    }
  }
}
