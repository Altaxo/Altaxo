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


namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// SNIP algorithm for background estimation on linear (unmodified) data. SNIP = Statistical sensitive Non-Linear Iterative Procedure.
  /// </summary>
  /// <remarks>
  /// In difference to the procedure described in Ref. 1, no previous smoothing is applied to the data. Furthermore,
  /// the paper suggests to twice logarithmize the data beforehand, which is also not done here.
  /// As described in the paper, after execution the number of regular stages of the algorithm, the window width is sucessivly decreased, until it reaches 1.
  /// This results in a smoothing of the background signal.
  /// 
  /// <para>References:</para>
  /// <para>[1] C.G. Ryan et al., SNIP, A STATISTICS-SENSITIVE BACKGROUND TREATMENT FOR THE QUANTITATIVE 
  /// ANALYSIS OF PIXE SPECTRA IN GEOSCIENCE APPLICATIONS, Nuclear Instruments and Methods in Physics Research 934 (1988) 396-402 
  /// North-Holland, Amsterdam</para>
  /// </remarks>
  public record SNIP_Linear : SNIP_Base, IBaselineEstimation
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SNIP_Linear), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SNIP_Linear)obj;
        info.AddValue("HalfWidth", s.HalfWidth);
        info.AddValue("IsHalfWidthInXUnits", s.IsHalfWidthInXUnits);
        info.AddValue("NumberOfIterations", s.NumberOfRegularIterations);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var halfWidth = info.GetDouble("HalfWidth");
        var isHalfWidthInXUnits = info.GetBoolean("IsHalfWidthInXUnits");
        var numberOfIterations = info.GetInt32("NumberOfIterations");

        return o is null ? new SNIP_Linear
        {
          HalfWidth = halfWidth,
          IsHalfWidthInXUnits = isHalfWidthInXUnits,
          NumberOfRegularIterations = numberOfIterations
        } :
          ((SNIP_Linear)o) with
          {
            HalfWidth = halfWidth,
            IsHalfWidthInXUnits = isHalfWidthInXUnits,
            NumberOfRegularIterations = numberOfIterations
          };
      }
    }
    #endregion

    public override string ToString()
    {
      return $"{this.GetType().Name} HW={HalfWidth}{(IsHalfWidthInXUnits ? 'X' : 'P')} Iterations={NumberOfRegularIterations}";
    }
  }
}
