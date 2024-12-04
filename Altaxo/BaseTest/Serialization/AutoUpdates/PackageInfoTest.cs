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
using System.Runtime.InteropServices;
using Altaxo.Main.Services;
using Xunit;

namespace Altaxo.Serialization.AutoUpdates
{
  public class PackageInfoTest
  {
    private static readonly string Test1_Line1 =
  """
  {
    "SerializationVersion": 1,
    "PackageVersion": "4.8.3242.0",
    "IsUnstable": true,
    "FileName": "AltaxoBinaries-4.8.3242.0.zip",
    "FileLength": 157205920,
    "FileHash": {
      "Name": "SHA256",
      "Value": "CD0F8469B617D1C3624A47934E6F2E4953B202E02187697DC486F48AD2B8C596"
    },
    "RequiredNetFrameworkVersion": "4.8"
  }
  """;

    private static readonly string Test1_Line2 =
      """
      {
        "SerializationVersion": 1,
        "PackageVersion": "4.8.3243.0",
        "IsUnstable": true,
        "FileName": "AltaxoBinaries-4.8.3243.0-WINDOWS-DotNet9.0.zip",
        "FileLength": 130066234,
        "FileHash": {
          "Name": "SHA256",
          "Value": "006D1811A812C9AFF0E65D4489793AA70E48AD2325D0DACDA9E1FCCB795D005C"
        },
        "RequiredDotNetVersion": "9.0",
        "RequiredOperatingSystem": [
          {
            "OSPlatform": "WINDOWS",
            "OSVersion": "10.0.19045"
          }
        ]
      }
      """;


    private static readonly string Test1_Line3 =
      """
      {
        "SerializationVersion": 1,
        "PackageVersion": "4.8.3243.0",
        "IsUnstable": true,
        "FileName": "AltaxoBinaries-4.8.3243.0-WINDOWS-X64-Net4.8.zip",
        "FileLength": 135277552,
        "FileHash": {
          "Name": "SHA256",
          "Value": "5F033C80A8EA9BBA3E1865710E3C65156B0693190DEDC1D719D3587E9D565C7D"
        },
        "RequiredNetFrameworkVersion": "4.8",
        "RequiredArchitecture": [
          "X64"
        ],
        "RequiredOperatingSystem": [
          {
            "OSPlatform": "WINDOWS",
            "OSVersion": "10.0.19045"
          }
        ]
      }
      """;



    private static readonly string Test1_Lines =
      """
      [
        {
          "SerializationVersion": 1,
          "PackageVersion": "4.8.3242.0",
          "IsUnstable": true,
          "FileName": "AltaxoBinaries-4.8.3242.0.zip",
          "FileLength": 157205920,
          "FileHash": {
            "Name": "SHA256",
            "Value": "CD0F8469B617D1C3624A47934E6F2E4953B202E02187697DC486F48AD2B8C596"
          },
          "RequiredNetFrameworkVersion": "4.8"
        },
        {
          "SerializationVersion": 1,
          "PackageVersion": "4.8.3243.0",
          "IsUnstable": true,
          "FileName": "AltaxoBinaries-4.8.3243.0-WINDOWS-DotNet9.0.zip",
          "FileLength": 130066234,
          "FileHash": {
            "Name": "SHA256",
            "Value": "006D1811A812C9AFF0E65D4489793AA70E48AD2325D0DACDA9E1FCCB795D005C"
          },
          "RequiredDotNetVersion": "9.0",
          "RequiredOperatingSystem": [
            {
              "OSPlatform": "WINDOWS",
              "OSVersion": "10.0.19045"
            }
          ]
        },
        {
          "SerializationVersion": 1,
          "PackageVersion": "4.8.3243.0",
          "IsUnstable": true,
          "FileName": "AltaxoBinaries-4.8.3243.0-WINDOWS-X64-Net4.8.zip",
          "FileLength": 135277552,
          "FileHash": {
            "Name": "SHA256",
            "Value": "5F033C80A8EA9BBA3E1865710E3C65156B0693190DEDC1D719D3587E9D565C7D"
          },
          "RequiredNetFrameworkVersion": "4.8",
          "RequiredArchitecture": [
            "X64"
          ],
          "RequiredOperatingSystem": [
            {
              "OSPlatform": "WINDOWS",
              "OSVersion": "10.0.19045"
            }
          ]
        }
      ]
      """;

    /// <summary>
    /// Test different lines for parsability.
    /// </summary>
    [Fact]
    public void TestLineParser()
    {
      var memstream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Test1_Line1));
      var pkg = PackageInfo.ReadPackageFromJson(memstream);
      VerifyPackage1(pkg);
      memstream.Dispose();

