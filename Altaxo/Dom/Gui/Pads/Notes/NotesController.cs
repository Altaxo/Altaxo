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

    /// <summary>
    /// Sets a binding from the <see cref="ITextBackedConsole"/> to the control.
    /// The binding should update every time the text is changed (not only if the focus is lost).
    /// </summary>
    /// <param name="con">The console.</param>
    void SetTextFromNotesAndSetBinding(Altaxo.Main.ITextBackedConsole con);

    bool IsEnabled { get; set; }
  }

  /// <summary>
  /// Responsible for showing the notes of worksheets and graph windows.
  /// </summary>
  [ExpectedTypeOfView(typeof(INotesView))]
  public class NotesController : AbstractPadContent
  {
    private INotesView _view;
    private WeakReference _currentlyActiveNotes;

    public NotesController()
    {
      Current.Workbench.ActiveViewContentChanged += new WeakEventHandler(EhWorkbenchViewContentChanged, Current.Workbench, nameof(Current.Workbench.ActiveViewContentChanged));
    }

    private void EhWorkbenchViewContentChanged(object sender, EventArgs e)
    {
      SaveTextBoxTextToNotes(); // Saves the old text

      if (_view is { } view) // _view==null can happen during shutdown
      {
        // Clears the old binding
        view.ClearBinding();

        ITextBackedConsole notes;

        switch (Current.Workbench?.ActiveViewContent)
        {
          case Altaxo.Gui.Worksheet.Viewing.WorksheetController ctrl1:
            notes = ctrl1.DataTable.Notes;
            break;
          case Altaxo.Gui.Graph.Gdi.Viewing.GraphController ctrl2:
            notes = ctrl2.Doc.Notes;
            break;
          case Altaxo.Gui.Graph.Graph3D.Viewing.Graph3DController ctrl3:
            notes = ctrl3.Doc.Notes;
            break;
          default:
            notes = null;
            break;
        }

        if (notes is { } _)
        {
          _view.SetTextFromNotesAndSetBinding(notes);
          _currentlyActiveNotes = new WeakReference(notes);
        }
        else
        {
          _view.Text = string.Empty;
          _currentlyActiveNotes = null;
        }

        _view.IsEnabled = !(_currentlyActiveNotes is null);

        if (!(_currentlyActiveNotes is null) && _view.Text.Length > 0)
        {
          var activeContent = Current.Workbench.ActiveContent;

          IsActive = true;
          IsSelected = true;

          // now focus back to the formerly active workbench window.
          if (activeContent is not null)
          {
            activeContent.IsActive = true;
            activeContent.IsSelected = true;
          }
        }
      }
    }

    /// <summary>
    /// Stores the text in the text control back to the graph document or worksheet.
    /// </summary>
    private void SaveTextBoxTextToNotes()
    {
      if (_view is { } view && _currentlyActiveNotes?.Target is ITextBackedConsole notes)
      {
        notes.Text = view.Text;
      }
    }

    #region IPadContent Members

    private void Initialize(bool initData)
    {
    }

    private void AttachView()
    {
      EhWorkbenchViewContentChanged(this, EventArgs.Empty);
    }

    private void DetachView()
    {
      _view?.ClearBinding();
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
          if (_view is not null)
            DetachView();

          _view = value as INotesView;

          if (_view is not null)
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
