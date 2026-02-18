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
  /// Represents a named compound auxiliary value consisting of multiple auxiliary data items.
  /// </summary>
  public record EnsembleAuxiliaryDataCompound : IEnsembleProcessingAuxiliaryData, Main.IImmutable
  {
    /// <inheritdoc/>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the contained auxiliary data items.
    /// </summary>
    public required IEnsembleProcessingAuxiliaryData[] Values { get; init; }

    #region Serialization

    /// <summary>
    /// V0: 2026-02-16
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(EnsembleAuxiliaryDataCompound), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (EnsembleAuxiliaryDataCompound)obj;

        info.AddValue("Name", s.Name);
        info.AddArray("Values", s.Values, s.Values.Length);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var name = info.GetString("Name");
        var values = info.GetArrayOfValues<IEnsembleProcessingAuxiliaryData>("Values", parent);
        return new EnsembleAuxiliaryDataCompound { Name = name, Values = values };
      }
    }

    #endregion Serialization
  }



}
