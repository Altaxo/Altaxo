#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui.Pads;

namespace Altaxo.Main.GUI
{
  /// <summary>
  /// Controls the Output window pad which shows the Altaxo text output.
  /// </summary>
  public class OutputWindowController : ICSharpCode.SharpDevelop.Gui.IPadContent, Altaxo.Main.Services.IOutputService
  {
    System.Windows.Forms.TextBox _view;
    ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(ResourceService));


    public OutputWindowController()
    {
      _view = new System.Windows.Forms.TextBox();
      _view.Multiline = true;
      _view.WordWrap=false;
      _view.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      _view.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace,8);
      _view.Dock = System.Windows.Forms.DockStyle.Fill;

      Current.SetOutputService(this);
    }


    #region IPadContent Members

    public string Title
    {
      get
      {
        return resourceService.GetString("MainWindow.Windows.AltaxoOutputWindowLabel");
      }
    }

    public System.Windows.Forms.Control Control
    {
      get
      {
        return this._view;
      }
    }

    public void BringToFront()
    {
      _view.BringToFront();
    }

    public string Icon
    {
      get
      {
        return "Icons.16x16.OpenFolderBitmap";
      }
    }

    public event System.EventHandler TitleChanged;

    public event System.EventHandler IconChanged;

    public void RedrawContent()
    {
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      if(_view!=null)
      {
        _view.Dispose();
        _view = null;
      }
    }

    #endregion

    #region IOutputService Members

    public void WriteLine()
    {
      _view.AppendText(System.Environment.NewLine);
    }

    public void WriteLine(string text)
    {
      Write(text);
      WriteLine();
    }

    public void WriteLine(string format, params object[] args)
    {
      Write(format,args);
      WriteLine();
    }

    public void WriteLine(System.IFormatProvider provider, string format, params object[] args)
    {
      Write(string.Format(provider,format,args));
      WriteLine();
    }
    public void Write(string text)
    {
      _view.AppendText(text);
    }

    public void Write(string format, params object[] args)
    {
      Write(string.Format(format,args));
    }

    public void Write(System.IFormatProvider provider, string format, params object[] args)
  {
    Write(string.Format(provider,format,args));
  }

    #endregion
  }
}
