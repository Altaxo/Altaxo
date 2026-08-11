#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using Altaxo.Serialization.NamePropertyExtraction;

namespace Altaxo.Gui.Serialization.NamePropertyExtraction
{
  /// <summary>
  /// Defines the view contract used by the index and extraction node controller.
  /// </summary>
  public interface IIndexAndExtractionNodeView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controls the display and editing of a specific index and its associated property extraction node in the property extraction tree.
  /// </summary>
  [ExpectedTypeOfView(typeof(IIndexAndExtractionNodeView))]
  [UserControllerForObject(typeof((int IndexOfNamePart, IPropertyExtractionTreeNode Node)))]
  public class IndexAndExtractionNodeController : MVCANControllerEditImmutableDocBase<(int IndexOfNamePart, IPropertyExtractionTreeNode Node), IIndexAndExtractionNodeView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(DetailsController, () => DetailsController = null!);
    }

    #region Bindings

    /// <summary>
    /// Gets or sets the index of the name part that this controller is responsible for.
    /// </summary>
    public int IndexOfNamePart
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field = value;
          OnPropertyChanged(nameof(IndexOfNamePart));
        }
      }
    }

    /// <summary>
    /// Gets or sets the details controller for the currently selected node in the property extraction tree.
    /// </summary>
    public IMVCANController? DetailsController
    {
      get => field;
      set
      {
        if (!(field == value))
        {
          field?.Dispose();
          field = value;
          OnPropertyChanged(nameof(DetailsController));
        }
      }
    }

    #endregion

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        IndexOfNamePart = _doc.IndexOfNamePart;
        if (_doc.Node is not null)
        {
          DetailsController = (IMVCANController?)Current.Gui.GetControllerAndControl(new object[] { _doc.Node }, typeof(IMVCANController), UseDocument.Directly);
        }
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (DetailsController is not null)
      {
        if (!DetailsController.Apply(disposeController))
          return ApplyEnd(false, disposeController);

        _doc = (IndexOfNamePart, DetailsController.ModelObject as IPropertyExtractionTreeNode);
      }
      else
      {
        _doc = (IndexOfNamePart, _doc.Node);
      }

      return ApplyEnd(true, disposeController);
    }

  }
}
