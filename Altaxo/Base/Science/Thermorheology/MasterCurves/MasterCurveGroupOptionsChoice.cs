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

#nullable enable
namespace Altaxo.Science.Thermorheology.MasterCurves
{
  /// <summary>
  /// Choices for the interpolation function used for master curve interpolation.
  /// </summary>
  public enum MasterCurveGroupOptionsChoice
  {
    /// <summary>Use the same interpolation function for all curve groups.</summary>
    SameForAllGroups,

    /// <summary>For each curve group, use a separate interpolation function.</summary>
    SeparateForEachGroup,

    /// <summary>When there are exactly two groups, use a complex interpolation function.</summary>
    ForComplex,
  }
}
