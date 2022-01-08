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
  public static partial class GuiHelper
  {
    public static DragDropEffects ConvertCopyMoveToDragDropEffect(bool copy, bool move)
    {
      var result = DragDropEffects.None;
      if (copy)
        result |= DragDropEffects.Copy;
      if (move)
        result |= DragDropEffects.Move;
      return result;
    }

    public static void ConvertDragDropEffectToCopyMove(DragDropEffects effects, out bool copy, out bool move)
    {
      copy = effects.HasFlag(DragDropEffects.Copy);
      move = effects.HasFlag(DragDropEffects.Move);
    }

    public static System.Windows.IDataObject ToWpf(Altaxo.Serialization.Clipboard.IDataObject dao)
    {
      return DataObjectAdapterAltaxoToWpf.FromAltaxoDataObject(dao);
    }

    public static Altaxo.Serialization.Clipboard.IDataObject ToAltaxo(System.Windows.IDataObject dao)
    {
      return DataObjectAdapterWpfToAltaxo.FromWpfDataObject(dao);
    }

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
