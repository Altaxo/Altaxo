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
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Serialization;

namespace Altaxo.Gui.Graph.Gdi
{
  #region interfaces

  public interface IArrangeLayersView : IDataContextAwareView
  {
  }



  #endregion interfaces

  /// <summary>
  /// Controller for the <see cref="ArrangeLayersDocument" />.
  /// </summary>
  [UserControllerForObject(typeof(ArrangeLayersDocument))]
  [ExpectedTypeOfView(typeof(IArrangeLayersView))]
  public class ArrangeLayersController : MVCANControllerEditOriginalDocBase<ArrangeLayersDocument, IArrangeLayersView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _NumberOfRows;

    public int NumberOfRows
    {
      get => _NumberOfRows;
      set
      {
        if (!(_NumberOfRows == value))
        {
          _NumberOfRows = value;
          OnPropertyChanged(nameof(NumberOfRows));
        }
      }
    }

    private int _NumberOfColumns;

    public int NumberOfColumns
    {
      get => _NumberOfColumns;
      set
      {
        if (!(_NumberOfColumns == value))
        {
          _NumberOfColumns = value;
          OnPropertyChanged(nameof(NumberOfColumns));
        }
      }
    }

    private double _RowSpacing;

    public double RowSpacing
    {
      get => _RowSpacing;
      set
      {
        if (!(_RowSpacing == value))
        {
          _RowSpacing = value;
          OnPropertyChanged(nameof(RowSpacing));
        }
      }
    }

    private double _ColumnSpacing;

    public double ColumnSpacing
    {
      get => _ColumnSpacing;
      set
      {
        if (!(_ColumnSpacing == value))
        {
          _ColumnSpacing = value;
          OnPropertyChanged(nameof(ColumnSpacing));
        }
      }
    }

    private double _LeftMargin;

    public double LeftMargin
    {
      get => _LeftMargin;
      set
      {
        if (!(_LeftMargin == value))
        {
          _LeftMargin = value;
          OnPropertyChanged(nameof(LeftMargin));
        }
      }
    }

    private double _TopMargin;

    public double TopMargin
    {
      get => _TopMargin;
      set
      {
        if (!(_TopMargin == value))
        {
          _TopMargin = value;
          OnPropertyChanged(nameof(TopMargin));
        }
      }
    }




    private double _RightMargin;

    public double RightMargin
    {
      get => _RightMargin;
      set
      {
        if (!(_RightMargin == value))
        {
          _RightMargin = value;
          OnPropertyChanged(nameof(RightMargin));
        }
      }
    }

    private double _BottomMargin;

    public double BottomMargin
    {
      get => _BottomMargin;
      set
      {
        if (!(_BottomMargin == value))
        {
          _BottomMargin = value;
          OnPropertyChanged(nameof(BottomMargin));
        }
      }
    }

    public SelectableListNodeList SuperfluousLayers { get; } = new SelectableListNodeList();

    private SuperfluousLayersAction _SelectedSuperfluousLayersAction;

    public SuperfluousLayersAction SelectedSuperfluousLayersAction
    {
      get => _SelectedSuperfluousLayersAction;
      set
      {
        if (!(_SelectedSuperfluousLayersAction == value))
        {
          _SelectedSuperfluousLayersAction = value;
          OnPropertyChanged(nameof(SelectedSuperfluousLayersAction));
        }
      }
    }

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        NumberOfRows = _doc.NumberOfRows;
        NumberOfColumns = _doc.NumberOfColumns;
        RowSpacing = _doc.RowSpacing * 100;
        ColumnSpacing = _doc.ColumnSpacing * 100;
        LeftMargin = _doc.LeftMargin * 100;
        TopMargin = _doc.TopMargin * 100;
        RightMargin = _doc.RightMargin * 100;
        BottomMargin = _doc.BottomMargin * 100;
        Altaxo.Serialization.GUIConversion.GetListOfChoices(_doc.SuperfluousLayersAction);
        SelectedSuperfluousLayersAction = _doc.SuperfluousLayersAction;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.NumberOfRows = NumberOfRows;
      _doc.NumberOfColumns = NumberOfColumns;
      _doc.RowSpacing = RowSpacing;
      _doc.ColumnSpacing = ColumnSpacing;
      _doc.LeftMargin = LeftMargin;
      _doc.TopMargin = TopMargin;
      _doc.RightMargin = RightMargin;
      _doc.BottomMargin = BottomMargin;
      _doc.SuperfluousLayersAction = SelectedSuperfluousLayersAction;

      return ApplyEnd(true, disposeController);
    }
  }
}
