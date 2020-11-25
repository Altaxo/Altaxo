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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
  /// <summary>
  /// Accumulatable event arg that can be used for general purposes to indicate a change in a collection.
  /// The only parameter here is the originator, i.e. the collection in which the change took place.
  /// </summary>
  public class SimpleCollectionChangedEventArgs : SelfAccumulateableEventArgs
  {
    /// <summary>
    /// Gets or sets the originator of this event, i.e. the collection in which the change took place.
    /// </summary>
    /// <value>
    /// The originator of the event, i.e. the collection in which the change took place.
    /// </value>
    public IDocumentLeafNode Originator { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleCollectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="originator">The originator.</param>
    /// <exception cref="System.ArgumentNullException">originator</exception>
    public SimpleCollectionChangedEventArgs(IDocumentLeafNode originator)
    {
      if (originator is null)
        throw new ArgumentNullException(nameof(originator));

      Originator = originator;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
      return 7 * GetType().GetHashCode() + 13 * Originator.GetHashCode();
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
      return obj is SimpleCollectionChangedEventArgs from && object.ReferenceEquals(Originator, from.Originator);
    }

    /// <summary>
    /// Adds the specified event args e.
    /// </summary>
    /// <param name="e">The <see cref="Altaxo.Main.SelfAccumulateableEventArgs" /> instance containing the event data.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Argument e should be of type SimpleCollectionChangedEventArgs
    /// or
    /// Argument e has an item which is not identical to this item. This should not happen since Equals and GetHashCode are overriden.
    /// </exception>
    public override void Add(SelfAccumulateableEventArgs e)
    {
      if (!(e is SimpleCollectionChangedEventArgs other))
        throw new ArgumentOutOfRangeException("Argument e should be of type SimpleCollectionChangedEventArgs");
      if (!object.ReferenceEquals(Originator, other.Originator))
        throw new ArgumentOutOfRangeException("Argument e has an item which is not identical to this item. This should not happen since Equals and GetHashCode are overriden.");
    }
  }
}
