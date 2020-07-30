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

#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Collections;
using Altaxo.Gui.Workbench;
using Altaxo.Main;
using Altaxo.Worksheet;

namespace Altaxo.Gui.Worksheet.Viewing
{
  [UserControllerForObject(typeof(WorksheetLayout))]
  [UserControllerForObject(typeof(WorksheetViewLayout))]
  [ExpectedTypeOfView(typeof(IWorksheetView))]
  public partial class WorksheetController : AbstractViewContent, IWorksheetController, IDisposable
  {
    /// <summary>Holds the data table cached from the layout.</summary>
    protected Altaxo.Data.DataTable _table;

    protected Altaxo.Worksheet.WorksheetLayout _worksheetLayout;

    protected WeakEventHandler? _weakEventHandlerForTable_Changed;
    protected WeakActionHandler<object, object, TunnelingEventArgs>? _weakEventHandlerForTable_TunneledEvent;
    protected WeakActionHandler<object, object, TunnelingEventArgs>? _weakEventHandlerForLayout_TunneledEvent;


    private IWorksheetView? _view;

    #region Constructors

    public WorksheetController(Altaxo.Data.DataTable table)
      : this(new Altaxo.Worksheet.WorksheetLayout(table ?? throw new ArgumentNullException(nameof(table))))
    {
      // we have created a new WorksheetLayout in the constructor above
      // it is still not part of Altaxo, so we need to register it
      Current.Project.TableLayouts.Add(WorksheetLayout);
    }

    public WorksheetController(WorksheetViewLayout viewLayout)
    : this(viewLayout?.WorksheetLayout ?? throw new ArgumentNullException(nameof(viewLayout)))
    {
    }

    /// <summary>
    /// Creates a WorksheetController which shows the table data using the specified <paramref name="layout"/>.
    /// </summary>
    /// <param name="layout">The worksheet layout.</param>
    public WorksheetController(Altaxo.Worksheet.WorksheetLayout layout)
    {
      SetMemberVariablesToDefault();
      WorksheetLayout = layout ?? throw new ArgumentNullException("Leaving the layout null in constructor is not supported here");
    }

    public bool InitializeDocument(params object[] args)
    {
      if (null == args || args.Length == 0)
        return false;
      if (args[0] is WorksheetLayout)
        WorksheetLayout = (WorksheetLayout)args[0];
      else if (args[0] is WorksheetViewLayout)
        WorksheetLayout = ((WorksheetViewLayout)args[0]).WorksheetLayout;
      else
        return false;

      return true;
    }

    public UseDocument UseDocumentCopy
    {
      set { }
    }

    [MemberNotNull(nameof(_worksheetLayout), nameof(_table))]
    protected virtual void InternalInitializeWorksheetLayout(WorksheetLayout value)
    {
      if (null != _worksheetLayout)
        throw new ApplicationException("This controller is already controlling a layout");
      if (null != _table)
        throw new ApplicationException("This controller is already controlling a table");
      if (null == value)
        throw new ArgumentNullException("value");
      if (null == value.DataTable)
        throw new ApplicationException("The DataTable of the WorksheetLayout is null");

      _worksheetLayout = value;
      _table = _worksheetLayout.DataTable;

      var table = _table;

      {
        // use local variable for anonymous method below in order to avoid references to the controller instance
        _weakEventHandlerForTable_Changed?.Remove();
        table.Changed += (_weakEventHandlerForTable_Changed = new WeakEventHandler(EhTable_Changed, table, nameof(table.Changed)));

        _weakEventHandlerForTable_TunneledEvent?.Remove();
        table.TunneledEvent += (_weakEventHandlerForTable_TunneledEvent = new WeakActionHandler<object, object, TunnelingEventArgs>(EhTable_TunneledEvent, table, nameof(table.TunneledEvent)));
      }

      var layout = _worksheetLayout;
      {
        _weakEventHandlerForLayout_TunneledEvent?.Remove();
        // use local variable for anonymous method below in order to avoid references to the controller instance
        layout.TunneledEvent += (_weakEventHandlerForLayout_TunneledEvent = new WeakActionHandler<object, object, TunnelingEventArgs>(EhLayout_TunneledEvent, layout, nameof(layout.TunneledEvent)));
      }



      Title = _table.Name;

      var dataColumns = _table.DataColumns;
      var propColumns = _table.PropCols;
      {
        _weakEventHandlerDataColumnChanged?.Remove();
        dataColumns.Changed += (_weakEventHandlerDataColumnChanged = new WeakEventHandler(EhTableDataChanged, dataColumns, nameof(dataColumns.Changed)));

        _weakEventHandlerPropertyColumnChanged?.Remove();
        propColumns.Changed += (_weakEventHandlerPropertyColumnChanged = new WeakEventHandler(EhPropertyDataChanged, propColumns, nameof(propColumns.Changed)));
      }

      SetCachedNumberOfDataColumns();
      SetCachedNumberOfDataRows();
      SetCachedNumberOfPropertyColumns();
    }

