/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Altaxo.Serialization.PrincetonInstruments
{
  /// <summary>
  /// Reader for Princeton Instruments SPE files. Those files are binary files with an Xml section containing the document metadata at the end of the file.
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1]: Python code <see cref="https://github.com/ashirsch/spe2py"/>.</para>
  /// </remarks>
  public class PrincetonInstrumentsSPEReader
  {
    /// <summary>
    /// The data type used in SPE files.
    /// </summary>
    public enum SPEDataType
    {
      Single = 0,
      Int32 = 1,
      Int16 = 2,
      UInt16 = 3,
      UInt32 = 8,
    };

    /// <summary>
    /// Region of Interest record.
    /// </summary>
    public record RegionOfInterest(int x, int xBinning, int width, int y, int yBinning, int height)
    {

    }

    /// <summary>
    /// Gets the x values.
    /// </summary>
    public double[] XValues { get; }

    /// <summary>
    /// Gets a value indicating whether the x values are shift values.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the x values are shift values in inverse centimeters; otherwise, if <c>false</c>, the XValues are wavelength values in nm.
    /// </value>
    public bool XValuesAreShiftValues => LaserWavelength.HasValue;

    private List<List<double[,]>> _data = new();
    /// <summary>
    /// Gets the data. The first index is the frame number. The second index is the index of the region of interest.
    /// Thus, Data[0][0] will return the data array of the first frame and the first region of interest.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double[,]>> Data => _data;

    public IReadOnlyList<(int width, int height)> Dimensions { get; }

    /// <summary>
    /// Gets the number of frames.
    /// </summary>
    public int NumberOfFrames { get; private set; }

    /// <summary>
    /// Gets the number of regions of interest.
    /// </summary>
    public int NumberOfRegionsOfInterest { get; private set; }

    /// <summary>
    /// Gets the version of the SPE file.
    /// </summary>
    public float Version { get; }

    /// <summary>
    /// Gets the complete metadata set of the SPE file.
    /// </summary>
    public XmlDocument DocumentMetaData { get; }

    /// <summary>
    /// Gets the laser wavelength in nm. If a laser wavelength value was given, the wavelength values of the x-axis are converted to shift values in inverse cm.
    /// </summary>
    public double? LaserWavelength { get; }

    /// <summary>
    /// Gets the type of the data used in the SPE files. In this reader, all data is converted to double values.
    /// </summary>
    public SPEDataType DataType { get; }

    /// <summary>
    /// Gets the regions of interest.
    /// </summary>
    public IReadOnlyList<RegionOfInterest> RegionsOfInterest { get; }

    /// <summary>
    /// Gets the meta data names. Meta data are given separately for each frame.
    /// Thus, the names here refer to the second dimension of the <see cref="FrameMetaDataValues"/> array. 
    /// </summary>
    public IReadOnlyList<string> FrameMetaDataNames { get; }

    /// <summary>
    /// Gets the meta data. Meta data are given separately for each frame. Thus, the first dimension is the frame index.
    /// The second dimension is the various metadata, whose name is given in the <see cref="FrameMetaDataNames"/> list.
    /// </summary>
    public double[,] FrameMetaDataValues { get; }

    private const int Location_DataTypeCode = 108;
    private const int Location_FooterPosition = 678;
    private const int Location_NumberOfFrames = 1446;
    private const int Location_VersionNumber = 1992;
    private const int Location_StartDataBlock = 4100;
    private const float MinimalVersionNumber = 3f;


    /// <summary>
    /// Initializes a new instance of the <see cref="PrincetonInstrumentsSPEReader"/> class.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    public PrincetonInstrumentsSPEReader(Stream stream)
    {
      var buffer = new byte[Location_StartDataBlock];

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
      stream.ForcedRead(buffer, 0, buffer.Length);
      var dataTypeCode = BitConverter.ToUInt16(buffer, Location_DataTypeCode);
After:
      FileIOExtensions.ReadExactly(stream, buffer, 0, buffer.Length);
      var dataTypeCode = BitConverter.ToUInt16(buffer, Location_DataTypeCode);
*/
      stream.ReadExactly(buffer, 0, buffer.Length);
      var dataTypeCode = BitConverter.ToUInt16(buffer, Location_DataTypeCode);
      var footerPosition = BitConverter.ToInt64(buffer, Location_FooterPosition);
      NumberOfFrames = (int)BitConverter.ToUInt16(buffer, Location_NumberOfFrames);
      Version = BitConverter.ToSingle(buffer, Location_VersionNumber);
      if (!(Version >= MinimalVersionNumber))
      {
        throw new InvalidDataException($"The version of SPE files must be at least {MinimalVersionNumber}, but it actually is {Version}");
      }

      DataType = (SPEDataType)dataTypeCode;

      var sizeOfDataElement = DataType switch
      {
        SPEDataType.Single => sizeof(Single),
        SPEDataType.Int32 => sizeof(Int32),
        SPEDataType.Int16 => sizeof(Int16),
        SPEDataType.UInt16 => sizeof(UInt16),
        SPEDataType.UInt32 => sizeof(UInt32),
        _ => throw new InvalidDataException($"Unknown data type code: {dataTypeCode}")
      };

      // read the footer
      stream.Seek(footerPosition, SeekOrigin.Begin);
      var doc = new System.Xml.XmlDocument();
      doc.Load(stream);
      DocumentMetaData = doc;
      //doc.Save(@"C:\Temp\metadata.xml");

      // get dimensions
      var node = doc["SpeFormat"]["DataFormat"]["DataBlock"];
      var dimensions = new List<(int width, int height)>();
      foreach (XmlElement child in node.ChildNodes)
      {
        dimensions.Add((XmlConvert.ToInt32(child.GetAttribute("width")), XmlConvert.ToInt32(child.GetAttribute("height"))));
      }
      Dimensions = dimensions.AsReadOnly();

      // get regions of interest
      var cameraSettings = doc["SpeFormat"]["DataHistories"]["DataHistory"]["Origin"]["Experiment"]["Devices"]["Cameras"]["Camera"];
      var customRegions = cameraSettings["ReadoutControl"]["RegionsOfInterest"]["CustomRegions"];
      var roiList = new List<RegionOfInterest>();
      foreach (XmlElement roi in customRegions.ChildNodes)
      {
        var entry = new RegionOfInterest(
          XmlConvert.ToInt32(roi.GetAttribute("x")),
          XmlConvert.ToInt32(roi.GetAttribute("xBinning")),
          XmlConvert.ToInt32(roi.GetAttribute("width")),
          XmlConvert.ToInt32(roi.GetAttribute("y")),
          XmlConvert.ToInt32(roi.GetAttribute("yBinning")),
          XmlConvert.ToInt32(roi.GetAttribute("height"))
          );
        roiList.Add(entry);
      }
      RegionsOfInterest = roiList.AsReadOnly();
      NumberOfRegionsOfInterest = RegionsOfInterest.Count;

      // extract the wavelength, calculate Raman shift if a wavelength is given
      var wavelength_txt = doc["SpeFormat"]["Calibrations"]["WavelengthMapping"]["Wavelength"].InnerText.Split(',');
      XValues = new double[wavelength_txt.Length];
      for (int i = 0; i < wavelength_txt.Length; i++)
      {
        XValues[i] = XmlConvert.ToDouble(wavelength_txt[i]);
      }
      var laserLineText = doc["SpeFormat"]["Calibrations"]["WavelengthMapping"].GetAttribute("laserLine");
      if (!string.IsNullOrEmpty(laserLineText))
      {
        LaserWavelength = XmlConvert.ToDouble(laserLineText);
        for (int i = 0; i < wavelength_txt.Length; i++)
        {
          XValues[i] = 1E7 * (1 / LaserWavelength.Value - 1 / XValues[i]);
        }
      }

      // get coordinates
      var xcoord = new int[NumberOfRegionsOfInterest][];
      var ycoord = new int[NumberOfRegionsOfInterest][];
      IEnumerable<int> RangeByStartEndStep(int start, int endExclusive, int step)
      {
        for (int i = start; i < endExclusive; i += step)
        {
          yield return i;
        }
      }
      for (int idxROI = 0; idxROI < NumberOfRegionsOfInterest; idxROI++)
      {
        var (xstart, xbinning, xheight, ystart, ybinning, yheight) = RegionsOfInterest[idxROI];
        xcoord[idxROI] = RangeByStartEndStep(xstart, (xstart + xheight), xbinning).ToArray();
        ycoord[idxROI] = RangeByStartEndStep(ystart, (ystart + yheight), ybinning).ToArray();
      }

      // read the data 
      stream.Seek(4100, SeekOrigin.Begin);
      var frameStride = XmlConvert.ToInt32(doc["SpeFormat"]["DataFormat"]["DataBlock"].GetAttribute("stride"));
      var frameSize = XmlConvert.ToInt32(doc["SpeFormat"]["DataFormat"]["DataBlock"].GetAttribute("size"));
      var metadataSize = frameStride - frameSize;
      List<string> meta_types = null;
      List<string> meta_names = null;

      if (metadataSize != 0)
      {
        meta_types = new List<string>();
        meta_names = new List<string>();
        string previousItem = null;

        foreach (XmlElement item in doc["SpeFormat"]["MetaFormat"]["MetaBlock"])
        {
          if (item.Name == "TimeStamp" && previousItem != "TimeStap") // Specify ExposureStarted vs. ExposureEnded
          {
            foreach (XmlElement element in item)
            {
              meta_names.Add(element.GetAttribute("event"));
              meta_types.Add(element.GetAttribute("type"));
            }
            previousItem = item.Name;
          }
          else if (item.Name == "GateTracking" && previousItem != "GateTracking") // Specify Delay vs. Width
          {
            foreach (XmlElement element in item)
            {
              meta_names.Add(element.GetAttribute("component"));
              meta_types.Add(element.GetAttribute("type"));
            }
            previousItem = item.Name;
          }
          else if (item.Name != previousItem) // All other metablock names only have one possible value
          {
            meta_names.Add(item.Name);
            meta_types.Add(item.GetAttribute("type"));
          }
        }
        FrameMetaDataValues = new double[NumberOfFrames, meta_types.Count];
        FrameMetaDataNames = meta_names.AsReadOnly();
      } // if metadata size != 0
      else
      {
        FrameMetaDataValues = new double[0, 0];
        FrameMetaDataNames = [];
      }

      for (var idxFrame = 0; idxFrame < NumberOfFrames; idxFrame++)
      {
        var frameList = new List<double[,]>();
        _data.Add(frameList);
        for (var idxRegion = 0; idxRegion < NumberOfRegionsOfInterest; idxRegion++)
        {
          int xdim, ydim;
          if (NumberOfRegionsOfInterest > 1)
          {
            xdim = xcoord[idxRegion].Length;
            ydim = ycoord[idxRegion].Length;
          }
          else
          {
            (xdim, ydim) = dimensions[0];
          }

          var arr = new double[ydim, xdim];
          var bufferArr = new byte[sizeOfDataElement * xdim * ydim];

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
          stream.ForcedRead(bufferArr, 0, bufferArr.Length);
          var ptr = 0;
After:
          FileIOExtensions.ReadExactly(stream, bufferArr, 0, bufferArr.Length);
          var ptr = 0;
*/
          stream.ReadExactly(bufferArr, 0, bufferArr.Length);
          var ptr = 0;
          for (int i = 0; i < xdim; i++)
          {
            for (int j = 0; j < ydim; j++)
            {
              switch (DataType)
              {
                case SPEDataType.Single:
                  arr[j, i] = BitConverter.ToSingle(bufferArr, ptr);
                  break;
                case SPEDataType.Int32:
                  arr[j, i] = BitConverter.ToInt32(bufferArr, ptr);
                  break;
                case SPEDataType.Int16:
                  arr[j, i] = BitConverter.ToInt16(bufferArr, ptr);
                  break;
                case SPEDataType.UInt32:
                  arr[j, i] = BitConverter.ToUInt32(bufferArr, ptr);
                  break;
                case SPEDataType.UInt16:
                  arr[j, i] = BitConverter.ToUInt16(bufferArr, ptr);
                  break;
              }
              ptr += sizeOfDataElement;
            }
          }
          frameList.Add(arr);
        } // for each region
        if (meta_types is not null)
        {
          for (int idxMetaBlock = 0; idxMetaBlock < meta_types.Count; idxMetaBlock++)
          {
            var type = meta_types[idxMetaBlock];
            // Note: the meta data type is either Int64 or Double, thus always read-in 8 bytes

/* Unmerged change from project 'AltaxoCore (net8.0)'
Before:
            stream.ForcedRead(buffer, 0, 8);
            if (type == "Int64")
After:
            FileIOExtensions.ReadExactly(stream, buffer, 0, 8);
            if (type == "Int64")
*/
            stream.ReadExactly(buffer, 0, 8);
            if (type == "Int64")
            {
              FrameMetaDataValues[idxFrame, idxMetaBlock] = BitConverter.ToInt64(buffer, 0);
            }
            else
            {
              FrameMetaDataValues[idxFrame, idxMetaBlock] = BitConverter.ToDouble(buffer, 0);
            }
          }
        }
      } // for each frame
    }
  }
}
