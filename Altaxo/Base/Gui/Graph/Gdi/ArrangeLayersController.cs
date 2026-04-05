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
using Altaxo.Gui.Common;
using Altaxo.Serialization;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi
{
  /// <summary>
  /// View contract for arranging graph layers.
  /// </summary>
  public interface IArrangeLayersView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for the <see cref="ArrangeLayersDocument" />.
  /// </summary>
  [UserControllerForObject(typeof(ArrangeLayersDocument))]
  [ExpectedTypeOfView(typeof(IArrangeLayersView))]
  public class ArrangeLayersController : MVCANControllerEditOriginalDocBase<ArrangeLayersDocument, IArrangeLayersView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _numberOfRows;

    /// <summary>
    /// Gets or sets the number of rows.
    /// </summary>
    public int NumberOfRows
    {
      get => _numberOfRows;
      set
      {
        if (!(_numberOfRows == value))
        {
          _numberOfRows = value;
          OnPropertyChanged(nameof(NumberOfRows));
        }
      }
    }

    private int _numberOfColumns;

    /// <summary>
    /// Gets or sets the number of columns.
    /// </summary>
    public int NumberOfColumns
    {
      get => _numberOfColumns;
      set
      {
        if (!(_numberOfColumns == value))
        {
          _numberOfColumns = value;
          OnPropertyChanged(nameof(NumberOfColumns));
        }
      }
    }

    /// <summary>
    /// Gets the spacing environment.
    /// </summary>
    public QuantityWithUnitGuiEnvironment SpacingEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _rowSpacing;

    /// <summary>
    /// Gets or sets the row spacing.
    /// </summary>
    public DimensionfulQuantity RowSpacing
    {
      get => _rowSpacing;
      set
      {
        if (!(_rowSpacing == value))
        {
          _rowSpacing = value;
          OnPropertyChanged(nameof(RowSpacing));
        }
      }
    }

    private DimensionfulQuantity _columnSpacing;

    /// <summary>
    /// Gets or sets the column spacing.
    /// </summary>
    public DimensionfulQuantity ColumnSpacing
    {
      get => _columnSpacing;
      set
      {
        if (!(_columnSpacing == value))
        {
          _columnSpacing = value;
          OnPropertyChanged(nameof(ColumnSpacing));
        }
      }
    }

    /// <summary>
    /// Gets the margin environment.
    /// </summary>
    public QuantityWithUnitGuiEnvironment MarginEnvironment => RelationEnvironment.Instance;


    private DimensionfulQuantity _leftMargin;

    /// <summary>
    /// Gets or sets the left margin.
    /// </summary>
    public DimensionfulQuantity LeftMargin
    {
      get => _leftMargin;
      set
      {
        if (!(_leftMargin == value))
        {
          _leftMargin = value;
          OnPropertyChanged(nameof(LeftMargin));
        }
      }
    }

    private DimensionfulQuantity _topMargin;

    /// <summary>
    /// Gets or sets the top margin.
    /// </summary>
    public DimensionfulQuantity TopMargin
    {
      get => _topMargin;
      set
      {
        if (!(_topMargin == value))
        {
          _topMargin = value;
          OnPropertyChanged(nameof(TopMargin));
        }
      }
    }




    private DimensionfulQuantity _rightMargin;

    /// <summary>
    /// Gets or sets the right margin.
    /// </summary>
    public DimensionfulQuantity RightMargin
    {
      get => _rightMargin;
      set
      {
        if (!(_rightMargin == value))
        {
          _rightMargin = value;
          OnPropertyChanged(nameof(RightMargin));
        }
      }
    }

    private DimensionfulQuantity _bottomMargin;

    /// <summary>
    /// Gets or sets the bottom margin.
    /// </summary>
    public DimensionfulQuantity BottomMargin
    {
      get => _bottomMargin;
      set
      {
        if (!(_bottomMargin == value))
        {
          _bottomMargin = value;
          OnPropertyChanged(nameof(BottomMargin));
        }
      }
    }

    private ItemsController<SuperfluousLayersAction> _superfluousLayers;

    /// <summary>
    /// Gets or sets the action for superfluous layers.
    /// </summary>
    public ItemsController<SuperfluousLayersAction> SuperfluousLayers
    {
      get => _superfluousLayers;
      set
      {
        if (!(_superfluousLayers == value))
        {
          _superfluousLayers = value;
          OnPropertyChanged(nameof(SuperfluousLayers));
        }
      }
    }

    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        NumberOfRows = _doc.NumberOfRows;
        NumberOfColumns = _doc.NumberOfColumns;
        RowSpacing = new DimensionfulQuantity(_doc.RowSpacing, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(SpacingEnvironment.DefaultUnit);
        ColumnSpacing = new DimensionfulQuantity(_doc.ColumnSpacing, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(SpacingEnvironment.DefaultUnit);
        LeftMargin = new DimensionfulQuantity(_doc.LeftMargin, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
        TopMargin = new DimensionfulQuantity(_doc.TopMargin, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
        RightMargin = new DimensionfulQuantity(_doc.RightMargin, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
        BottomMargin = new DimensionfulQuantity(_doc.BottomMargin, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(MarginEnvironment.DefaultUnit);
        SuperfluousLayers = new ItemsController<SuperfluousLayersAction>(new SelectableListNodeList(_doc.SuperfluousLayersAction));
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc.NumberOfRows = NumberOfRows;
      _doc.NumberOfColumns = NumberOfColumns;
      _doc.RowSpacing = RowSpacing.AsValueInSIUnits;
      _doc.ColumnSpacing = ColumnSpacing.AsValueInSIUnits;
      _doc.LeftMargin = LeftMargin.AsValueInSIUnits;
      _doc.TopMargin = TopMargin.AsValueInSIUnits;
      _doc.RightMargin = RightMargin.AsValueInSIUnits;
      _doc.BottomMargin = BottomMargin.AsValueInSIUnits;
      _doc.SuperfluousLayersAction = SuperfluousLayers.SelectedValue;

      return ApplyEnd(true, disposeController);
    }
  }
}
