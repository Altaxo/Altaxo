#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Data
{
  using System.ComponentModel;
  using Altaxo.Collections;
  using Altaxo.Data;

  public interface IConvertXYVToMatrixOptionsView : Altaxo.Gui.IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(ConvertXYVToMatrixOptions))]
  [ExpectedTypeOfView(typeof(IConvertXYVToMatrixOptionsView))]
  public class ConvertXYVToMatrixOptionsController : MVCANControllerEditOriginalDocBase<ConvertXYVToMatrixOptions, IConvertXYVToMatrixOptionsView>, INotifyPropertyChanged
  {
    private SelectableListNodeList _xColumnSorting;
    private SelectableListNodeList _yColumnSorting;
    private SelectableListNodeList _averaging;
    private SelectableListNodeList _columnNaming;
    private string _columnNameFormatString;
    private bool _useClusteringForX;
    private bool _useClusteringForY;
    private int? _numberOfClustersX;
    private int? _numberOfClustersY;
    private bool _createStdDevX;
    private bool _createStdDevY;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      base.Dispose(isDisposing);
    }

    #region ViewModel

    public SelectableListNodeList ColumnXSorting { get { return _xColumnSorting; } set { _xColumnSorting = value; OnPropertyChanged(nameof(ColumnXSorting)); } }
    public SelectableListNodeList ColumnYSorting { get { return _yColumnSorting; } set { _yColumnSorting = value; OnPropertyChanged(nameof(ColumnYSorting)); } }
    public SelectableListNodeList Averaging { get { return _averaging; } set { _averaging = value; OnPropertyChanged(nameof(Averaging)); } }
    public SelectableListNodeList ColumnNaming { get { return _columnNaming; } set { _columnNaming = value; OnPropertyChanged(nameof(ColumnNaming)); OnPropertyChanged(nameof(IsColumnNameFormatStringEnabled)); } }
    public bool IsColumnNameFormatStringEnabled { get { return (ConvertXYVToMatrixOptions.OutputNaming?)(_columnNaming.FirstSelectedNode?.Tag) == ConvertXYVToMatrixOptions.OutputNaming.FormatString; } }
    public string ColumnNameFormatString { get { return _columnNameFormatString; } set { _columnNameFormatString = value; OnPropertyChanged(nameof(ColumnNameFormatString)); } }


    public bool UseClusteringForX { get { return _useClusteringForX; } set { _useClusteringForX = value; OnPropertyChanged(nameof(UseClusteringForX)); } }
    public bool UseClusteringForY { get { return _useClusteringForY; } set { _useClusteringForY = value; OnPropertyChanged(nameof(UseClusteringForY)); } }

    public int? NumberOfClustersX { get { return _numberOfClustersX; } set { _numberOfClustersX = value; OnPropertyChanged(nameof(NumberOfClustersX)); } }
    public int? NumberOfClustersY { get { return _numberOfClustersY; } set { _numberOfClustersY = value; OnPropertyChanged(nameof(NumberOfClustersY)); } }

    public bool CreateStdDevX { get { return _createStdDevX; } set { _createStdDevX = value; OnPropertyChanged(nameof(CreateStdDevX)); } }
    public bool CreateStdDevY { get { return _createStdDevY; } set { _createStdDevY = value; OnPropertyChanged(nameof(CreateStdDevY)); } }


    #endregion

    protected override void AttachView()
    {

      base.AttachView();

      if (_view is not null)
        _view.DataContext = this;
    }

    protected override void DetachView()
    {
      if (_view is not null)
        _view.DataContext = null;

      base.DetachView();
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _xColumnSorting = new SelectableListNodeList();
        _xColumnSorting.FillWithEnumeration(_doc.DestinationXColumnSorting);
        _yColumnSorting = new SelectableListNodeList();
        _yColumnSorting.FillWithEnumeration(_doc.DestinationYColumnSorting);

        _averaging = new SelectableListNodeList();
        _averaging.FillWithEnumeration(_doc.ValueAveraging);
        _columnNaming = new SelectableListNodeList();
        _columnNaming.FillWithEnumeration(_doc.ColumnNaming);
        _columnNameFormatString = _doc.ColumnNameFormatString;

        _useClusteringForX = _doc.UseClusteringForX;
        _useClusteringForY = _doc.UseClusteringForY;
        _numberOfClustersX = _doc.NumberOfClustersX;
        _numberOfClustersY = _doc.NumberOfClustersY;
        _createStdDevX = _doc.CreateStdDevX;
        _createStdDevY = _doc.CreateStdDevY;
      }
      if (_view is not null)
      {
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.DestinationXColumnSorting = (SortDirection)_xColumnSorting.FirstSelectedNode.Tag;
      _doc.DestinationYColumnSorting = (SortDirection)_yColumnSorting.FirstSelectedNode.Tag;
      _doc.ValueAveraging = (ConvertXYVToMatrixOptions.OutputAveraging)_averaging.FirstSelectedNode.Tag;
      _doc.ColumnNaming = (ConvertXYVToMatrixOptions.OutputNaming)_columnNaming.FirstSelectedNode.Tag;
      _doc.ColumnNameFormatString = _columnNameFormatString;

      _doc.UseClusteringForX = _useClusteringForX;
      _doc.UseClusteringForY = _useClusteringForY;
      _doc.NumberOfClustersX = _numberOfClustersX;
      _doc.NumberOfClustersY = _numberOfClustersY;
      _doc.CreateStdDevX = _createStdDevX;
      _doc.CreateStdDevY = _createStdDevY;
      return ApplyEnd(true, disposeController);
    }
  }
}
