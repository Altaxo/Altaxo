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
using Altaxo.Collections;
using Altaxo.DataConnection;

namespace Altaxo.Gui.DataConnection
{
  public interface IEntireTableQueryView
  { /// <summary>Sets content of the tree view that shows the tables, views and stored procedures of a data base.</summary>
    /// <remarks>The image indices 0, 1, and 2 correspond to the nodes: Table , View, and Procedure.</remarks>
    void SetTreeSource(NGTreeNode rootNode);

    event Action ViewResults;

    /// <summary>
    /// Occurs when the selected tree node of the schema tree changed.
    /// </summary>
    event Action SelectedSchemaNodeChanged;
  }

  [ExpectedTypeOfView(typeof(IEntireTableQueryView))]
  public class EntireTableQueryController : IMVCAController
  {
    // max number of records shown on the preview dialog
    private const int MAX_PREVIEW_RECORDS = 5000;

    private IEntireTableQueryView _view;
    private AltaxoOleDbConnectionString _connectionString;
    private string _selectionStatement;

    private OleDbSchema _schema;

    private NGTreeNode _treeRootNode;

    public EntireTableQueryController()
    {
      _schema = new OleDbSchema();
      _treeRootNode = new NGTreeNode();
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
        else if (!object.Equals(value, oldValue))
        {
          _schema.ConnectionString = value.ConnectionStringWithTemporaryCredentials;
          UpdateTableTree();
        }
      }
    }

    public string SelectionStatement
    {
      get
      {
        return _selectionStatement;
      }
    }

    private void Initialize(bool initData)
    {
      if (initData)
      {
        if (null != _connectionString && !_connectionString.IsEmpty)
          _schema.ConnectionString = _connectionString.ConnectionStringWithTemporaryCredentials;
      }
      if (null != _view)
      {
        _view.SetTreeSource(_treeRootNode);
      }
    }

    private void Reset()
    {
      _treeRootNode.Nodes.Clear();
    }

    private void UpdateTableTree()
    {
      // initialize table tree
      var nodes = _treeRootNode.Nodes;
      nodes.Clear();
      var ndTables = new NGTreeNodeWithImageIndex() { Text = Current.ResourceService.GetString("Gui.DataConnection.Tables"), ImageIndex = 0, SelectedImageIndex = 0 };
      var ndViews = new NGTreeNodeWithImageIndex() { Text = Current.ResourceService.GetString("Gui.DataConnection.Views"), ImageIndex = 1, SelectedImageIndex = 1 };
      var ndProcs = new NGTreeNodeWithImageIndex() { Text = Current.ResourceService.GetString("Gui.DataConnection.StoredProcedures"), ImageIndex = 2, SelectedImageIndex = 2 };

      // populate using current schema
      if (_schema != null)
      {
        // populate the tree
        foreach (System.Data.DataTable dt in _schema.Tables)
        {
          // create new node, save table in tag property
          var node = new NGTreeNodeWithImageIndex() { Text = dt.TableName };
          node.Tag = dt;
          if (IsTableNameIdentical(dt.TableName, TableName))
          {
            node.IsExpanded = true;
            node.IsSelected = true;
          }

          // add new node to appropriate parent
          switch (OleDbSchema.GetTableType(dt))
          {
            case TableType.Table:
              ndTables.Nodes.Add(node);
              node.ImageIndex = node.SelectedImageIndex = 0;
              break;

            case TableType.View:
              ndViews.Nodes.Add(node);
              node.ImageIndex = node.SelectedImageIndex = 1;
              break;

            case TableType.Procedure:
              ndProcs.Nodes.Add(node);
              node.ImageIndex = node.SelectedImageIndex = 2;
              break;
          }
        }

        // add non-empty nodes to tree
        foreach (NGTreeNode nd in new NGTreeNode[] { ndTables, ndViews, ndProcs })
        {
          if (nd.Nodes.Count > 0)
          {
            nd.Text = string.Format("{0} ({1})", nd.Text, nd.Nodes.Count);
            nodes.Add(nd);
          }
        }

        // expand tables node
        ndTables.IsExpanded = true;

        // done
        if (null != _view)
        {
          _view.SetTreeSource(_treeRootNode);
        }
      }
    }

    private bool IsTableNameIdentical(string shortTableName, string fullTableName)
    {
      if (string.IsNullOrEmpty(fullTableName))
        return false;
      if (string.IsNullOrEmpty(shortTableName))
        return false;

      var idx = fullTableName.IndexOf(shortTableName);
      if (idx == 0 && fullTableName.Length == shortTableName.Length)
        return true;

      if (idx > 0 && (idx + shortTableName.Length) == fullTableName.Length && fullTableName[idx - 1] == '.')
        return true;

      return false;
    }

    private void EhSelectedSchemaNodeChanged()
    {
      _selectionStatement = string.Empty;
      var selNode = _treeRootNode.AnyBetweenHereAndLeaves(x => x.IsSelected);
      if (null == selNode)
        return;

      if (selNode.Tag is System.Data.DataTable)
        _selectionStatement = OleDbSchema.GetSelectStatement(selNode.Tag as System.Data.DataTable);

      //UpdateUI();
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

      // get table/view name
      var selNode = _treeRootNode.AnyBetweenHereAndLeaves(x => x.IsSelected);
      var table = selNode == null ? null : selNode.Tag as System.Data.DataTable;
      dt.TableName = table.TableName;

      // get view parameters if necessary
      var parms = OleDbSchema.GetTableParameters(table);
      if (parms != null && parms.Count > 0)
      {
        var ctrl = new ParametersController(parms);
        if (!Current.Gui.ShowDialog(ctrl, "Parameter", false))
        {
          return;
        }
      }

      // get data
      try
      {
        using (var da = new System.Data.OleDb.OleDbDataAdapter(_selectionStatement, ConnectionString.ConnectionStringWithTemporaryCredentials))
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

    private void AttachView()
    {
      _view.ViewResults += EhViewResults;
      _view.SelectedSchemaNodeChanged += EhSelectedSchemaNodeChanged;
    }

    private void DetachView()
    {
      _view.ViewResults -= EhViewResults;
      _view.SelectedSchemaNodeChanged -= EhSelectedSchemaNodeChanged;
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

        _view = value as IEntireTableQueryView;

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
      return !string.IsNullOrEmpty(_selectionStatement);
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

    public string TableName { get; set; }
  }
}
