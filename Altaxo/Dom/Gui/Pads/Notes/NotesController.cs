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

#nullable disable warnings
using System;
using System.ComponentModel;
using Altaxo.Gui.Workbench;
using Altaxo.Main;

namespace Altaxo.Gui.Pads.Notes
{
  public interface INotesView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Responsible for showing the notes of worksheets and graph windows.
  /// </summary>
  [ExpectedTypeOfView(typeof(INotesView))]
  public class NotesController : AbstractPadContent
  {
    private INotesView _view;
    private WeakReference _currentlyActiveNotes;
    WeakPropertyChangedEventHandler _notesPropertyChangedHandler;


    public NotesController()
    {
      Current.Workbench.ActiveViewContentChanged += new WeakEventHandler(EhWorkbenchViewContentChanged, Current.Workbench, nameof(Current.Workbench.ActiveViewContentChanged));
    }

    #region Bindings


    public ITextBackedConsole CurrentlyActiveNotes
    {
      get
      {
        return _currentlyActiveNotes?.Target as ITextBackedConsole;
      }
      set
      {
        if (!object.ReferenceEquals(CurrentlyActiveNotes, value))
        {
          // unsuscribe from notes property changed event
          if (CurrentlyActiveNotes is { } notesOld && _notesPropertyChangedHandler is { } nph)
          {
            notesOld.PropertyChanged -= nph;
            _notesPropertyChangedHandler = null;
          }

          _currentlyActiveNotes = value is null ? null : new WeakReference(value);

          if (value is { } notesNew)
          {
            _notesPropertyChangedHandler = new WeakPropertyChangedEventHandler(EhNotes_PropertyChanged, notesNew, nameof(PropertyChanged));
            notesNew.PropertyChanged += _notesPropertyChangedHandler;
          }

          OnPropertyChanged(nameof(CurrentlyActiveNotes));
          OnPropertyChanged(nameof(Text));
          OnPropertyChanged(nameof(IsEnabled));
        }
      }
    }

    public bool IsEnabled
    {
      get
      {
        return CurrentlyActiveNotes is not null;
      }
    }

    public string Text
    {
      get
      {
        return CurrentlyActiveNotes?.Text ?? string.Empty;
      }
      set
      {
        if (CurrentlyActiveNotes is { } notes)
          notes.Text = value;
      }
    }


    private void EhNotes_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Text))
      {
        OnPropertyChanged(nameof(Text));
      }
    }

    #endregion

    private void EhWorkbenchViewContentChanged(object sender, EventArgs e)
    {
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
        case Altaxo.Gui.Text.Viewing.TextDocumentController ctrl4:
          notes = ctrl4.TextDocument.Notes;
          break;
        default:
          notes = null;
          break;
      }

      CurrentlyActiveNotes = notes;

      if (notes is not null && notes.Text.Length > 0)
      {
        var activeContent = Current.Workbench?.ActiveContent;

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

    #region IPadContent Members

    private void Initialize(bool initData)
    {
      if (initData)
      {
        EhWorkbenchViewContentChanged(this, EventArgs.Empty);
      }
    }

    private void AttachView()
    {
      if (_view is { } view)
      {
        view.DataContext = this;
      }
    }

    private void DetachView()
    {
      if (_view is { } view)
      {
        _view.DataContext = null;
      }
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
