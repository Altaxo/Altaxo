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

namespace Altaxo.Science.Spectroscopy
{
  public record SpectralPreprocessingOptionsList : SpectralPreprocessingOptionsBase
  {
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
  }
}