      memstream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Test1_Line2));
      pkg = PackageInfo.ReadPackageFromJson(memstream);
      VerifyPackage2(pkg);
      memstream.Dispose();

      memstream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Test1_Line3));
      pkg = PackageInfo.ReadPackageFromJson(memstream);
      VerifyPackage3(pkg);
      memstream.Dispose();
    }

    [Fact]
    public void TestFullParser()
    {
      var memstream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Test1_Lines));
      var packages = PackageInfo.ReadPackagesFromJson(memstream);
      Assert.Equal(3, packages.Length);
      VerifyPackage1(packages[0]);
      VerifyPackage2(packages[1]);
      VerifyPackage3(packages[2]);

      PackageInfo pkg;
      // inject service
      lock (SystemRequirementsHelper.ServiceLocker)
      {
        Current.Services = new AltaxoServiceContainer();
        Current.AddService<ISystemRequirementsDetermination>(new SystemRequirements_DotNet9IsNotInstalled());
        pkg = SystemRequirements.TryGetHighestVersion(packages);
      }

      Assert.NotNull(pkg);
      Assert.Equal("AltaxoBinaries-4.8.3243.0-WINDOWS-X64-Net4.8.zip", pkg.FileNameOfPackageZipFile);

      lock (SystemRequirementsHelper.ServiceLocker)
      {
        Current.Services = new AltaxoServiceContainer();
        Current.AddService<ISystemRequirementsDetermination>(new SystemRequirements_DotNet9IsInstalled());
        pkg = SystemRequirements.TryGetHighestVersion(packages);
      }

      Assert.NotNull(pkg);
      Assert.Equal("AltaxoBinaries-4.8.3243.0-WINDOWS-DotNet9.0.zip", pkg.FileNameOfPackageZipFile);

    }
    private static PackageInfo VerifyPackage1(PackageInfo pkg)
    {
      Assert.True(pkg.IsUnstableVersion);
      Assert.True(pkg.IsOldStyleFile);
      Assert.Equal(new Version(4, 8, 3242, 0), pkg.Version);
      Assert.Equal(157205920, pkg.FileLength);
      Assert.Equal("SHA256", pkg.HashName.Name);
      Assert.Equal("CD0F8469B617D1C3624A47934E6F2E4953B202E02187697DC486F48AD2B8C596", pkg.Hash);
      Assert.Equal("AltaxoBinaries-4.8.3242.0.zip", pkg.FileNameOfPackageZipFile);
      Assert.NotNull(pkg.RequiredNetFrameworkVersion);
      Assert.Null(pkg.RequiredDotNetVersion);
      Assert.Empty(pkg.RequiredArchitectures);
      Assert.Empty(pkg.RequiredOperatingSystems);
      Assert.Equal(new Version(4, 8), pkg.RequiredNetFrameworkVersion);
      return pkg;
    }
    private static void VerifyPackage2(PackageInfo pkg)
    {
      Assert.True(pkg.IsUnstableVersion);
      Assert.False(pkg.IsOldStyleFile);
      Assert.Equal(new Version(4, 8, 3243, 0), pkg.Version);
      Assert.Equal(130066234, pkg.FileLength);
      Assert.Equal("SHA256", pkg.HashName.Name);
      Assert.Equal("006D1811A812C9AFF0E65D4489793AA70E48AD2325D0DACDA9E1FCCB795D005C", pkg.Hash);
      Assert.Equal("AltaxoBinaries-4.8.3243.0-WINDOWS-DotNet9.0.zip", pkg.FileNameOfPackageZipFile);
      Assert.Null(pkg.RequiredNetFrameworkVersion);
      Assert.NotNull(pkg.RequiredDotNetVersion);
      Assert.Empty(pkg.RequiredArchitectures);
      Assert.Single(pkg.RequiredOperatingSystems);
      Assert.Equal(new Version(9, 0), pkg.RequiredDotNetVersion);
      Assert.Equal(OSPlatform.Windows, pkg.RequiredOperatingSystems[0].Platform);
      Assert.Equal(new Version(10, 0, 19045), pkg.RequiredOperatingSystems[0].Version);
    }

    private static void VerifyPackage3(PackageInfo pkg)
    {
      Assert.True(pkg.IsUnstableVersion);
      Assert.False(pkg.IsOldStyleFile);
      Assert.Equal(new Version(4, 8, 3243, 0), pkg.Version);
      Assert.Equal(135277552, pkg.FileLength);
      Assert.Equal("SHA256", pkg.HashName.Name);
      Assert.Equal("5F033C80A8EA9BBA3E1865710E3C65156B0693190DEDC1D719D3587E9D565C7D", pkg.Hash);
      Assert.Equal("AltaxoBinaries-4.8.3243.0-WINDOWS-X64-Net4.8.zip", pkg.FileNameOfPackageZipFile);
      Assert.NotNull(pkg.RequiredNetFrameworkVersion);
      Assert.Null(pkg.RequiredDotNetVersion);
      Assert.Single(pkg.RequiredArchitectures);
      Assert.Single(pkg.RequiredOperatingSystems);

      Assert.Equal(new Version(4, 8), pkg.RequiredNetFrameworkVersion);
      Assert.Equal(Architecture.X64, pkg.RequiredArchitectures[0]);
      Assert.Equal(OSPlatform.Windows, pkg.RequiredOperatingSystems[0].Platform);
      Assert.Equal(new Version(10, 0, 19045), pkg.RequiredOperatingSystems[0].Version);
    }




  }
}

