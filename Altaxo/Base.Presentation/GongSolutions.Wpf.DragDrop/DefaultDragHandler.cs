#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
// BSD 3-Clause License
//
// Copyright(c) 2015-16, Jan Karger(Steven Kirk)
//
// All rights reserved.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System.Linq;
using System.Windows;
using GongSolutions.Wpf.DragDrop.Utilities;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Provides the default implementation for starting and completing drag operations.
  /// </summary>
  public class DefaultDragHandler : IDragSource
  {
    /// <inheritdoc/>
    public virtual void StartDrag(IDragInfo dragInfo)
    {
      var itemCount = dragInfo.SourceItems.Cast<object>().Count();

      if (itemCount == 1)
      {
        dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();
      }
      else if (itemCount > 1)
      {
        dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems);
      }

      dragInfo.Effects = (dragInfo.Data is not null) ?
                           DragDropEffects.Copy | DragDropEffects.Move :
                           DragDropEffects.None;
    }

    /// <inheritdoc/>
    public bool CanStartDrag(IDragInfo dragInfo)
    {
      return true;
    }

    /// <inheritdoc/>
    public virtual void Dropped(IDropInfo dropInfo, DragDropEffects effects) // ModifiedByLellid
    {
    }

    /// <inheritdoc/>
    public virtual void DragCancelled()
    {
    }
  }
}
