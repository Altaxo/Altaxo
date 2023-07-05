#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable disable warnings
using System.Windows.Controls;

namespace Altaxo.Gui.Pads.Notes
{
  public class NotesControl : TextBox, INotesView
  {
    public NotesControl()
    {
      TextWrapping = System.Windows.TextWrapping.NoWrap;
      AcceptsReturn = true;
      AcceptsTab = true;
      VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
      HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Visible;
      FontFamily = new System.Windows.Media.FontFamily("Global Monospace");
      IsEnabled = false;

      {
        // Set the Text binding
        var binding = new System.Windows.Data.Binding
        {
          Path = new System.Windows.PropertyPath(nameof(Text)),
          Mode = System.Windows.Data.BindingMode.TwoWay, // binding the other way is handled by the event
          UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged
        };
        SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);
      }

      {
        // Set the Enabled binding
        var binding = new System.Windows.Data.Binding
        {
          Path = new System.Windows.PropertyPath(nameof(IsEnabled)),
        };
        SetBinding(System.Windows.Controls.TextBox.IsEnabledProperty, binding);
      }
    }


  }
}
