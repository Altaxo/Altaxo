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

using System.Linq;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Science.Spectroscopy.Normalization
{
  /// <summary>
  /// Executes vector normalization : y' = (y)/(norm), in which norm is the L2 norm of the array.
  /// </summary>
  /// <seealso cref="Altaxo.Science.Spectroscopy.Normalization.INormalization" />
  public class NormalizationVector : INormalization
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NormalizationVector), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new NormalizationVector();
      }
    }
    #endregion


    /// <inheritdoc/>
    public double[] Execute(double[] x)
    {
      var delta = VectorMath.L2Norm(x);
      return x.Select(yy => (yy) / delta).ToArray();
    }
  }
}
