#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

namespace Altaxo.Data
{
  /// <summary>
  /// Interface to a readable column that is amended with a transformation.
  /// </summary>
  /// <seealso cref="Altaxo.Data.IReadableColumn" />
  public interface ITransformedReadableColumn : IReadableColumn
  {
    /// <summary>
    /// Gets the original readable column, i.e. the readable column without the transformation.
    /// </summary>
    /// <value>
    /// The original readable column.
    /// </value>
    IReadableColumn UnderlyingReadableColumn { get; }

    /// <summary>
    /// Gets a new instance of this class with the same transformation, but another underlying readable column.
    /// </summary>
    /// <param name="underlyingReadableColumn">The new original readable column.</param>
    /// <returns>New instance of this class with the same transformation, but another underlying readable column.</returns>
    ITransformedReadableColumn WithUnderlyingReadableColumn(IReadableColumn underlyingReadableColumn);

    /// <summary>
    /// Gets the transformation.
    /// </summary>
    /// <value>
    /// The transformation.
    /// </value>
    IVariantToVariantTransformation Transformation { get; }

    /// <summary>
    /// Gets a new instance of this class with the same underlying original column, but with another transformation.
    /// </summary>
    /// <param name="transformation">The new transformation.</param>
    /// <returns>A new instance of this class with the same underlying original column, but with another transformation.</returns>
    ITransformedReadableColumn WithTransformation(IVariantToVariantTransformation transformation);
  }
}
