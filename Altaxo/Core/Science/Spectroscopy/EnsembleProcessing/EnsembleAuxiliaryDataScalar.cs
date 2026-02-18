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
  /// Represents a named scalar auxiliary value produced during ensemble processing.
  /// </summary>
  public record EnsembleAuxiliaryDataScalar : IEnsembleProcessingAuxiliaryData
  {
    /// <inheritdoc/>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the scalar value.
    /// </summary>
    public required double Value { get; init; }

    #region Serialization

    /// <summary>
    /// V0: 2026-02-16
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EnsembleAuxiliaryDataScalar), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EnsembleAuxiliaryDataScalar)obj;

        info.AddValue("Name", s.Name);
        info.AddValue("Value", s.Value);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var name = info.GetString("Name");
        var value = info.GetDouble("Value");
        return new EnsembleAuxiliaryDataScalar { Name = name, Value = value };
      }
    }

    #endregion Serialization

  }



}
