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
  public record SpectralPreprocessingOptionsList : SpectralPreprocessingOptionsBase
  {
    #region Serialization

    #region Version 0

    /// <summary>
    /// 2023-04-16 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectralPreprocessingOptionsList), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
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


    public SpectralPreprocessingOptionsList()
    {
    }

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

    public SpectralPreprocessingOptionsList Add(ISingleSpectrumPreprocessor processor)
    {
      return this with { InnerList = InnerList.Add(processor ?? throw new ArgumentNullException(nameof(processor))) };
    }

    public SpectralPreprocessingOptionsList RemoveAt(int index)
    {
      return this with { InnerList = this.InnerList.RemoveAt(index) };
    }

    public SpectralPreprocessingOptionsList Insert(int index, ISingleSpectrumPreprocessor processor)
    {
      return this with { InnerList = InnerList.Insert(index, processor ?? throw new ArgumentNullException(nameof(processor))) };
    }

    public static SpectralPreprocessingOptionsList CreateWithoutNoneElements(IEnumerable<ISingleSpectrumPreprocessor> elements)
    {
      var result = new SpectralPreprocessingOptionsList
      {
        InnerList = elements.Where(e => e is not null && !e.GetType().Name.Contains("None")).ToImmutableList()
      };

      return result;
    }

    public override string ToString()
    {
      return base.ToString();
    }
  }
}
