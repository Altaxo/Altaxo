#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using Altaxo.AddInItems;
using System;
using System.Collections;

namespace Altaxo.Main
{
  /// <summary>
  /// Graph3DDisplayBindingDoozer generates a class with interface <see cref="IProjectItemDisplayBindingDescriptor"/>, which can be used
  /// to bind the project item of type <see cref="Altaxo.Graph.Graph3D.GraphDocument"/> to the appropriate view content.
  ///
  /// </summary>
  public class Graph3DDisplayBindingDoozer : IDoozer
  {
    /// <summary>
    /// Gets if the doozer handles codon conditions on its own.
    /// If this property return false, the item is excluded when the condition is not met.
    /// </summary>
    public bool HandleConditions
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Creates an item with the specified sub items. And the current
    /// Condition status for this item.
    /// </summary>
    public object BuildItem(BuildItemArgs args)
    {
      return new Graph3DDisplayBindingDescriptor(args.Codon, typeof(Altaxo.Graph.Graph3D.GraphDocument), typeof(Gui.Graph.Graph3D.Viewing.Graph3DController));
    }

    private class Graph3DDisplayBindingDescriptor : IProjectItemDisplayBindingDescriptor
    {
      private Type _projectItemType;
      private Type _viewContentType;
      public Type ProjectItemType { get { return _projectItemType; } }
      public Type ViewContentType { get { return _viewContentType; } }

      private Codon _codon;

      public string Id { get; set; }
      public string Title { get; set; }

      public Graph3DDisplayBindingDescriptor(Codon codon, Type projectItemType, Type viewContentType)
      {
        if (codon == null)
          throw new ArgumentNullException(nameof(codon));

        this._codon = codon;
        this.Id = codon.Id;

        string title = codon.Properties["title"];
        if (string.IsNullOrEmpty(title))
          this.Title = codon.Id;
        else
          this.Title = title;

        _projectItemType = projectItemType;
        _viewContentType = viewContentType;
      }

      public override string ToString()
      {
        return string.Format("[Graph3DDisplayBindingDescriptor ItemClass={0} ControllerClass={1}]", ProjectItemType, ViewContentType);
      }
    }
  }
}
