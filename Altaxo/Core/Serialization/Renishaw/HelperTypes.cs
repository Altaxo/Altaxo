#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger, T.Tian, Alex Henderson
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
using System.Threading.Tasks;

namespace Altaxo.Serialization.Renishaw
{
  /// <summary>
  /// Types of scan supported by Renishaw data files.
  /// </summary>
  public enum ScanType
  {
    /// <summary>Scan type not specified.</summary>
    Unspecified = 0,
    /// <summary>A static (single point) measurement.</summary>
    Static = 1,
    /// <summary>A continuous (streaming) scan.</summary>
    Continuous = 2,
    /// <summary>A step-and-repeat acquisition.</summary>
    StepRepeat = 3,
    /// <summary>A filter scan.</summary>
    FilterScan = 4,
    /// <summary>A filtered image acquisition.</summary>
    FilterImage = 5,
    /// <summary>StreamLine scan mode.</summary>
    StreamLine = 6,
    /// <summary>High-resolution StreamLine mode.</summary>
    StreamLineHR = 7,
    /// <summary>Measurement using a point detector.</summary>
    PointDetector = 8,
  }

  /// <summary>
  /// Types of measurement contained in the data.
  /// </summary>
  public enum MeasurementType
  {
    /// <summary>Measurement type not specified.</summary>
    Unspecified = 0,
    /// <summary>A single measurement.</summary>
    Single = 1,
    /// <summary>A series of measurements.</summary>
    Series = 2,
    /// <summary>A mapping (spatial) measurement.</summary>
    Mapping = 3,
  }

  /// <summary>
  /// Units that may be used for axis or metadata values.
  /// </summary>
  public enum UnitType
  {
    /// <summary>Arbitrary units.</summary>
    Arbitrary = 0,
    /// <summary>Raman shift, typically in inverse centimetres (cm^-1).</summary>
    RamanShift = 1,   // cm^-1 by default
    /// <summary>Wavenumber unit (legacy naming; often represented as nanometres).</summary>
    Wavenumber = 2,   // nm
    /// <summary>Nanometre (nm).</summary>
    Nanometre = 3,
    /// <summary>Electron volt (eV).</summary>
    ElectronVolt = 4,
    /// <summary>Micrometre (µm).</summary>
    Micron = 5,   // same for EXIF units
    /// <summary>Counts (detector counts).</summary>
    Counts = 6,
    /// <summary>Number of electrons (sensor-specific).</summary>
    Electrons = 7,
    /// <summary>Millimetres (mm).</summary>
    Millimetres = 8,
    /// <summary>Metres (m).</summary>
    Metres = 9,
    /// <summary>Kelvin (K).</summary>
    Kelvin = 10,
    /// <summary>Pascal (Pa).</summary>
    Pascal = 11,
    /// <summary>Seconds (s).</summary>
    Seconds = 12,
    /// <summary>Milliseconds (ms).</summary>
    Milliseconds = 13,
    /// <summary>Hours.</summary>
    Hours = 14,
    /// <summary>Days.</summary>
    Days = 15,
    /// <summary>Pixels (image units).</summary>
    Pixels = 16,
    /// <summary>Intensity (generic).</summary>
    Intensity = 17,
    /// <summary>Relative intensity (unitless ratio).</summary>
    RelativeIntensity = 18,
    /// <summary>Degrees (angle).</summary>
    Degrees = 19,
    /// <summary>Radians (angle).</summary>
    Radians = 20,
    /// <summary>Celsius (°C).</summary>
    Celsius = 21,
    /// <summary>Fahrenheit (°F).</summary>
    Fahrenheit = 22,
    /// <summary>Kelvin per minute (K/min).</summary>
    KelvinPerMinute = 23,
    /// <summary>File time (platform specific timestamp representation).</summary>
    FileTime = 24,
    /// <summary>Microseconds (µs).</summary>
    Microseconds = 25,
  }

  /// <summary>
  /// Semantic meaning of data channels or fields in Renishaw data structures.
  /// </summary>
  public enum DataType
  {
    /// <summary>Arbitrary data type.</summary>
    Arbitrary = 0,
    /// <summary>Frequency data.</summary>
    Frequency = 1,
    /// <summary>Intensity data.</summary>
    Intensity = 2,
    /// <summary>Spatial X coordinate.</summary>
    Spatial_X = 3,
    /// <summary>Spatial Y coordinate.</summary>
    Spatial_Y = 4,
    /// <summary>Spatial Z coordinate.</summary>
    Spatial_Z = 5,
    /// <summary>Radial spatial coordinate (R).</summary>
    Spatial_R = 6,
    /// <summary>Polar angle (theta).</summary>
    Spatial_Theta = 7,
    /// <summary>Azimuthal angle (phi).</summary>
    Spatial_Phi = 8,
    /// <summary>Temperature.</summary>
    Temperature = 9,
    /// <summary>Pressure.</summary>
    Pressure = 10,
    /// <summary>Time value.</summary>
    Time = 11,
    /// <summary>Derived quantity.</summary>
    Derived = 12,
    /// <summary>Polarization information.</summary>
    Polarization = 13,
    /// <summary>Focus tracking information.</summary>
    FocusTrack = 14,
    /// <summary>Ramp rate (e.g. temperature ramp).</summary>
    RampRate = 15,
    /// <summary>Checksum field.</summary>
    Checksum = 16,
    /// <summary>Flags (bitfield).</summary>
    Flags = 17,
    /// <summary>Elapsed time.</summary>
    ElapsedTime = 18,
    /// <summary>Spectral data channel.</summary>
    Spectral = 19,
    /// <summary>Multiprobe well spatial X.</summary>
    Mp_Well_Spatial_X = 22,
    /// <summary>Multiprobe well spatial Y.</summary>
    Mp_Well_Spatial_Y = 23,
    /// <summary>Multiprobe location index.</summary>
    Mp_LocationIndex = 24,
    /// <summary>Multiprobe well reference.</summary>
    Mp_WellReference = 25,
    /// <summary>End marker for data blocks.</summary>
    EndMarker = 26,
    /// <summary>Exposure time for detector.</summary>
    ExposureTime = 27,
  }
}