    public override void Dispose()
    {
      var view = _view;
      ViewObject = null;
      HideCellEditControl();
      (view as IDisposable)?.Dispose();

      _weakEventHandlerForLayout_TunneledEvent?.Remove();

      _weakEventHandlerForTable_Changed?.Remove();
      _weakEventHandlerForTable_TunneledEvent?.Remove();

      _weakEventHandlerDataColumnChanged?.Remove();
      _weakEventHandlerPropertyColumnChanged?.Remove();

      _table = null!;
      _worksheetLayout = null!; // removes also the event handler(s)
    }

    #endregion Constructors

    #region IWorksheetController Members

    public Altaxo.Data.DataTable DataTable
    {
      get
      {
        return _table;
      }
    }

    public void TableAreaInvalidate()
    {
      _view?.TableArea_TriggerRedrawing();
    }

    public WorksheetLayout WorksheetLayout
    {
      get { return _worksheetLayout; }

      [MemberNotNull(nameof(_table), nameof(_worksheetLayout))]
      set
      {
        InternalInitializeWorksheetLayout(value);
      }
    }

    public void EhTable_Changed(object? sender, EventArgs e)
    {
      if (e is Altaxo.Main.NamedObjectCollectionChangedEventArgs eAsCCEA && object.ReferenceEquals(eAsCCEA.Item, _table))
      {
        if (eAsCCEA.WasItemRenamed)
        {
          var owner = (INameOwner)eAsCCEA.Item;
          Title = owner.Name;
        }
      }
    }

    private void EhTable_TunneledEvent(object sender, object originalSource, TunnelingEventArgs e)
    {
      if (e is DisposeEventArgs && object.ReferenceEquals(originalSource, _table))
      {
        Current.ProjectService.RemoveWorksheet(this);
      }
    }

    private void EhLayout_TunneledEvent(object sender, object originalSource, TunnelingEventArgs e)
    {
      if (e is DisposeEventArgs && object.ReferenceEquals(originalSource, _worksheetLayout))
      {
        Current.ProjectService.RemoveWorksheet(this);
      }
    }


    #endregion IWorksheetController Members

    #region IMVCController Members

    private void AttachView()
    {
      _view!.Controller = this;
      _view.CellEdit_PreviewKeyPressed += EhCellEditControl_PreviewKeyDown;
      _view.CellEdit_LostFocus += EhCellEditControl_LostFocus;
      _view.CellEdit_TextChanged += EhCellEditControl_TextChanged;
    }

    private void DetachView()
    {
      _view!.CellEdit_PreviewKeyPressed -= EhCellEditControl_PreviewKeyDown;
      _view.CellEdit_LostFocus -= EhCellEditControl_LostFocus;
      _view.CellEdit_TextChanged -= EhCellEditControl_TextChanged;
      _view.Controller = null;
    }

    public override object? ViewObject
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
          {
            DetachView();
          }

          _view = value as IWorksheetView;

          if (null != _view)
          {
            AttachView();

            // Werte für gerade vorliegende Scrollpositionen und Scrollmaxima zum (neuen) View senden
            VertScrollMaximum = _scrollVertMax;
            HorzScrollMaximum = _scrollHorzMax;

            VerticalScrollPosition = _scrollVertPos;
            HorzScrollPos = _scrollHorzPos;

            // Simulate a SizeChanged event
            EhView_TableAreaSizeChanged(new EventArgs());
          }
        }
      }
    }

    public override object ModelObject
    {
      get
      {
        return new WorksheetViewLayout(_worksheetLayout);
      }
    }

    public bool Apply(bool disposeController)
    {
      return true;
    }

    /// <summary>
    /// Try to revert changes to the model, i.e. restores the original state of the model.
    /// </summary>
    /// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
    /// <returns>
    ///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
    /// </returns>
    public bool Revert(bool disposeController)
    {
      return false;
    }

    #endregion IMVCController Members
  }
}
