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
using System;

namespace Altaxo.Data
{
  /// <summary>
  /// A column that is always empty (count is zero).
  /// </summary>
  public class EmptyDoubleColumn : INumericColumn, IReadableColumn, ICloneable, Main.IImmutable
  {
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static EmptyDoubleColumn Instance { get; } = new EmptyDoubleColumn();


    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EmptyDoubleColumn), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EmptyDoubleColumn)o;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return EmptyDoubleColumn.Instance;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyDoubleColumn"/> class.
    /// </summary>
    public EmptyDoubleColumn()
    {
    }

    /// <summary>
    /// Creates a cloned instance of this object.
    /// </summary>
    /// <returns>The cloned instance of this object.</returns>
    public object Clone()
    {
      return EmptyDoubleColumn.Instance;
    }

    /// <summary>
    /// Gets the type of the colum's items.
    /// </summary>
    /// <value>
    /// The type of the item.
    /// </value>
    public Type ItemType { get { return typeof(double); } }

    /// <summary>
    /// Simply returns the value i.
    /// </summary>
    /// <param name="i">The index i.</param>
    /// <returns>The index i.</returns>
    public double this[int i]
    {
      get
      {
        return double.NaN;
      }
    }

    /// <summary>
    /// This returns always true.
    /// </summary>
    /// <param name="i">The index i.</param>
    /// <returns>Always true.</returns>
    public bool IsElementEmpty(int i)
    {
      return true;
    }

    /// <summary>
    /// Returns the index i as AltaxoVariant.
    /// </summary>
    /// <inheritdoc />
    AltaxoVariant IReadableColumn.this[int i]
    {
      get
      {
        return new AltaxoVariant(double.NaN);
      }
    }

    /// <summary>
    /// The full name of this column (Gui culture)
    /// </summary>
    public string FullName
    {
      get { return "Empty DoubleColumn"; }
    }

    /// <summary>
    /// Gets the number of rows if known.
    /// </summary>
    public int? Count
    {
      get
      {
        return 0;
      }
    }

    /// <inheritdoc />
    public override string ToString()
    {
      return "Empty DoubleColumn";
    }

    /// <summary>
    /// Gets a value indicating whether the column is editable.
    /// </summary>
    public bool IsEditable { get { return false; } }
  }
}
