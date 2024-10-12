using System;
using System.IO;
using Xunit;

namespace Altaxo.Serialization.PrincetonInstruments
{
  public class PrincetonInstrumentsSPEReaderTest
  {
    public string TestFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Serialization\\PrincetonInstruments\\TestFiles");

    public FileStream GetFileStream(string fileName)
    {
      return new FileStream(Path.Combine(TestFilePath, fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public PrincetonInstrumentsSPEReader GetReader(string fileName)
    {
      using var str = GetFileStream(fileName);
      return new PrincetonInstrumentsSPEReader(str);
    }

    [Fact]
    public void Test_AllFilesReadable()
    {
      var speFiles = new DirectoryInfo(TestFilePath).GetFiles("*.spe");
      foreach (var file in speFiles)
      {
        using var str = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
        var reader = new PrincetonInstrumentsSPEReader(str);
      }
    }

    [Fact]
    public void Test_BasicFileLoading_FullSensorImage()
    {
      var r = GetReader("full_sensor_image.spe");
      Assert.Equal(1, r.NumberOfFrames);
      Assert.Equal(1, r.NumberOfRegionsOfInterest);
      Assert.NotNull(r.Data);
      Assert.NotEmpty(r.Data);
      Assert.NotEmpty(r.Data[0]);
      Assert.NotNull(r.Data[0][0]);
      Assert.Equal(1024, r.Data[0][0].GetLength(0));
      Assert.Equal(1024, r.Data[0][0].GetLength(1));
    }

    [Fact]
    public void Test_BasicFileLoading_SmallRoi()
    {
      var r = GetReader("small_roi.spe");
      Assert.Equal(1, r.NumberOfFrames);
      Assert.Equal(1, r.NumberOfRegionsOfInterest);
      Assert.NotNull(r.Data);
      Assert.NotEmpty(r.Data);
      Assert.NotEmpty(r.Data[0]);
      Assert.NotNull(r.Data[0][0]);
      Assert.Equal(638, r.Data[0][0].GetLength(0));
      Assert.Equal(705, r.Data[0][0].GetLength(1));
    }

    [Fact]
    public void Test_BasicFileLoading_MultipleRoi()
    {
      var r = GetReader("two_rectangular_rois_different_binning.spe");
      Assert.Equal(1, r.NumberOfFrames);
      Assert.Equal(2, r.NumberOfRegionsOfInterest);
      Assert.NotNull(r.Data);
      Assert.NotEmpty(r.Data);
      Assert.NotEmpty(r.Data[0]);
      Assert.Equal(2, r.Data[0].Count);
      Assert.NotNull(r.Data[0][0]);
      Assert.Equal(638, r.Data[0][0].GetLength(0));
      Assert.Equal(705, r.Data[0][0].GetLength(1));
      Assert.NotNull(r.Data[0][1]);
      Assert.Equal(55, r.Data[0][1].GetLength(0));
      Assert.Equal(80, r.Data[0][1].GetLength(1));
      Assert.Equal(1, r.RegionsOfInterest[0].xBinning);
      Assert.Equal(4, r.RegionsOfInterest[1].xBinning);
      Assert.Equal(2, r.RegionsOfInterest[1].yBinning);
    }

    [Fact]
    public void Test_BasicFileLoading_OneDimension()
    {
      var r = GetReader("one_dimensional_spectrum.spe");
      Assert.Equal(1, r.NumberOfFrames);
      Assert.Equal(1, r.NumberOfRegionsOfInterest);
      Assert.NotNull(r.Data);
      Assert.NotEmpty(r.Data);
      Assert.NotEmpty(r.Data[0]);
      Assert.Single(r.Data[0]);
      Assert.NotNull(r.Data[0][0]);
      Assert.Equal(1, r.Data[0][0].GetLength(0));
      Assert.Equal(1024, r.Data[0][0].GetLength(1));
    }

    [Fact]
    public void Test_BasicFileLoading_ComplexFile()
    {
      var r = GetReader("ten_frames_two_rois_different_binning.spe");
      Assert.Equal(10, r.NumberOfFrames);
      Assert.Equal(2, r.NumberOfRegionsOfInterest);
      Assert.NotNull(r.Data);
      Assert.NotEmpty(r.Data);
      Assert.Equal(10, r.Data.Count);
      Assert.NotEmpty(r.Data[0]);
      Assert.Equal(2, r.Data[0].Count);
      Assert.NotNull(r.Data[0][0]);
      Assert.Equal(177, r.Data[0][0].GetLength(0));
      Assert.Equal(626, r.Data[0][0].GetLength(1));
      Assert.Equal(46, r.Data[0][1].GetLength(0));
      Assert.Equal(256, r.Data[0][1].GetLength(1));
    }

    [Fact]
    public void Test_BasicFileLoading_StepAndGlue()
    {
      var r = GetReader("step_and_glue.spe");
      Assert.Equal(1, r.NumberOfFrames);
      Assert.Equal(1, r.NumberOfRegionsOfInterest);
      Assert.NotNull(r.Data);
      Assert.NotEmpty(r.Data);
      Assert.Single(r.Data);
      Assert.NotEmpty(r.Data[0]);
      Assert.Single(r.Data[0]);
      Assert.NotNull(r.Data[0][0]);
      Assert.Equal(1024, r.Data[0][0].GetLength(0));
      Assert.Equal(1567, r.Data[0][0].GetLength(1));
    }


    [Fact]
    public void Test_BasicFileLoading_RamanSpectrumSilicon()
    {
      var r = GetReader("raman_spectrum_silicon.spe");
      Assert.Equal(1, r.NumberOfFrames);
      Assert.Equal(1, r.NumberOfRegionsOfInterest);
      Assert.NotNull(r.Data);
      Assert.NotEmpty(r.Data);
      Assert.Single(r.Data);
      Assert.NotEmpty(r.Data[0]);
      Assert.Single(r.Data[0]);
      Assert.NotNull(r.Data[0][0]);
      Assert.Equal(1, r.Data[0][0].GetLength(0));
      Assert.Equal(1340, r.Data[0][0].GetLength(1));
      Assert.Equal(1340, r.XValues.Length);
      Assert.True(r.XValuesAreShiftValues);
      Assert.True(r.LaserWavelength.HasValue);
    }
  }
}
