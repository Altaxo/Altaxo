#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using Altaxo.Gui;
using Altaxo.Gui.AddInItems;
using Altaxo.Gui.Workbench;
using System;
using System.IO;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Menu point to provoke an exception.
  /// </summary>
  public class AltaxoProvokeException : SimpleCommand
  {
    private bool _disable;

    internal bool Disable { set { _disable = value; } }

    public override void Execute(object parameter)
    {
      if (!_disable)
        throw new ApplicationException("This is a menu point to provoke an exception");
      System.Diagnostics.Debug.WriteLine("Exception thrown");
    }
  }
}

namespace Altaxo.Main.Commands.ScriptEditorCommands
{
  public class Cut : SimpleCommand
  {
    public override bool CanExecute(object parameter)
    {
      if (!(parameter is IWorkbenchContent activeContent))
        activeContent = Current.Workbench.ActiveContent;

      if (activeContent is IClipboardHandler editable)
      {
        return editable.EnableCut;
      }
      return false;
    }

    public override void Execute(object parameter)
    {
      if (CanExecute(parameter))
      {
        if (!(parameter is IWorkbenchContent activeContent))
          activeContent = Current.Workbench.ActiveContent;

        if (activeContent is IClipboardHandler editable)
        {
          editable.Cut();
        }
      }
    }
  }

  public class Copy : SimpleCommand
  {
    public override bool CanExecute(object parameter)
    {
      if (!(parameter is IWorkbenchContent activeContent))
        activeContent = Current.Workbench.ActiveContent;

      if (activeContent is IClipboardHandler editable)
      {
        return editable.EnableCopy;
      }
      return false;
    }

    public override void Execute(object parameter)
    {
      if (CanExecute(parameter))
      {
        if (!(parameter is IWorkbenchContent activeContent))
          activeContent = Current.Workbench.ActiveContent;

        if (activeContent is IClipboardHandler editable)
        {
          editable.Copy();
        }
      }
    }
  }

  public class Paste : SimpleCommand
  {
    public override bool CanExecute(object parameter)
    {
      if (!(parameter is IWorkbenchContent activeContent))
        activeContent = Current.Workbench.ActiveContent;

      if (activeContent is IClipboardHandler editable)
      {
        return editable.EnablePaste;
      }
      return false;
    }

    public override void Execute(object parameter)
    {
      if (CanExecute(parameter))
      {
        if (!(parameter is IWorkbenchContent activeContent))
          activeContent = Current.Workbench.ActiveContent;

        if (activeContent is IClipboardHandler editable)
        {
          editable.Paste();
        }
      }
    }
  }

  public class Delete : SimpleCommand
  {
    public override bool CanExecute(object parameter)
    {
      if (!(parameter is IWorkbenchContent activeContent))
        activeContent = Current.Workbench.ActiveContent;

      if (activeContent is IClipboardHandler editable)
      {
        return editable.EnableDelete;
      }
      return false;
    }

    public override void Execute(object parameter)
    {
      if (!(parameter is IWorkbenchContent activeContent))
        activeContent = Current.Workbench.ActiveContent;

      if (activeContent is IClipboardHandler editable)
      {
        editable.Delete();
      }
    }
  }
}
