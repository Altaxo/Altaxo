#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

namespace Altaxo.Science.Spectroscopy.EnsembleProcessing
{
  /// <summary>
  /// Represents a named vector auxiliary value produced during ensemble processing.
  /// </summary>
  public record EnsembleAuxiliaryDataVector : IEnsembleProcessingAuxiliaryData
  {
    /// <inheritdoc/>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the vector values.
    /// </summary>
    public required double[] Value { get; init; }

    /// <summary>
    /// Gets the type of the vector.
    /// </summary>
    public required EnsembleAuxiliaryDataVectorType VectorType { get; init; }

    #region Serialization

    /// <summary>
    /// V0: 2026-02-16
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EnsembleAuxiliaryDataVector), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EnsembleAuxiliaryDataVector)obj;

        info.AddValue("Name", s.Name);
        info.AddEnum("VectorType", s.VectorType);
        info.AddArray("Value", s.Value, s.Value.Length);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var name = info.GetString("Name");
        var vectorType = info.GetEnum<EnsembleAuxiliaryDataVectorType>("VectorType");
        info.GetArray("Value", out double[] values);
        return new EnsembleAuxiliaryDataVector { Name = name, VectorType = vectorType, Value = values };
      }
    }

    #endregion Serialization

  }

  /// <summary>
  /// Represents the type of an auxiliary data vector.
  /// </summary>
  public enum EnsembleAuxiliaryDataVectorType
  {
    /// <summary>
    /// Represents an auxiliary data vector with a length equal to the number of spectral points in the ensemble.
    /// </summary>
    Spectrum = 0,

    /// <summary>
    /// Represents an auxiliary data vector with a length equal to the number of samples in the ensemble.
    /// </summary>
    Samples = 1,

    /// <summary>
    /// Represents an auxiliary data vector of other length or type.
    /// </summary>
    Other = 2
  }


}
