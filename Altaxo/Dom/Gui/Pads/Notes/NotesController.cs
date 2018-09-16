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

using System;
using System.ComponentModel;
using Altaxo.Gui.Workbench;
using Altaxo.Main;

namespace Altaxo.Gui.Pads.Notes
{
  public interface INotesView
  {
    string Text { get; set; }

    void ClearBinding();

    void SetTextFromNotesAndSetBinding(Altaxo.Main.ITextBackedConsole con);

    bool IsEnabled { get; set; }

    event Action ShouldSaveText;
  }

  /// <summary>
  /// Responsible for showing the notes of worksheets and graph windows.
  /// </summary>
  [ExpectedTypeOfView(typeof(INotesView))]
  public class NotesController : AbstractPadContent
  {
    private INotesView _view;

    /// <summary>The currently active view content to which the text belongs.</summary>
    private WeakReference _currentActiveViewContent = new WeakReference(null);

    public NotesController()
    {
      Current.Workbench.ActiveViewContentChanged += new WeakEventHandler(EhWorkbenchViewContentChanged, handler => Current.Workbench.ActiveViewContentChanged -= handler).EventSink;
    }

    private void EhWorkbenchViewContentChanged(object sender, EventArgs e)
    {
      SaveTextBoxTextToNotes(); // Saves the old text

      if (null == _view)
        return; // can happen during shutdown

      // Clears the old binding
      _view.ClearBinding();

      _currentActiveViewContent = new WeakReference(Current.Workbench.ActiveViewContent);

      bool enable = true;

      if (_currentActiveViewContent.Target is Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl1)
      {
        _view.SetTextFromNotesAndSetBinding(ctrl1.DataTable.Notes);
      }
      else if (_currentActiveViewContent.Target is Altaxo.Gui.Graph.Gdi.Viewing.GraphController ctrl2)
      {
        _view.SetTextFromNotesAndSetBinding(ctrl2.Doc.Notes);
      }
      else if (_currentActiveViewContent.Target is Altaxo.Gui.Graph.Graph3D.Viewing.Graph3DController ctrl3)
      {
        _view.SetTextFromNotesAndSetBinding(ctrl3.Doc.Notes);
      }
      else
      {
        _view.Text = string.Empty;
        enable = false;
      }

      _view.IsEnabled = enable;

      if (enable && _view.Text.Length > 0)
      {
        var activeContent = Current.Workbench.ActiveContent;

        IsActive = true;
        IsSelected = true;

        // now focus back to the formerly active workbench window.
        if (null != activeContent)
        {
          activeContent.IsActive = true;
          activeContent.IsSelected = true;
        }
      }
    }

    /// <summary>
    /// Stores the text in the text control back to the graph document or worksheet.
    /// </summary>
    private void SaveTextBoxTextToNotes()
    {
      if (null != _view)
      {
        ITextBackedConsole notes = null;
        if (_currentActiveViewContent.Target is Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl1)
        {
          notes = ctrl1.DataTable?.Notes;
        }
        else if (_currentActiveViewContent.Target is Altaxo.Gui.Graph.Gdi.Viewing.GraphController ctrl2)
        {
          notes = ctrl2.Doc?.Notes;
        }
        else if (_currentActiveViewContent.Target is Altaxo.Gui.Graph.Graph3D.Viewing.Graph3DController ctrl3)
        {
          notes = ctrl3.Doc?.Notes;
        }
        if (notes != null)
          notes.Text = _view.Text;
      }
    }

    #region IPadContent Members

    private void Initialize(bool initData)
    {
    }

    private void AttachView()
    {
      _view.ShouldSaveText += SaveTextBoxTextToNotes;
    }

    private void DetachView()
    {
      _view.ShouldSaveText -= SaveTextBoxTextToNotes;
    }

    public override object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (!object.ReferenceEquals(_view, value))
        {
          if (null != _view)
            DetachView();

          _view = value as INotesView;

          if (null != _view)
          {
            Initialize(false);
            AttachView();
          }
        }
      }
    }

    public override object ModelObject => null;

    #endregion IPadContent Members
  }
}
