using System;
using System.Collections.Immutable;

namespace Altaxo.Science.Spectroscopy.Sanitizing
{
  public class SpectrumSubtraction : ISingleSpectrumPreprocessor
  {
    /// <inheritdoc/>
    public (string TableName, string XColumnName, string YColumnName)? DataOrigin { get; init; }

    /// <inheritdoc/>
    public ImmutableArray<(double x, double y)> Spectrum { get; init; }

    #region Serialization

    /// <summary>
    /// 2023-03-29 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectrumSubtraction), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpectrumSubtraction)obj;

        info.AddValue("TableName", s.DataOrigin?.TableName);
        info.AddValue("XColumnName", s.DataOrigin?.XColumnName);
        info.AddValue("YColumnName", s.DataOrigin?.YColumnName);

        info.CreateArray("Spectrum", s.Spectrum.Length);
        {
          foreach (var ele in s.Spectrum)
          {
            info.CreateElement("e");
            {
              info.AddValue("x", ele.x);
              info.AddValue("y", ele.y);
            }
            info.CommitElement(); // e
          }
        }
        info.CommitArray();
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var tableName = info.GetString("TableName");
        var xcolumnname = info.GetString("XColumnName");
        var ycolumnname = info.GetString("YColumnName");



        var count = info.OpenArray("CalibrationTable");
        var array = new (double x_uncalibrated, double x_calibrated)[count];
        for (int i = 0; i < count; i++)
        {
          info.OpenElement();
          {
            array[i] = (info.GetDouble("x"), info.GetDouble("y"));
          }
          info.CloseElement();
        }
        info.CloseArray(count);


        return new SpectrumSubtraction()
        {
          DataOrigin = !string.IsNullOrEmpty(tableName) &&
                       !string.IsNullOrEmpty(xcolumnname) &&
                       !string.IsNullOrEmpty(ycolumnname) ? (tableName, xcolumnname, ycolumnname) : null,
          Spectrum = array.ToImmutableArray(),
        };
      }
    }
    #endregion


    /// <inheritdoc/>
    public (double[] x, double[] y, int[]? regions) Execute(double[] x, double[] y, int[]? regions)
    {
      var yy = new double[y.Length];
      foreach (var (start, end) in RegionHelper.GetRegionRanges(regions, x.Length))
      {
        var xSpan = new ReadOnlySpan<double>(x, start, end - start).ToArray();
        var ySpan = new ReadOnlySpan<double>(y, start, end - start).ToArray();
        var yySpan = new double[end - start];
        Execute(xSpan, ySpan, yySpan);
        //Span(yy,start, end-start).
      }

      return (x, yy, regions);
    }

    public void Execute(double[] x, double[] y, ArraySegment<double> yResult)
    {
      var spl = new Altaxo.Calc.Interpolation.AkimaCubicSpline();
      spl.Interpolate(x, y);

      for (int i = 0; i < yResult.Count; ++i)
      {
        yResult[i] -= spl.GetYOfX(x[i]);
      }
    }
  }
}
