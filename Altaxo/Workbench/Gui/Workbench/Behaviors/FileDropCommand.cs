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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Attached behaviour to implement the drop event via delegate command binding or routed commands. This command allows dropping files only.
  /// </summary>
  /// <remarks>
  /// References:
  /// <para><see href="http://stackoverflow.com/questions/1034374/drag-and-drop-in-mvvm-with-scatterview"/></para>
  /// <para><see href="http://social.msdn.microsoft.com/Forums/de-DE/wpf/thread/21bed380-c485-44fb-8741-f9245524d0ae"/></para>
  /// </remarks>
  /// <example>
  /// Add the following to the XAMK of your UIElement:
  /// <code>
  /// AllowDrop="True"
  /// local:FileDropCommand.DropCommand="{Binding DropCommand}"
  /// </code>
  /// </example>
  public static class FileDropCommand
  {
    // Field of attached ICommand property
    private static readonly DependencyProperty DropCommandProperty = DependencyProperty.RegisterAttached(
            "DropCommand",
            typeof(ICommand),
            typeof(FileDropCommand),
            new PropertyMetadata(null, OnDropCommandChange));

    /// <summary>
    /// Setter method of the attached DropCommand <seealso cref="ICommand"/> property
    /// </summary>
    /// <param name="source">The UIElement this behavior is attached to.</param>
    /// <param name="value">The drop command.</param>
    public static void SetDropCommand(UIElement source, ICommand value)
    {
      source.SetValue(DropCommandProperty, value);
    }

    /// <summary>
    /// Getter method of the attached DropCommand <seealso cref="ICommand"/> property
    /// </summary>
    /// <param name="source">The UIElement this behavior is attached to.</param>
    /// <returns>The drop command.</returns>
    public static ICommand GetDropCommand(UIElement source)
    {
      return (ICommand)source.GetValue(DropCommandProperty);
    }

    /// <summary>
    /// This method is hooked in the definition of the <seealso cref="DropCommandProperty"/>.
    /// It is called whenever the attached property changes - in our case the event of binding
    /// and unbinding the property to a sink is what we are looking for.
    /// </summary>
    /// <param name="d">The UIElement this behavior is attached to.</param>
    /// <param name="e">DependencyPropertyChangedEventArgs concerning the drop command property.</param>
    private static void OnDropCommandChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var uiElement = (UIElement)d;     // Remove the handler if it exist to avoid memory leaks
      uiElement.Drop -= UIElement_Drop;

      if (e.NewValue is ICommand command)
      {
        // the property is attached so we attach the Drop event handler
        uiElement.Drop += UIElement_Drop;
      }
    }

    /// <summary>
    /// This method is called when the Drop event occurs. The sender should be the control
    /// on which this behaviour is attached - so we convert the sender into a <seealso cref="UIElement"/>
    /// and receive the Command through the <seealso cref="GetDropCommand"/> getter listed above.
    ///
    /// The <paramref name="e"/> parameter contains the standard <seealso cref="DragEventArgs"/> data,
    /// which is unpacked and reales upon the bound command.
    ///
    /// This implementation supports binding of delegate commands and routed commands.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void UIElement_Drop(object sender, DragEventArgs e)
    {
      var uiElement = sender as UIElement;

      // Sanity check just in case this was somehow send by something else
      if (uiElement is null)
        return;

      ICommand dropCommand = FileDropCommand.GetDropCommand(uiElement);

      // There may not be a command bound to this after all
      if (dropCommand is null)
        return;

      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        if(e.Data.GetData(DataFormats.FileDrop, true) is string[] droppedFilePaths)
        {
          foreach (string droppedFilePath in droppedFilePaths)
          {
            // Check whether this attached behaviour is bound to a RoutedCommand
            if (dropCommand is RoutedCommand rcmd)
            {
              // Execute the routed command
              rcmd.Execute(droppedFilePath, uiElement);
            }
            else
            {
              // Execute the Command as bound delegate
              dropCommand.Execute(droppedFilePath);
            }
          }
        }
      }
    }
  }
}
