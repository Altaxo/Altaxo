#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Calc;
using Altaxo.Data;

namespace Altaxo.Graph.Scales.Ticks
{
  public class TextTickSpacing : TickSpacing
  {
    private List<AltaxoVariant> _majorTicks;
    private List<AltaxoVariant> _minorTicks;
    private List<AltaxoVariant> _majorTextTicks;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(TextTickSpacing), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TextTickSpacing)obj;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        TextTickSpacing s = SDeserialize(o, info, parent);
        return s;
      }

      protected virtual TextTickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        TextTickSpacing s = null != o ? (TextTickSpacing)o : new TextTickSpacing();
        return s;
      }
    }

    #endregion Serialization

    public TextTickSpacing()
    {
      _majorTicks = new List<AltaxoVariant>();
      _minorTicks = new List<AltaxoVariant>();
      _majorTextTicks = new List<AltaxoVariant>();
    }

    public TextTickSpacing(TextTickSpacing from)
      : base(from)// everything is done here, since CopyFrom is virtual!
    {
    }

    public override bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as TextTickSpacing;
      if (null == from)
        return false;

      using (var suspendToken = SuspendGetToken())
      {
        _majorTicks = new List<AltaxoVariant>(from._majorTicks);
        _minorTicks = new List<AltaxoVariant>(from._minorTicks);
        _majorTextTicks = new List<AltaxoVariant>(from._majorTextTicks);

        EhSelfChanged();
        suspendToken.Resume();
      }
      return true;
    }

    public override object Clone()
    {
      return new TextTickSpacing(this);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    public override bool PreProcessScaleBoundaries(ref Altaxo.Data.AltaxoVariant org, ref Altaxo.Data.AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
    {
      return false;
    }

    public override void FinalProcessScaleBoundaries(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end, Scale scale)
    {
      _minorTicks.Clear();
      _majorTicks.Clear();
      _majorTextTicks.Clear();

      // make major ticks at integral numbers and minor ticks at halfway
      double dorg = org;
      double dend = end;

      if (!(dorg < dend))
        return;

      double span = Math.Abs(dend - dorg);

      if (double.IsInfinity(span))
        return;

      var textBounds = scale.DataBoundsObject as Boundaries.TextBoundaries;

      double majorSpan = 1;
      if (span > 100)
        majorSpan = Math.Ceiling(span / 100);

      double minorSpan = majorSpan / 2;

      double firstmajor = Math.Floor(dorg / majorSpan) - 1; // -1 for safety and for minor
      double lastmajor = Math.Ceiling(dend / majorSpan) + 1; // +1 for safety

      for (double maj = firstmajor; maj <= lastmajor; maj += 1)
      {
        double majortick = maj * majorSpan;
        double minortick = maj * majorSpan - minorSpan;

        if (majortick.IsInIntervalCC(dorg, dend))
        {
          _majorTicks.Add(majortick);
          int item = (int)(majortick - 1);
          if (textBounds != null && item >= 0 && item < textBounds.NumberOfItems)
            _majorTextTicks.Add(textBounds.GetItem(item));
          else
            _majorTextTicks.Add(majortick);
        }
        if (minortick.IsInIntervalCC(dorg, dend))
        {
          _minorTicks.Add(minortick);
        }
      }
    }

    public override Altaxo.Data.AltaxoVariant[] GetMajorTicksAsVariant()
    {
      return _majorTextTicks.ToArray();
    }

    public override Altaxo.Data.AltaxoVariant[] GetMinorTicksAsVariant()
    {
      return _minorTicks.ToArray();
    }

    public override double[] GetMajorTicksNormal(Scale scale)
    {
      double[] result = new double[_majorTicks.Count];
      for (int i = 0; i < _majorTicks.Count; i++)
        result[i] = scale.PhysicalVariantToNormal(_majorTicks[i]);

      return result;
    }
  }
}
