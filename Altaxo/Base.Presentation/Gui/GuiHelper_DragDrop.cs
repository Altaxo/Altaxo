#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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

using System.Windows;

namespace Altaxo.Gui
{
  /// <summary>
  /// Provides helper methods for drag-and-drop conversions.
  /// </summary>
  public static partial class GuiHelper
  {
    /// <summary>
    /// Converts copy and move flags to a WPF drag-and-drop effect.
    /// </summary>
    /// <param name="copy">Whether copy is allowed.</param>
    /// <param name="move">Whether move is allowed.</param>
    /// <returns>The resulting drag-and-drop effect.</returns>
    public static DragDropEffects ConvertCopyMoveToDragDropEffect(bool copy, bool move)
    {
      var result = DragDropEffects.None;
      if (copy)
        result |= DragDropEffects.Copy;
      if (move)
        result |= DragDropEffects.Move;
      return result;
    }

    /// <summary>
    /// Converts a WPF drag-and-drop effect to copy and move flags.
    /// </summary>
    /// <param name="effects">The drag-and-drop effects.</param>
    /// <param name="copy">Set to <c>true</c> if copy is allowed.</param>
    /// <param name="move">Set to <c>true</c> if move is allowed.</param>
    public static void ConvertDragDropEffectToCopyMove(DragDropEffects effects, out bool copy, out bool move)
    {
      copy = effects.HasFlag(DragDropEffects.Copy);
      move = effects.HasFlag(DragDropEffects.Move);
    }

    /// <summary>
    /// Converts an Altaxo clipboard data object to a WPF data object.
    /// </summary>
    /// <param name="dao">The Altaxo data object.</param>
    /// <returns>The converted WPF data object.</returns>
    public static System.Windows.IDataObject ToWpf(Altaxo.Serialization.Clipboard.IDataObject dao)
    {
      return DataObjectAdapterAltaxoToWpf.FromAltaxoDataObject(dao);
    }

    /// <summary>
    /// Converts a WPF data object to an Altaxo clipboard data object.
    /// </summary>
    /// <param name="dao">The WPF data object.</param>
    /// <returns>The converted Altaxo data object.</returns>
    public static Altaxo.Serialization.Clipboard.IDataObject ToAltaxo(System.Windows.IDataObject dao)
    {
      return DataObjectAdapterWpfToAltaxo.FromWpfDataObject(dao);
    }

    /// <summary>
    /// Converts a GongSolutions relative insert position to the Altaxo equivalent.
    /// </summary>
    /// <param name="pos">The GongSolutions relative insert position.</param>
    /// <returns>The converted Altaxo relative insert position.</returns>
    public static Altaxo.Gui.Common.DragDropRelativeInsertPosition ToAltaxo(GongSolutions.Wpf.DragDrop.RelativeInsertPosition pos)
    {
      Altaxo.Gui.Common.DragDropRelativeInsertPosition result = 0;

      if (pos.HasFlag(GongSolutions.Wpf.DragDrop.RelativeInsertPosition.BeforeTargetItem))
        result |= Common.DragDropRelativeInsertPosition.BeforeTargetItem;

      if (pos.HasFlag(GongSolutions.Wpf.DragDrop.RelativeInsertPosition.AfterTargetItem))
        result |= Common.DragDropRelativeInsertPosition.AfterTargetItem;

      if (pos.HasFlag(GongSolutions.Wpf.DragDrop.RelativeInsertPosition.TargetItemCenter))
        result |= Common.DragDropRelativeInsertPosition.TargetItemCenter;

      return result;
    }
  }
}
