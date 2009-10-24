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
  /// Responsible for showing the notes of worksheets and graph windows.
  /// </summary>
  public class NotesController :
    ICSharpCode.SharpDevelop.Gui.IPadContent,
    ICSharpCode.SharpDevelop.Gui.IClipboardHandler
  {
    System.Windows.Forms.TextBox _view;

		/// <summary>The currently active view content to which the text belongs.</summary>
		object _currentActiveViewContent;

    public NotesController()
    {
      _view = new System.Windows.Forms.TextBox();
      _view.Multiline = true;
      _view.WordWrap=true;
			_view.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      _view.Font = new System.Drawing.Font(System.Drawing.FontFamily.GenericMonospace,8);
      _view.Dock = System.Windows.Forms.DockStyle.Fill;

			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += EhActiveWorkbenchWindowChanged;
			_view.Validating += EhTextValidating;
    }

		void EhActiveWorkbenchWindowChanged(object sender, EventArgs e)
		{
			StoreCurrentText(); // Saves the old text

			_currentActiveViewContent = Current.Workbench.ActiveViewContent;

			bool enable = true;
			if (_currentActiveViewContent is Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)
			{
				_view.Text = ((Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)_currentActiveViewContent).Controller.DataTable.Notes;
				// this. = "Notes for " + ((Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)obj).Controller.Doc.Name;
			}
			else if (_currentActiveViewContent is Altaxo.Gui.SharpDevelop.SDGraphViewContent)
			{
				_view.Text = ((Altaxo.Gui.SharpDevelop.SDGraphViewContent)_currentActiveViewContent).Controller.Doc.Notes;
			}
			else
			{
				_view.Text = string.Empty;
				enable = false;
			}

			_view.Enabled = enable;

			if (enable && _view.Text.Length > 0)
			{
					ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow ww = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
					WorkbenchSingleton.Workbench.WorkbenchLayout.ActivatePad(this.GetType().ToString());
					// now focus back to the formerly active workbench window.
					ww.SelectWindow();
			}
		}

		/// <summary>
		/// Fired when the user leaves the text window. Stores the text then back to the graph or worksheet.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void EhTextValidating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			StoreCurrentText();
		}

		/// <summary>
		/// Stores the text in the text control back to the graph document or worksheet.
		/// </summary>
		void StoreCurrentText()
		{
			if (null != _view)
			{
				if (_currentActiveViewContent is Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)
					((Altaxo.Gui.SharpDevelop.SDWorksheetViewContent)_currentActiveViewContent).Controller.DataTable.Notes = _view.Text;
				else if (_currentActiveViewContent is Altaxo.Gui.SharpDevelop.SDGraphViewContent)
					((Altaxo.Gui.SharpDevelop.SDGraphViewContent)_currentActiveViewContent).Controller.Doc.Notes = _view.Text;
			}
		}

    #region IPadContent Members

    public string Title
    {
      get
      {
        return ResourceService.GetString("MainWindow.Windows.AltaxoNotesWindowLabel");
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
