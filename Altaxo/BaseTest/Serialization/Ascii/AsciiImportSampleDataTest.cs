using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Altaxo.Serialization.Ascii
{
  public class AsciiImportSampleDataTest
  {
    public string GetSampleDataDirectory()
    {
      var callingAssemblyLocation = AppDomain.CurrentDomain.BaseDirectory;
      var nameParts = GetType().Namespace.Split(new[] { '.' }).Skip(1).ToArray();
      var subPath = string.Join("\\", nameParts);
      return Path.Combine(callingAssemblyLocation, subPath, "SampleData");
    }

    [Fact]
    public void TestSampleData1()
    {
      var fileName = Path.Combine(GetSampleDataDirectory(), "SampleData1.txt");

      foreach (var linesToAnalyze in new int[] { 50, 300, 5000 })
      {
        // we will test the analysis of this sample data both with the German and the invariant culture
        var GermanCulture = System.Globalization.CultureInfo.GetCultureInfo("de");
        var analysisOptions = AsciiDocumentAnalysisOptions.GetOptionsForCultures(
            System.Globalization.CultureInfo.InvariantCulture, GermanCulture);
        analysisOptions.NumberOfLinesToAnalyze = linesToAnalyze;

        AsciiImportOptions analysis;
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          analysis = AsciiDocumentAnalysis.Analyze(new AsciiImportOptions(), stream, analysisOptions);
        }

        Assert.Equal(3, analysis.NumberOfMainHeaderLines);
        Assert.Equal(0, analysis.IndexOfCaptionLine);

        Assert.IsType<SingleCharSeparationStrategy>(analysis.SeparationStrategy);
        Assert.Equal('\t', ((SingleCharSeparationStrategy)analysis.SeparationStrategy).SeparatorChar); // tab as separator char
        Assert.Equal(25, analysis.RecognizedStructure.Count); // file consists of 25 columns
        Assert.True("iv" == analysis.NumberFormatCulture.TwoLetterISOLanguageName); // invariant culture
        Assert.True("iv" == analysis.DateTimeFormatCulture.TwoLetterISOLanguageName); // invariant culture
        Assert.True(analysis.IsFullySpecified);

        Assert.True(AsciiColumnType.Double == analysis.RecognizedStructure[0].ColumnType);
        Assert.True(AsciiColumnType.Double == analysis.RecognizedStructure[1].ColumnType || AsciiColumnType.Int64 == analysis.RecognizedStructure[1].ColumnType);
        Assert.True(AsciiColumnType.DateTime == analysis.RecognizedStructure[2].ColumnType);

        for (int i = 3; i <= 12; ++i)
        {
          Assert.True(AsciiColumnType.Double == analysis.RecognizedStructure[i].ColumnType || AsciiColumnType.Int64 == analysis.RecognizedStructure[i].ColumnType);
        }
        for (int i = 13; i <= 16; ++i)
        {
          Assert.True(AsciiColumnType.DBNull == analysis.RecognizedStructure[i].ColumnType);
        }
        for (int i = 17; i <= 19; ++i)
        {
          Assert.True(AsciiColumnType.Double == analysis.RecognizedStructure[i].ColumnType || AsciiColumnType.Int64 == analysis.RecognizedStructure[i].ColumnType);
        }
        for (int i = 20; i <= 23; ++i)
        {
          Assert.True(AsciiColumnType.DBNull == analysis.RecognizedStructure[i].ColumnType);
        }
        for (int i = 24; i <= 24; ++i)
        {
          Assert.True(AsciiColumnType.Double == analysis.RecognizedStructure[i].ColumnType || AsciiColumnType.Int64 == analysis.RecognizedStructure[i].ColumnType);
        }
      }
    }


    [Fact]
    public void TestSampleData2()
    {
      var fileName = Path.Combine(GetSampleDataDirectory(), "SampleData2.txt");

      foreach (var linesToAnalyze in new int[] { 30, 300, 5000 })
      {
        // we will test the analysis of this sample data both with the German and the invariant culture
        var GermanCulture = System.Globalization.CultureInfo.GetCultureInfo("de");
        var analysisOptions = AsciiDocumentAnalysisOptions.GetOptionsForCultures(
            System.Globalization.CultureInfo.InvariantCulture, GermanCulture);
        analysisOptions.NumberOfLinesToAnalyze = linesToAnalyze;

        AsciiImportOptions analysis;
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          analysis = AsciiDocumentAnalysis.Analyze(new AsciiImportOptions(), stream, analysisOptions);
        }

        Assert.Equal(3, analysis.NumberOfMainHeaderLines);
        Assert.Equal(1, analysis.IndexOfCaptionLine);

        Assert.IsType<SingleCharSeparationStrategy>(analysis.SeparationStrategy);
        Assert.Equal('\t', ((SingleCharSeparationStrategy)analysis.SeparationStrategy).SeparatorChar); // tab as separator char
        Assert.Equal(4, analysis.RecognizedStructure.Count); // file consists of 4 columns
        Assert.True("iv" == analysis.NumberFormatCulture.TwoLetterISOLanguageName); // invariant culture
        Assert.True(analysis.IsFullySpecified);

        Assert.Equal(AsciiColumnType.Double, analysis.RecognizedStructure[0].ColumnType);
        Assert.True(AsciiColumnType.Double == analysis.RecognizedStructure[1].ColumnType || AsciiColumnType.Int64 == analysis.RecognizedStructure[1].ColumnType);
        Assert.Equal(AsciiColumnType.Double, analysis.RecognizedStructure[2].ColumnType);
        Assert.True(AsciiColumnType.Double == analysis.RecognizedStructure[3].ColumnType || AsciiColumnType.DBNull == analysis.RecognizedStructure[3].ColumnType);
      }
    }
  }
}
