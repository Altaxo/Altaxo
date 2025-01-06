#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

using Altaxo.Drawing;
using Altaxo.Main;

namespace Altaxo.Science.Signals
{
  /// <summary>
  /// Options for the step evaluation tool mouse handler.
  /// </summary>
  public record FourPointPeakEvaluationToolMouseHandlerOptions : IImmutable
  {
    /// <summary>
    /// If true, the options dialog is shown when the four point step evaluation tool is activated.
    /// </summary>
    public bool ShowOptionsWhenToolIsActivated { get; init; } = true;

    /// <summary>
    /// Gets the pen that is used for drawing the lines.
    /// </summary>
    public PenX LinePen { get; init; } = new PenX(NamedColors.LightBlue, 2);

    public BrushX AreaBrush { get; init; } = new BrushX(NamedColors.LightBlue);

    #region Serialization

    /// <summary>
    /// 2024-01-06 V0
    /// </summary>
    /// <seealso cref="Serialization.Xml.IXmlSerializationSurrogate" />
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(FourPointPeakEvaluationToolMouseHandlerOptions), 0)]
    public class SerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FourPointPeakEvaluationToolMouseHandlerOptions)obj;

        info.AddValue("ShowOptionsWhenToolIsActivated", s.ShowOptionsWhenToolIsActivated);
        info.AddValue("LinePen", s.LinePen);
        info.AddValue("AreaBrush", s.AreaBrush);
      }

      public object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var showOptionsWhenToolIsActivated = info.GetBoolean("ShowOptionsWhenToolIsActivated");
        var linePen = info.GetValue<PenX>("LinePen", null);
        var areaBrush = info.GetValue<BrushX>("AreaBrush", null);

        return new FourPointPeakEvaluationToolMouseHandlerOptions()
        {
          ShowOptionsWhenToolIsActivated = showOptionsWhenToolIsActivated,
          LinePen = linePen,
          AreaBrush = areaBrush,
        };
      }
    }
    #endregion

  }
}
