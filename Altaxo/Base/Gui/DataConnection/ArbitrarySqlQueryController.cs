#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.DataConnection;

namespace Altaxo.Gui.DataConnection
{
  public interface IArbitrarySqlQueryView
  {
    event Action CheckSql;

    event Action ViewResults;

    event Action ClearQuery;

    event Action SqlTextChanged;

    string SqlText { get; set; }

    /// <summary>
    /// Updates the status of Gui items according to the provided parameters;
    /// </summary>
    /// <param name="isConnectionStringEmpty">If set to <c>true</c>, the connection string is empty.</param>
    /// <param name="isSelectionStatementEmpty">If set to <c>true</c>, the selection statement is empty.</param>
    void UpdateStatus(bool isConnectionStringEmpty, bool isSelectionStatementEmpty);
  }

  [ExpectedTypeOfView(typeof(IArbitrarySqlQueryView))]
  public class ArbitrarySqlQueryController : IMVCAController
  {
    // max number of records shown on the preview dialog
    private const int MAX_PREVIEW_RECORDS = 5000;

    private IArbitrarySqlQueryView _view;

    private AltaxoOleDbConnectionString _connectionString;
    private string _selectionStatement;
    private OleDbSchema _schema;

    public ArbitrarySqlQueryController()
    {
      _schema = new OleDbSchema();
    }

    /// <summary>
    /// Gets or sets the connection string that represents the underlying database.
    /// </summary>
    public AltaxoOleDbConnectionString ConnectionString
    {
      get { return _connectionString; }
      set
      {
        var oldValue = _connectionString;

        _connectionString = value;

        if (null == _connectionString || _connectionString.IsEmpty)
        {
          Reset();
        }
        else if (!object.Equals(oldValue, value))
        {
          _schema.ConnectionString = value.ConnectionStringWithTemporaryCredentials;
        }
      }
    }

    public string SelectionStatement
    {
      get
      {
        return _selectionStatement;
      }
      set
      {
        var oldValue = _selectionStatement;
        _selectionStatement = value;

        if (null != _view)
        {
          _view.SqlText = _selectionStatement;
        }
      }
    }

    private void Reset()
    {
      SelectionStatement = string.Empty;
    }

    private void Initialize(bool initData)
    {
      if (initData)
      {
      }
      if (null != _view)
      {
        _view.SqlText = _selectionStatement;
      }
    }

    private void EhClearQuery()
    {
      SelectionStatement = string.Empty;
    }

    private void EhViewResults()
    {
      // make sure we have a select statement
      var sql = _selectionStatement;
      if (string.IsNullOrEmpty(sql))
      {
        return;
      }

      // create table to load with data and display
      var dt = new System.Data.DataTable("Query");

      // get data
      try
      {
        using (var da = new System.Data.OleDb.OleDbDataAdapter(SelectionStatement, ConnectionString.ConnectionStringWithTemporaryCredentials))
        {
          // get data
          da.Fill(0, MAX_PREVIEW_RECORDS, dt);

          // show the data
          var ctrl = new DataPreviewController(dt);
          string title = string.Format("{0} ({1:n0} records)", dt.TableName, dt.Rows.Count);
          Current.Gui.ShowDialog(ctrl, title, false);
        }
      }
      catch (Exception x)
      {
        Current.Gui.ErrorMessageBox(string.Format("Failed to retrieve data:\r\n{0}", x.Message));
      }
    }

    private void EhCheckSql()
    {
      try
      {
        var da = new System.Data.OleDb.OleDbDataAdapter(SelectionStatement, ConnectionString.ConnectionStringWithTemporaryCredentials);
        var dt = new System.Data.DataTable();
        da.FillSchema(dt, System.Data.SchemaType.Mapped);
        Current.Gui.InfoMessageBox(

            "The SQL syntax has been verified against the data source.",
            "Success");
      }
      catch (Exception x)
      {
        var msg = string.Format("Failed to retrieve the data:\r\n{0}", x.Message);
        Current.Gui.ErrorMessageBox(msg, "Warning");
      }
    }

    private void EhSqlTextChanged()
    {
      SelectionStatement = _view.SqlText;
    }

    private void AttachView()
    {
      _view.CheckSql += EhCheckSql;
      _view.ViewResults += EhViewResults;
      _view.ClearQuery += EhClearQuery;
      _view.SqlTextChanged += EhSqlTextChanged;
    }

    private void DetachView()
    {
      _view.CheckSql -= EhCheckSql;
      _view.ViewResults -= EhViewResults;
      _view.ClearQuery -= EhClearQuery;
      _view.SqlTextChanged -= EhSqlTextChanged;
    }

    public object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (null != _view)
        {
          DetachView();
        }

        _view = value as IArbitrarySqlQueryView;

        if (null != _view)
        {
          Initialize(false);
          AttachView();
        }
      }
    }

    public object ModelObject
    {
      get { throw new NotImplementedException(); }
    }

    public void Dispose()
    {
      ViewObject = null;
    }

    public bool Apply(bool disposeController)
    {
      return null != _connectionString && !_connectionString.IsEmpty && !string.IsNullOrEmpty(_selectionStatement);
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
  }
}
