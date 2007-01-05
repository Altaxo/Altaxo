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

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;


namespace Altaxo.Gui.Pads
{
  /// <summary>
  /// Controls the data display window pad that shows the data obtained from the data reader.
  /// </summary>
  public class DataDisplayController :
    ICSharpCode.SharpDevelop.Gui.IPadContent, 
    Altaxo.Main.Services.IDataDisplayService,
    ICSharpCode.SharpDevelop.Gui.IClipboardHandler
  {
    System.Windows.Forms.TextBox _view;
  


    public DataDisplayController()
    {
      _view = new System.Windows.Forms.TextBox();
      _view.Multiline = true;
      _view.WordWrap=false;
      _view.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      _view.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace,8);
      _view.Dock = System.Windows.Forms.DockStyle.Fill;

      Current.SetDataDisplayService(this);
    }


    #region IPadContent Members

    public string Title
    {
      get
      {
        return ResourceService.GetString("MainWindow.Windows.AltaxoDataDisplayWindowLabel");
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

    string category;
    public string Category 
    {
      get 
      {
        return category;
      }
      set
      {
        category = value;
      }
    }
    string[] shortcut; // TODO: Inherit from AbstractPadContent
    public string[] Shortcut 
    {
      get 
      {
        return shortcut;
      }
      set 
      {
        shortcut = value;
      }
    }

    /*
    public void BringPadToFront()
    {
      if (!WorkbenchSingleton.Workbench.WorkbenchLayout.IsVisible(this)) 
      {
        WorkbenchSingleton.Workbench.WorkbenchLayout.ShowPad(this);
      }
      WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this);
    }
    */

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

    #region IDataDisplayService Members

    /// <summary>Writes a string to the output.</summary>
    /// <param name="text">The text to write to the output.</param>
    public void WriteOneLine(string text)
    {
      _view.Text = text;
      if(!_view.Visible || _view.Parent==null)
      {
        ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow ww = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
        WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this.GetType().ToString());
        // now focus back to the formerly active workbench window.
        ww.SelectWindow();
       // ww.ActiveViewContent.Control.Focus();

      }
    }

    /// <summary>
    /// Writes two lines to the window.
    /// </summary>
    /// <param name="line1">First line.</param>
    /// <param name="line2">Second line.</param>
    public void WriteTwoLines(string line1, string line2)
    {
      _view.Text = line1 + System.Environment.NewLine + line2;
    }

    /// <summary>
    /// Writes three lines to the output.
    /// </summary>
    /// <param name="line1">First line.</param>
    /// <param name="line2">Second line.</param>
    /// <param name="line3">Three line.</param>
    public void WriteThreeLines(string line1, string line2, string line3)
    {
      _view.Text = line1 + System.Environment.NewLine + line2 + System.Environment.NewLine + line3;
    }
    #endregion

    #region IClipboardHandler Members

    public bool EnableCut
    {
      get { return _view.SelectionLength > 0; }
    }

    public bool EnableCopy
    {
      get { return _view.SelectionLength > 0; }
    }

    public bool EnablePaste
    {
      get { return true; }
    }

    public bool EnableDelete
    {
      get { return _view.SelectionLength > 0; }
    }

    public bool EnableSelectAll
    {
      get { return true; }
    }

    public void Cut()
    {
      _view.Cut();
    }

    public void Copy()
    {
      _view.Copy();
    }

    public void Paste()
    {
      _view.Paste();
    }

    public void Delete()
    {
      int start = _view.SelectionStart;
      int len = _view.SelectionLength;
      if (len > 0)
        _view.Text = _view.Text.Substring(0, start) + _view.Text.Substring(start + len);

    }

    public void SelectAll()
    {
      _view.SelectAll();
    }

    #endregion
  }
}
