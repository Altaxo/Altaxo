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

#nullable disable
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Graph.Gdi.LabelFormatting;
using Altaxo.Gui.Common;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.LabelFormatting
{
  /// <summary>
  /// Provides the view for editing multiline label formatting.
  /// </summary>
  public interface IMultiLineLabelFormattingBaseView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controls the common multiline label formatting settings for 2D graphs.
  /// </summary>
  [UserControllerForObject(typeof(MultiLineLabelFormattingBase))]
  [ExpectedTypeOfView(typeof(IMultiLineLabelFormattingBaseView))]
  public class MultiLineLabelFormattingBaseController : MVCANControllerEditOriginalDocBase<MultiLineLabelFormattingBase, IMultiLineLabelFormattingBaseView>
  {
    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    /// <summary>
    /// Gets the quantity environment used for editing line spacing.
    /// </summary>
    public QuantityWithUnitGuiEnvironment LineSpacingEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _lineSpacing;

    /// <summary>
    /// Gets or sets the spacing between lines.
    /// </summary>
    public DimensionfulQuantity LineSpacing
    {
      get => _lineSpacing;
      set
      {
        if (!(_lineSpacing == value))
        {
          _lineSpacing = value;
          OnPropertyChanged(nameof(LineSpacing));
        }
      }
    }

    private ItemsController<System.Drawing.StringAlignment> _textBlockAlignment;

    /// <summary>
    /// Gets or sets the alignment of the text block.
    /// </summary>
    public ItemsController<System.Drawing.StringAlignment> TextBlockAlignment
    {
      get => _textBlockAlignment;
      set
      {
        if (!(_textBlockAlignment == value))
        {
          _textBlockAlignment = value;
          OnPropertyChanged(nameof(TextBlockAlignment));
        }
      }
    }


    #endregion

    /// <inheritdoc />
    public override void Dispose(bool isDisposing)
    {
      _textBlockAlignment = null;
      base.Dispose(isDisposing);
    }

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        TextBlockAlignment = new ItemsController<System.Drawing.StringAlignment>(new SelectableListNodeList(_doc.TextBlockAlignment));
        LineSpacing = new DimensionfulQuantity(_doc.LineSpacing, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(LineSpacingEnvironment.DefaultUnit);
      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      _doc.LineSpacing = LineSpacing.AsValueInSIUnits;
      _doc.TextBlockAlignment = _textBlockAlignment.SelectedValue;

      return ApplyEnd(true, disposeController);
    }
  }
}
