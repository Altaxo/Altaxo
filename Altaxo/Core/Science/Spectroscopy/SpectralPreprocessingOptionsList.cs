#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Linq;

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// A flexible list-based set of spectral preprocessing options.
  /// </summary>
  /// <remarks>
  /// In contrast to <see cref="SpectralPreprocessingOptions"/>, this type does not enforce a fixed set or order of elements.
  /// </remarks>
  public record SpectralPreprocessingOptionsList : SpectralPreprocessingOptionsBase
  {
    /// <summary>
    /// Gets an empty preprocessing list.
    /// </summary>
    public static SpectralPreprocessingOptionsList Empty { get; } = new SpectralPreprocessingOptionsList();

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2023-04-16 Initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectralPreprocessingOptionsList), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpectralPreprocessingOptionsList)obj;

        info.CreateArray("Elements", s.Count);
        {
          for (int i = 0; i < s.Count; i++)
          {
            info.AddValue("e", s[i]);
          }
        }
        info.CommitArray();


      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var count = info.OpenArray("Elements");
        var list = new ISingleSpectrumPreprocessor[count];
        {
          for (int i = 0; i < count; ++i)
          {
            list[i] = info.GetValue<ISingleSpectrumPreprocessor>("e", parent);
          }
          info.CloseArray(count);
        }
        return new SpectralPreprocessingOptionsList(list);
      }
    }
    #endregion

    #endregion

    private SpectralPreprocessingOptionsList()
    {
    }

    /// <summary>
    /// Initializes a new instance from the provided preprocessing elements.
    /// </summary>
    /// <param name="list">The preprocessing elements.</param>
    public SpectralPreprocessingOptionsList(params ISingleSpectrumPreprocessor[] list)
      : this((IEnumerable<ISingleSpectrumPreprocessor>)list)
    {
    }

    /// <summary>
    /// Initializes a new instance from the provided preprocessing elements.
    /// </summary>
    /// <param name="list">The preprocessing elements.</param>
    /// <exception cref="ArgumentException">The list contains <see langword="null"/> elements.</exception>
    public SpectralPreprocessingOptionsList(IEnumerable<ISingleSpectrumPreprocessor> list)
    {

      var ilist = list.ToImmutableList();
      // all elements must be not null
      for (int i = 0; i < ilist.Count; ++i)
      {
        if (ilist[i] is null)
          throw new ArgumentException($"List element[{i}] is null!", nameof(list));
      }
      InnerList = ilist;
    }



    /// <summary>
    /// Returns a new instance with the specified processor appended.
    /// </summary>
    /// <param name="processor">The processor to add.</param>
    /// <returns>A new instance with the specified processor appended.</returns>
    public SpectralPreprocessingOptionsList WithAdded(ISingleSpectrumPreprocessor processor)
    {
      return this with { InnerList = InnerList.Add(processor ?? throw new ArgumentNullException(nameof(processor))) };
    }

    /// <summary>
    /// Returns a new instance with the processor at the specified index removed.
    /// </summary>
    /// <param name="index">The index of the processor to remove.</param>
    /// <returns>A new instance with the processor at the specified index removed.</returns>
    public SpectralPreprocessingOptionsList WithRemovedAt(int index)
    {
      return this with { InnerList = this.InnerList.RemoveAt(index) };
    }

    /// <summary>
    /// Returns a new instance with the specified processor inserted at the specified index.
    /// </summary>
    /// <param name="index">The index at which to insert the processor.</param>
    /// <param name="processor">The processor to insert.</param>
    /// <returns>A new instance with the specified processor inserted.</returns>
    public SpectralPreprocessingOptionsList WithInserted(int index, ISingleSpectrumPreprocessor processor)
    {
      return this with { InnerList = InnerList.Insert(index, processor ?? throw new ArgumentNullException(nameof(processor))) };
    }

    /// <summary>
    /// Creates a new instance from the provided elements, omitting <see langword="null"/> elements and elements whose type name contains <c>None</c>.
    /// </summary>
    /// <param name="elements">The preprocessing elements.</param>
    /// <returns>A new instance containing only the non-<c>None</c> elements.</returns>
    public static SpectralPreprocessingOptionsList CreateWithoutNoneElements(IEnumerable<ISingleSpectrumPreprocessor> elements)
    {
      var result = new SpectralPreprocessingOptionsList
      {
        InnerList = elements.Where(e => e is not null && !e.GetType().Name.Contains("None")).ToImmutableList()
      };

      return result;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return base.ToString();
    }
  }
}
