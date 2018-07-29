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

namespace Altaxo.Data
{
  public class TransformedReadableColumn : ITransformedReadableColumn, Main.IImmutable
  {
    private IReadableColumn _originalColumn;
    private IVariantToVariantTransformation _transformation;

    #region Serialization

    // This type has not serialization.
    // Serialization should be done via the corresponding Proxy type.

    #endregion Serialization

    public TransformedReadableColumn(IReadableColumn column, IVariantToVariantTransformation transformation)
    {
      if (null == column)
        throw new ArgumentNullException(nameof(column));

      if (null == transformation)
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

    public AltaxoVariant this[int i]
    {
      get
      {
        return _transformation.Transform(_originalColumn[i]);
      }
    }

    public int? Count
    {
      get
      {
        return _originalColumn.Count;
      }
    }

    public string FullName
    {
      get
      {
        return (_transformation.RepresentationAsOperator ?? _transformation.RepresentationAsFunction) + " " + _originalColumn.FullName;
      }
    }

    public IReadableColumn UnderlyingReadableColumn
    {
      get
      {
        return _originalColumn;
      }
    }

    public IVariantToVariantTransformation Transformation
    {
      get
      {
        return _transformation;
      }
    }

    public object Clone()
    {
      return this; // this class is immutable
    }

    public bool IsElementEmpty(int i)
    {
      if (_originalColumn.IsElementEmpty(i))
        return true;
      var val = _transformation.Transform(_originalColumn[i]);
      if (val.IsType(AltaxoVariant.Content.VDouble) && double.IsNaN(val))
        return true;
      if (val.IsType(AltaxoVariant.Content.VString) && null == (string)val)
        return true;

      return false;
    }

    public ITransformedReadableColumn WithUnderlyingReadableColumn(IReadableColumn originalReadableColumn)
    {
      if (object.Equals(_originalColumn, originalReadableColumn))
      {
        return this;
      }
      else
      {
        if (null == originalReadableColumn)
          throw new ArgumentNullException(nameof(originalReadableColumn));
        var result = (TransformedReadableColumn)this.MemberwiseClone();
        result._originalColumn = originalReadableColumn;
        return result;
      }
    }

    public ITransformedReadableColumn WithTransformation(IVariantToVariantTransformation transformation)
    {
      if (object.Equals(_transformation, transformation))
      {
        return this;
      }
      else
      {
        if (null == transformation)
          throw new ArgumentNullException(nameof(transformation));
        var result = (TransformedReadableColumn)this.MemberwiseClone();
        result._transformation = transformation;
        return result;
      }
    }
  }
}
