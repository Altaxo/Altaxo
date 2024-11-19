#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using Xunit;

namespace Altaxo.Serialization.AutoUpdates
{
  public class PackageInfoTest
  {
    private static readonly string[] Lines = new[]
    {
      "Unstable\t4.8.3228.0\t253216686\tB9898BE3301B057DA6B1BA41A52A98F6C232F08C\tRequiredNetFrameworkVersion=4.8\r\n",
      "Stable\t4.8.3228.0\t253216686\tB9898BE3301B057DA6B1BA41A52A98F6C232F08C\tRequiredNetFrameworkVersion=4.8\r\n",
      "Unstable\tx64_4.8.3228.0\t253216686\tB9898BE3301B057DA6B1BA41A52A98F6C232F08C\tRequiredNetFrameworkVersion=4.8\r\n",
      "Unstable\tx64_4.8.3228.0\t253216686\tB9898BE3301B057DA6B1BA41A52A98F6C232F08C\tRequiredNetFrameworkVersion=4.8\tRequiredDotNetVersion=9.0\tRequiredArchitecture=x64\tRequiredOperatingSystem=Windows_10762\r\n",
    };

    [Fact]
    public void Test1()
    {
      var pkg = PackageInfo.FromLine(Lines[0], 0);
      Assert.True(pkg.IsUnstableVersion);
      Assert.Empty(pkg.TargetName);
      Assert.Equal(new Version(4, 8, 3228, 0), pkg.Version);
      Assert.Equal(253216686, pkg.FileLength);
      Assert.Equal("B9898BE3301B057DA6B1BA41A52A98F6C232F08C", pkg.Hash);
      Assert.Single(pkg.Properties);
      Assert.Equal("4.8", pkg.Properties[SystemRequirements.PropertyKeyNetFrameworkVersion]);
      Assert.Equal("AltaxoBinaries-4.8.3228.0.zip", pkg.PackageFileName);

      pkg = PackageInfo.FromLine(Lines[1], 1);
      Assert.False(pkg.IsUnstableVersion);
      Assert.Empty(pkg.TargetName);
      Assert.Equal(new Version(4, 8, 3228, 0), pkg.Version);
      Assert.Equal(253216686, pkg.FileLength);
      Assert.Equal("B9898BE3301B057DA6B1BA41A52A98F6C232F08C", pkg.Hash);
      Assert.Single(pkg.Properties);
      Assert.Equal("4.8", pkg.Properties[SystemRequirements.PropertyKeyNetFrameworkVersion]);
      Assert.Equal("AltaxoBinaries-4.8.3228.0.zip", pkg.PackageFileName);

      pkg = PackageInfo.FromLine(Lines[2], 2);
      Assert.True(pkg.IsUnstableVersion);
      Assert.Equal("x64", pkg.TargetName);
      Assert.Equal(new Version(4, 8, 3228, 0), pkg.Version);
      Assert.Equal(253216686, pkg.FileLength);
      Assert.Equal("B9898BE3301B057DA6B1BA41A52A98F6C232F08C", pkg.Hash);
      Assert.Single(pkg.Properties);
      Assert.Equal("4.8", pkg.Properties[SystemRequirements.PropertyKeyNetFrameworkVersion]);
      Assert.Equal("AltaxoBinaries-x64-4.8.3228.0.zip", pkg.PackageFileName);

      pkg = PackageInfo.FromLine(Lines[3], 3);
      Assert.True(pkg.IsUnstableVersion);
      Assert.Equal("x64", pkg.TargetName);
      Assert.Equal(new Version(4, 8, 3228, 0), pkg.Version);
      Assert.Equal(253216686, pkg.FileLength);
      Assert.Equal("B9898BE3301B057DA6B1BA41A52A98F6C232F08C", pkg.Hash);
      Assert.Equal("AltaxoBinaries-x64-4.8.3228.0.zip", pkg.PackageFileName);
      Assert.Equal(4, pkg.Properties.Count);
      Assert.Equal("4.8", pkg.Properties[SystemRequirements.PropertyKeyNetFrameworkVersion]);
      Assert.Equal("9.0", pkg.Properties[SystemRequirements.PropertyKeyDotNetVersion]);
      Assert.Equal("x64", pkg.Properties[SystemRequirements.PropertyKeyArchitecture]);
      Assert.Equal("Windows_10762", pkg.Properties[SystemRequirements.PropertyKeyOperatingSystem]);
    }
  }
}
