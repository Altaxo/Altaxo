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

namespace Altaxo.Data
{
  /// <summary>
  /// Readable column wrapper that applies a transformation to the values of another readable column.
  /// </summary>
  public class TransformedReadableColumn : ITransformedReadableColumn, Main.IImmutable
  {
    private IReadableColumn _originalColumn;
    private IVariantToVariantTransformation _transformation;

    #region Serialization

    // This type has not serialization.
    // Serialization should be done via the corresponding Proxy type.

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="TransformedReadableColumn"/> class.
    /// </summary>
    /// <param name="column">The source column.</param>
    /// <param name="transformation">The transformation applied to the source values.</param>
    public TransformedReadableColumn(IReadableColumn column, IVariantToVariantTransformation transformation)
    {
      if (column is null)
        throw new ArgumentNullException(nameof(column));

      if (transformation is null)
        throw new ArgumentNullException(nameof(transformation));

      _originalColumn = column;
      _transformation = transformation;
    }

    /// <summary>
    /// Gets the type of the colum's items.
    /// </summary>
    /// <value>
    /// The type of the item.
    /// </value>
    public Type ItemType { get { return _transformation.OutputValueType; } }

    /// <inheritdoc/>
    public AltaxoVariant this[int i]
    {
      get
      {
        return _transformation.Transform(_originalColumn[i]);
      }
    }

    /// <inheritdoc/>
    public int? Count
    {
      get
      {
        return _originalColumn.Count;
      }
    }

    /// <inheritdoc/>
    public string FullName
    {
      get
      {
        return (_transformation.RepresentationAsOperator ?? _transformation.RepresentationAsFunction) + " " + _originalColumn.FullName;
      }
    }

    /// <inheritdoc/>
    public IReadableColumn UnderlyingReadableColumn
    {
      get
      {
        return _originalColumn;
      }
    }

    /// <inheritdoc/>
    public IVariantToVariantTransformation Transformation
    {
      get
      {
        return _transformation;
      }
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return this; // this class is immutable
    }

    /// <inheritdoc/>
    public bool IsElementEmpty(int i)
    {
      if (_originalColumn.IsElementEmpty(i))
        return true;
      var val = _transformation.Transform(_originalColumn[i]);
      if (val.IsType(AltaxoVariant.Content.VDouble) && double.IsNaN(val))
        return true;
      if (val.IsType(AltaxoVariant.Content.VString) && string.IsNullOrEmpty(val))
        return true;

      return false;
    }

    /// <inheritdoc/>
    public ITransformedReadableColumn WithUnderlyingReadableColumn(IReadableColumn originalReadableColumn)
    {
      if (object.Equals(_originalColumn, originalReadableColumn))
      {
        return this;
      }
      else
      {
        if (originalReadableColumn is null)
          throw new ArgumentNullException(nameof(originalReadableColumn));
        var result = (TransformedReadableColumn)MemberwiseClone();
        result._originalColumn = originalReadableColumn;
        return result;
      }
    }

    /// <inheritdoc/>
    public ITransformedReadableColumn WithTransformation(IVariantToVariantTransformation transformation)
    {
      if (object.Equals(_transformation, transformation))
      {
        return this;
      }
      else
      {
        if (transformation is null)
          throw new ArgumentNullException(nameof(transformation));
        var result = (TransformedReadableColumn)MemberwiseClone();
        result._transformation = transformation;
        return result;
      }
    }
  }
}
