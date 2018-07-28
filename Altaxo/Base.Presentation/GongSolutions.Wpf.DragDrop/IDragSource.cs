using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Interface implemented by Drag Handlers.
  /// </summary>
  public interface IDragSource
  {
    /// <summary>
    /// Queries whether a drag can be started.
    /// </summary>
    ///
    /// <param name="dragInfo">
    /// Information about the drag.
    /// </param>
    ///
    /// <remarks>
    /// To allow a drag to be started, the <see cref="DragInfo.Effects"/> property on <paramref name="dragInfo"/>
    /// should be set to a value other than <see cref="DragDropEffects.None"/>.
    /// </remarks>
    void StartDrag(IDragInfo dragInfo);

    /// <summary>
    /// With this action it's possible to check if the drga and drop operation is allowed to start
    /// e.g. check for a UIElement inside a list view item, that should not start a drag&amp;drop operation
    /// </summary>
    bool CanStartDrag(IDragInfo dragInfo);

    /// <summary>
    /// Notifies the drag handler that a drop has occurred.
    /// </summary>
    ///
    /// <param name="dropInfo">
    ///   Information about the drop. Is <c>null</c> if the drop occured in another app.
    /// </param>
    /// <param name="effects">The operation (Copy, paste, etc.) with which the drop was executed.</param>
    void Dropped(IDropInfo dropInfo, DragDropEffects effects); // ModifiedByLellid

    /// <summary>
    /// Notifies the drag handler that a drag has been aborted.
    /// </summary>
    void DragCancelled();
  }
}
