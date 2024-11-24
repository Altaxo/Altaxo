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
using System.IO;
using Altaxo.Main.Services;
using Xunit;

namespace Altaxo.Serialization.AutoUpdates
{
  public class PackageInfoTest
  {
    private static readonly string[] Test1_Lines = new[]
    {
      "Unstable\t4.8.3228.0\t253216686\tB9898BE3301B057DA6B1BA41A52A98F6C232F08C\tRequiredNetFrameworkVersion=4.8\r\n",
      "Stable\t4.8.3228.0\t253216686\tB9898BE3301B057DA6B1BA41A52A98F6C232F08C\tRequiredNetFrameworkVersion=4.8\r\n",
      "Unstable\t4.8.3228.0\t253216686\tB9898BE3301B057DA6B1BA41A52A98F6C232F08C\tRequiredNetFrameworkVersion=4.8\tFileName=AltaxoBinaries-X64-4.8.3228.0.zip\r\n",
      "Unstable\t4.8.3228.0\t253216686\tB9898BE3301B057DA6B1BA41A52A98F6C232F08C\tRequiredNetFrameworkVersion=4.8\tRequiredDotNetVersion=9.0\tRequiredArchitecture=x64\tRequiredOperatingSystem=Windows_10.0.17763\tFileName=AltaxoBinaries-x64-4.8.3228.0.zip\r\n",
    };

    /// <summary>
    /// Test different lines for parsability.
    /// </summary>
    [Fact]
    public void TestLineParser()
    {
      var pkg = PackageInfo.FromLine(Test1_Lines[0], 0);
      Assert.True(pkg.IsUnstableVersion);
      Assert.Equal(new Version(4, 8, 3228, 0), pkg.Version);
      Assert.Equal(253216686, pkg.FileLength);
      Assert.Equal("B9898BE3301B057DA6B1BA41A52A98F6C232F08C", pkg.Hash);
      Assert.Single(pkg.Properties);
      Assert.Equal("4.8", pkg.Properties[SystemRequirements.PropertyKeyNetFrameworkVersion]);
      Assert.Equal("AltaxoBinaries-4.8.3228.0.zip", pkg.PackageFileName);

      pkg = PackageInfo.FromLine(Test1_Lines[1], 1);
      Assert.False(pkg.IsUnstableVersion);
      Assert.Equal(new Version(4, 8, 3228, 0), pkg.Version);
      Assert.Equal(253216686, pkg.FileLength);
      Assert.Equal("B9898BE3301B057DA6B1BA41A52A98F6C232F08C", pkg.Hash);
      Assert.Single(pkg.Properties);
      Assert.Equal("4.8", pkg.Properties[SystemRequirements.PropertyKeyNetFrameworkVersion]);
      Assert.Equal("AltaxoBinaries-4.8.3228.0.zip", pkg.PackageFileName);

      pkg = PackageInfo.FromLine(Test1_Lines[2], 2);
      Assert.True(pkg.IsUnstableVersion);
      Assert.Equal(new Version(4, 8, 3228, 0), pkg.Version);
      Assert.Equal(253216686, pkg.FileLength);
      Assert.Equal("B9898BE3301B057DA6B1BA41A52A98F6C232F08C", pkg.Hash);
      Assert.Equal(2, pkg.Properties.Count);
      Assert.Equal("4.8", pkg.Properties[SystemRequirements.PropertyKeyNetFrameworkVersion]);
      Assert.Equal("AltaxoBinaries-X64-4.8.3228.0.zip", pkg.PackageFileName);

      pkg = PackageInfo.FromLine(Test1_Lines[3], 3);
      Assert.True(pkg.IsUnstableVersion);
      Assert.Equal(new Version(4, 8, 3228, 0), pkg.Version);
      Assert.Equal(253216686, pkg.FileLength);
      Assert.Equal("B9898BE3301B057DA6B1BA41A52A98F6C232F08C", pkg.Hash);
      Assert.Equal("AltaxoBinaries-x64-4.8.3228.0.zip", pkg.PackageFileName);
      Assert.Equal(5, pkg.Properties.Count);
      Assert.Equal("4.8", pkg.Properties[SystemRequirements.PropertyKeyNetFrameworkVersion]);
      Assert.Equal("9.0", pkg.Properties[SystemRequirements.PropertyKeyDotNetVersion]);
      Assert.Equal("x64", pkg.Properties[SystemRequirements.PropertyKeyArchitecture]);
      Assert.Equal("Windows_10.0.17763", pkg.Properties[SystemRequirements.PropertyKeyOperatingSystem]);
    }


    private static readonly string Test2_Lines =

      "Unstable\t4.0.1447.0\t61028037\tA7F87F7FE41686AD797606B5CF2638FF184F9F21\tRequiredNetFrameworkVersion=4.0\r\n" +
      "Unstable\t4.8.3230.0\t253227706\t8FAD7BDC2B5A38D01029E8275B889BD1C2A9D6C5\tRequiredNetFrameworkVersion=4.8\r\n";

    /// <summary>
    /// Test a file with two lines with equal requirements. The parser then should choose the bottommost line.
    /// </summary>
    [Fact]
    public void TestBackwardCompatibility()
    {
      var buffer = System.Text.Encoding.UTF8.GetBytes(Test2_Lines);
      var memstream = new MemoryStream(buffer);
      var packages = PackageInfo.FromStream(memstream);

      PackageInfo pkg;
      // inject service
      lock (SystemRequirementsHelper.ServiceLocker)
      {
        Current.Services = new AltaxoServiceContainer();
        Current.AddService<ISystemRequirementsDetermination>(new SystemRequirements_DotNet9IsNotInstalled());
        pkg = PackageInfo.GetHighestVersion(packages);
      }

      Assert.NotNull(pkg);
      Assert.Equal("AltaxoBinaries-4.8.3230.0.zip", pkg.PackageFileName);
    }
  }
}

