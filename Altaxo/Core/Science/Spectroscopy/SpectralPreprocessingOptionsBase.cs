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

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Altaxo.Main;

namespace Altaxo.Science.Spectroscopy
{
  public abstract record SpectralPreprocessingOptionsBase : IImmutable, ISingleSpectrumPreprocessorCompound, IEnumerable<ISingleSpectrumPreprocessor>, IReadOnlyList<ISingleSpectrumPreprocessor>
  {
    public ISingleSpectrumPreprocessor this[int index] => ((IReadOnlyList<ISingleSpectrumPreprocessor>)InnerList)[index];

    public int Count => InnerList.Count;

    protected ImmutableList<ISingleSpectrumPreprocessor> InnerList { get; init; } = ImmutableList<ISingleSpectrumPreprocessor>.Empty;

    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      if (regions is null || regions.Length == 0)
      {
        System.Array.Sort(x, y);
      }

      foreach (var processor in InnerList)
      {
        (x, y, regions) = processor.Execute(x, y, regions);
      }
      return (x, y, regions);
    }

    public IEnumerator<ISingleSpectrumPreprocessor> GetEnumerator()
    {
      return ((IEnumerable<ISingleSpectrumPreprocessor>)InnerList).GetEnumerator();
    }

    public IEnumerable<ISingleSpectrumPreprocessor> GetProcessorElements()
    {
      return InnerList;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable)InnerList).GetEnumerator();
    }

    public override string ToString()
    {
      var stb = new StringBuilder();
      foreach (var ele in InnerList)
      {
        if (!(ele.GetType().Name.Contains("None")))
        {
          stb.Append('[');
          stb.Append(ele.ToString());
          stb.Append("] ");
        }
      }
      return stb.ToString();
    }
  }
}
