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
  public enum ScanType
  {
    Unspecified = 0,
    Static = 1,
    Continuous = 2,
    StepRepeat = 3,
    FilterScan = 4,
    FilterImage = 5,
    StreamLine = 6,
    StreamLineHR = 7,
    PointDetector = 8,
  }

  public enum MeasurementType
  {
    Unspecified = 0,
    Single = 1,
    Series = 2,
    Mapping = 3,
  }

  public enum UnitType
  {
    Arbitrary = 0,
    RamanShift = 1,   // cm^-1 by default
    Wavenumber = 2,   // nm
    Nanometre = 3,
    ElectronVolt = 4,
    Micron = 5,   // same for EXIF units
    Counts = 6,
    Electrons = 7,
    Millimetres = 8,
    Metres = 9,
    Kelvin = 10,
    Pascal = 11,
    Seconds = 12,
    Milliseconds = 13,
    Hours = 14,
    Days = 15,
    Pixels = 16,
    Intensity = 17,
    RelativeIntensity = 18,
    Degrees = 19,
    Radians = 20,
    Celsius = 21,
    Fahrenheit = 22,
    KelvinPerMinute = 23,
    FileTime = 24,
    Microseconds = 25,
  }

  public enum DataType
  {
    Arbitrary = 0,
    Frequency = 1,
    Intensity = 2,
    Spatial_X = 3,
    Spatial_Y = 4,
    Spatial_Z = 5,
    Spatial_R = 6,
    Spatial_Theta = 7,
    Spatial_Phi = 8,
    Temperature = 9,
    Pressure = 10,
    Time = 11,
    Derived = 12,
    Polarization = 13,
    FocusTrack = 14,
    RampRate = 15,
    Checksum = 16,
    Flags = 17,
    ElapsedTime = 18,
    Spectral = 19,
    Mp_Well_Spatial_X = 22,
    Mp_Well_Spatial_Y = 23,
    Mp_LocationIndex = 24,
    Mp_WellReference = 25,
    EndMarker = 26,
    ExposureTime = 27,
  }
}
