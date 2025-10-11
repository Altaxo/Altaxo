#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Information about a downloaded package.
  /// </summary>
  public class PackageInfo
  {
    #region Constants

    /// <summary>Identifier for the stable version.</summary>
    public const string StableIdentifier = "Stable";

    /// <summary>Identifier for the unstable version.</summary>
    public const string UnstableIdentifier = "Unstable";

    /// <summary>The start of the file name of the binary package file (the .zip file)</summary>
    public const string StartTextOfPackageZipFileName = "AltaxoBinaries-";

    /// <summary>The start of the file name of the Msi installation file (the .msi file)</summary>
    public const string StartTextOfPackageMsiFileName = "Altaxo-";

    /// <summary>
    /// The property key: .NET framework version
    /// </summary>
    public const string PropertyKeyNetFrameworkVersion = "RequiredNetFrameworkVersion";

    /// <summary>
    /// Property key for the required dotnet runtime version
    /// </summary>
    public const string PropertyKeyDotNetVersion = "RequiredDotNetVersion";

    /// <summary>
    /// The property for the required architecture (x86, X64, Arm, Arm64 etc.)
    /// </summary>
    public const string PropertyKeyArchitecture = "RequiredArchitecture";

    /// <summary>
    /// The property for the operating system. The value consist of a name (Windows, OSX, Linux) and a version number, separated by an underscore.
    /// </summary>
    public const string PropertyKeyOperatingSystem = "RequiredOperatingSystem";

    /// <summary>
    /// The property for the operating system. The value consist of a name (Windows, OSX, Linux) and a version number, separated by an underscore.
    /// </summary>
    public const string PropertyKeyFileName = "FileName";

    /// <summary>
    /// The property key that indicates if this package has the old style naming.
    /// </summary>
    public const string PropertyKeyIsOldStyleFile = "IsOldStyleFile";

    /// <summary>Name (without path) of the version file, both at the remote location and on the local hard disk.</summary>
    public const string VersionFileName_Before2024_11 = "VersionInfo.txt";

    /// <summary>Name (without path) of the version file, both at the remote location and on the local hard disk.</summary>
    public const string VersionFileName = "VersionInfo.json";


    #endregion

    #region Members and Properties

    /// <summary>Gets the version of the package.</summary>
    public Version Version { get; private set; } = new Version(0, 0, 0, 0);

    /// <summary>Gets the name of the hash algorithm.</summary>
    public HashAlgorithmName HashName { get; private set; } = HashAlgorithmName.SHA256;

    /// <summary>Gets the hash sum of the package file.</summary>
    public string Hash { get; private set; } = string.Empty;

    /// <summary>Gets a value indicating whether this package is the unstable or the stable build of the program.</summary>
    public bool IsUnstableVersion { get; set; }

    /// <summary>Gets the file length of the package.</summary>
    public long FileLength { get; private set; }

    /// <summary>
    /// The value is set after reading in a package from Json. It designates the file name of the package file.
    /// This value overrides the automatically created name of the package file in <see cref="FileNameOfPackageZipFile"/>.
    /// </summary>
    public string? FileNameOfPackageZipFileOverride { get; private set; }

    /// <summary>
    /// If not null, this value indicates that the net framework with at least the provided version number is required for this package.
    /// </summary>
    public Version? RequiredNetFrameworkVersion { get; private set; }

    /// <summary>
    /// If not null, this value indicates that the dotnet version with at least the provided version number is required for this package.
    /// </summary>
    public Version? RequiredDotNetVersion { get; private set; }

    /// <summary>
    /// If not empty, the array indicates the required architectures for which this package is suitable, e.g. X64, X86, ARM etc.
    /// </summary>
    public System.Runtime.InteropServices.Architecture[] RequiredArchitectures { get; private set; } = [];

    /// <summary>
    /// If not empty, the array entries indicate the required operating system for which the package is suitable. Each entry consist of
    /// the operating system (e.g., WINDOWS) and the minimal suitable version number of the operating system.
    /// </summary>
    public (System.Runtime.InteropServices.OSPlatform Platform, Version Version)[] RequiredOperatingSystems { get; private set; } = [];

    // Properties

    /// <summary>
    /// Returns either 'Unstable' or 'Stable'
    /// </summary>
    public string UnstableOrStableName => IsUnstableVersion ? UnstableIdentifier : StableIdentifier;

    /// <summary>
    /// If true, this package is an old style package. Old style packages are packages before 2024-11, which where read-in using a text file.
    /// Additionally, the hash of those packages was SHA1, and the name was in the style 'AltaxoBinaries-version.zip.
    /// </summary>
    public bool IsOldStyleFile { get; private set; }

    /// <summary>
    /// Gets the file name identifier that codes operating system, architecture, dotnet version and net framework version
    /// </summary>
    /// <value>
    /// The file name identifier. Normally the file name is then AltaxoBinaries-'FileNameIdentifier'-'Version'.zip
    /// </value>
    public string FileNameIdentifier
    {
      get
      {
        if (IsOldStyleFile)
        {
          return string.Empty; // this is especially for the file that should be detected by Altaxo versions before 2024-11
        }
        else
        {

          var stb = new StringBuilder();
          if (RequiredOperatingSystems.Length > 0)
          {
            if (stb.Length > 0)
              stb.Append('-');

            stb.Append(RequiredOperatingSystems[0].Platform);
            foreach (var os in RequiredOperatingSystems.Skip(1))
            {
              stb.Append(';');
              stb.Append(os.Platform);
            }

          }
          if (RequiredArchitectures.Length > 0)
          {
            if (stb.Length > 0)
              stb.Append('-');
            stb.Append(RequiredArchitectures[0]);
            foreach (var architecture in RequiredArchitectures.Skip(1))
            {
              stb.Append(';');
              stb.Append(architecture);
            }
          }
          if (RequiredDotNetVersion is not null)
          {
            if (stb.Length > 0)
              stb.Append('-');
            stb.Append("DotNet");
            stb.Append(RequiredDotNetVersion.ToString());
          }
          else if (RequiredNetFrameworkVersion is not null)
          {
            if (stb.Length > 0)
              stb.Append('-');
            stb.Append("Net");
            stb.Append(RequiredNetFrameworkVersion);
          }

          return stb.ToString();
        }
      }
    }

    /// <summary>
    /// Gets the file name of the package file evaluated from the package properties, without pretext (i.e. without 'AltaxoBinaries') and without extension (i.e. without .zip).
    /// </summary>
    public string FileNameWithoutPretextAndWithoutExtension
    {
      get
      {
        var fid = FileNameIdentifier;
        return string.IsNullOrEmpty(fid) ? Version.ToString(4) : $"{Version.ToString(4)}-{fid}";
      }
    }

    /// <summary>
    /// Gets the file name of the package file evaluated from the package properties, without extension (i.e. without .zip).
    /// </summary>
    public string FileNameOfPackageZipFileWithoutExtension
    {
      get
      {
        return $"{StartTextOfPackageZipFileName}{FileNameWithoutPretextAndWithoutExtension}";
      }
    }

    /// <summary>Gets the name of the package file.
    /// If the property <see cref="FileNameOfPackageZipFileOverride"/> is not set, the file name is automatically evaluated from
    /// the properties of the package (see also <see cref="FileNameOfPackageZipFileWithoutExtension"/>). If the property <see cref="FileNameOfPackageZipFileOverride"/>
    /// is set, the value of this property is returned.
    /// <value>The package file name.</value>
    public string FileNameOfPackageZipFile
    {
      get
      {
        return string.IsNullOrEmpty(FileNameOfPackageZipFileOverride) ? $"{FileNameOfPackageZipFileWithoutExtension}.zip" : FileNameOfPackageZipFileOverride;
      }
    }

    /// <summary>Gets the name of the package .msi file. If the property dictionary contain a property named 'FileName', then the value of this property is returned.
    /// Otherwise, the old behavior 'AltaxoBinaries-' and version is used.</summary>
    /// <value>The package file name.</value>
    public string FileNameOfPackageMsiFile
    {
      get
      {
        return $"{StartTextOfPackageMsiFileName}{FileNameWithoutPretextAndWithoutExtension}.msi";
      }
    }

    #endregion

    #region Input / output from / to text lines (PackageInfo.txt on SourceForge; old style; before 2024-11)

    /// <summary>Gets the package infos from the lines of the provided stream.</summary>
    /// <param name="stream">Stream to read from.</param>
    /// <returns>The package infos. If the format of the stream is invalid, various exceptions will be thrown.</returns>
    public static PackageInfo[] ReadPackagesFromText_Before2024_11(Stream stream)
    {
      var sr = new StreamReader(stream, true);

      var resultList = new List<PackageInfo>();

      int lineNumber = 0;
      while (sr.ReadLine() is string line)
      {
        ++lineNumber;
        line = line.Trim();
        if (string.IsNullOrEmpty(line))
          continue;

        var packageInfo = ReadPackageFromText(line, lineNumber);
        resultList.Add(packageInfo);
      }

      return resultList.ToArray();
    }

    /// <summary>
    /// Create a package info from a single line.
    /// </summary>
    /// <param name="line">The line to parse.</param>
    /// <param name="lineNumber">The line number (1-based; the first line has line number 1).</param>
    /// <returns>The package info parsed from that line.</returns>
    /// <exception cref="InvalidOperationException">Occurs if the line is not properly formatted.</exception>
    public static PackageInfo ReadPackageFromText(string line, int lineNumber)
    {
      line = line.Trim();
      var entries = line.Split(new char[] { '\t' }, StringSplitOptions.None);

      if (entries.Length < 4)
        throw new InvalidOperationException($"Line number {lineNumber} of the package info file doesn't contain at least 4 words, separated by tabulators");

      if (!IsValidStableIdentifier(entries[0].Trim(), out var isUnstableVersion))
        throw new InvalidOperationException($"First item in line number {lineNumber} of the package info file is neither 'stable' nor 'unstable'");

      // the version string may consist of a prefix, which designates architecture, OS, and so on, and the version itself, separated by a underline
      var version = Version.Parse(entries[1].Trim());

      Version? requiredNetFrameworkVersion;
      if (entries[4].StartsWith("RequiredNetFrameworkVersion="))
      {
        requiredNetFrameworkVersion = Version.Parse(entries[4].Substring("RequiredNetFrameworkVersion=".Length).Trim());
      }
      else
      {
        throw new InvalidOperationException("The line does not contain a RequiredNetFrameworkVersion= property.");
      }

      var result = new PackageInfo()
      {
        IsOldStyleFile = true,
        IsUnstableVersion = isUnstableVersion,
        Version = version,
        FileLength = long.Parse(entries[2].Trim()),
        HashName = HashAlgorithmName.SHA1,
        Hash = entries[3].Trim(),
        RequiredNetFrameworkVersion = requiredNetFrameworkVersion,
        FileNameOfPackageZipFileOverride = $"AltaxoBinaries-{version.ToString(4)}",
      };

      return result;
    }

    /// <summary>
    /// Writes the (old style) package to one line of text.
    /// </summary>
    /// <param name="sw">The sw.</param>
    public void WritePackageToText(StreamWriter sw)
    {
      sw.WriteLine($"{UnstableOrStableName}\t{Version.ToString(4)}\t{FileLength}\t{Hash}\tRequiredNetFrameworkVersion={RequiredNetFrameworkVersion ?? new Version(4, 8)}");
    }

    #endregion

    #region Input / output from / to Json format (PackageInfo.json on SourceForge; new style; after 2024-11)

    /// <summary>
    /// Reads multiple packages from a Json file. The outer node of the Json file has to be an array of package nodes.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The packages that were read from the Json file. Packages which could not be read will be silently ignored.</returns>
    public static PackageInfo[] ReadPackagesFromJson(Stream stream)
    {
      var doc = (JsonArray)JsonNode.Parse(stream);

      var result = new List<PackageInfo>();

      for (int i = 0; i < doc.Count; i++)
      {
        if (doc[i] is { } entry)
        {
          try
          {
            result.Add(ReadPackageFromJson(entry));
          }
          catch (Exception ex)
          {
            Console.WriteLine($"WARNING reading package[{i}]: {ex.ToString()}");
          }
        }
      }
      return result.ToArray();
    }

    /// <summary>
    /// Reads a single package from a Json file. The outer node of the file has to be a package node.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <returns>The package. If the package could not be read, an exception is thrown.</returns>
    public static PackageInfo ReadPackageFromJson(Stream stream)
    {
      var doc = JsonNode.Parse(stream);
      return ReadPackageFromJson(doc);
    }

    /// <summary>
    /// Reads a single package from a Json package node.
    /// </summary>
    /// <param name="doc">The Json package node to read from.</param>
    /// <returns>The package. If the package could not be read, an exception is thrown.</returns>
    public static PackageInfo ReadPackageFromJson(JsonNode doc)
    {
      var result = new PackageInfo();

      var serializationVersion = doc["SerializationVersion"].GetValue<Int32>();

      if (serializationVersion == 1)
      {
        result.Version = Version.Parse(doc["PackageVersion"].GetValue<string>());

        result.IsUnstableVersion = doc["IsUnstable"].GetValue<bool>();

        var fileNameOfPackageZipFileOverride = doc["FileName"].GetValue<string>();

        result.FileLength = doc["FileLength"].GetValue<long>();

        var hashObj = doc["FileHash"];

        result.HashName = new HashAlgorithmName(hashObj["Name"].GetValue<string>());
        result.Hash = hashObj["Value"].GetValue<string>();

        if (doc[PropertyKeyNetFrameworkVersion] is { } requiredNetFrameworkVersionObj)
        {
          result.RequiredNetFrameworkVersion = Version.Parse(requiredNetFrameworkVersionObj.GetValue<string>());
        }

        if (doc[PropertyKeyDotNetVersion] is { } requiredDotNetVersionObj)
        {
          result.RequiredDotNetVersion = Version.Parse(requiredDotNetVersionObj.GetValue<string>());
        }

        if (doc[PropertyKeyArchitecture] is JsonArray requiredArchitecture)
        {
          result.RequiredArchitectures = new System.Runtime.InteropServices.Architecture[requiredArchitecture.Count];
          for (int i = 0; i < requiredArchitecture.Count; i++)
          {
            var architectureName = requiredArchitecture[i].GetValue<string>();
            if (Enum.TryParse<System.Runtime.InteropServices.Architecture>(architectureName, out var architecture_i))
              result.RequiredArchitectures[i] = architecture_i;
            else
              throw new System.IO.InvalidDataException($"The architecture name #{i}: '{architectureName}') is not recognized.");
          }
        }

        if (doc[PropertyKeyOperatingSystem] is JsonArray requiredOperatingSystem)
        {
          result.RequiredOperatingSystems = new (System.Runtime.InteropServices.OSPlatform Platform, Version Version)[requiredOperatingSystem.Count];
          for (int i = 0; i < requiredOperatingSystem.Count; i++)
          {
            var node = requiredOperatingSystem[i];
            var osPlatform = System.Runtime.InteropServices.OSPlatform.Create(node["OSPlatform"].GetValue<string>());
            var osVersion = Version.Parse(node["OSVersion"].GetValue<string>());
            result.RequiredOperatingSystems[i] = (osPlatform, osVersion);
          }
        }

        // if everything is read-in, we determine the fate of fileNameOfPackageZipFileOverride
        {
          result.IsOldStyleFile = true;
          var oldStylePackageName = result.FileNameOfPackageZipFile;
          result.IsOldStyleFile = false;
          var newStylePackageName = result.FileNameOfPackageZipFile;

          if (fileNameOfPackageZipFileOverride == oldStylePackageName)
          {
            result.IsOldStyleFile = true;
          }
          else if (fileNameOfPackageZipFileOverride == newStylePackageName)
          {
            result.IsOldStyleFile = false;
          }
          else
          {
            result.IsOldStyleFile = false;
            result.FileNameOfPackageZipFileOverride = fileNameOfPackageZipFileOverride;
          }
        }
      }
      else
      {
        throw new InvalidDataException($"Unknown serialization version: {serializationVersion}");
      }

      return result;
    }

    /// <summary>
    /// Saves the package to a stream. Intended for MakePackage, in which every package info is saved into a single file.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void WritePackageToJson(Stream stream)
    {
      using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
      {
        WritePackageToJson(writer);
        writer.Flush();
      }
      stream.Flush();
    }

    /// <summary>
    /// Saves the package using a Json writer. This call can be used to save more than one package into a file.
    /// </summary>
    /// <param name="writer">The Json writer.</param>
    public void WritePackageToJson(Utf8JsonWriter writer)
    {
      writer.WriteStartObject();
      {
        writer.WriteNumber("SerializationVersion", 1);
        writer.WriteString("PackageVersion", Version.ToString(4));
        writer.WriteBoolean("IsUnstable", IsUnstableVersion);
        writer.WriteString("FileName", FileNameOfPackageZipFile);
        writer.WriteNumber("FileLength", FileLength);
        writer.WriteStartObject("FileHash");
        {
          writer.WriteString("Name", HashName.ToString());
          writer.WriteString("Value", Hash);
        }
        writer.WriteEndObject(); // Hash

        if (RequiredNetFrameworkVersion is not null)
        {
          writer.WriteString(PropertyKeyNetFrameworkVersion, RequiredNetFrameworkVersion.ToString());
        }

        if (RequiredDotNetVersion is not null)
        {
          writer.WriteString(PropertyKeyDotNetVersion, RequiredDotNetVersion.ToString());
        }

        if (RequiredArchitectures is not null && RequiredArchitectures.Length > 0)
        {
          writer.WriteStartArray(PropertyKeyArchitecture);
          {
            foreach (var arch in RequiredArchitectures)
            {
              writer.WriteStringValue(arch.ToString());
            }
          }
          writer.WriteEndArray();
        }

        if (RequiredOperatingSystems is not null && RequiredOperatingSystems.Length > 0)
        {
          writer.WriteStartArray(PropertyKeyOperatingSystem);

          foreach (var item in RequiredOperatingSystems)
          {
            writer.WriteStartObject();
            {
              writer.WriteString("OSPlatform", item.Platform.ToString());
              writer.WriteString("OSVersion", item.Version.ToString());
            }
            writer.WriteEndObject();
          }
          writer.WriteEndArray();
        }
      }
      writer.WriteEndObject();
    }

    #endregion

    #region Input from command line arguments (for MakePackage executable, new style, after 2024-11)

    /// <summary>
    /// Reads a package from command line arguments.
    /// </summary>
    /// <param name="isUnstableVersion">If set to <c>true</c>, this indicates the unstable package version.</param>
    /// <param name="packageVersion">The package version.</param>
    /// <param name="args">The arguments. This are properties in the form PropertyKey=PropertyValue.</param>
    /// <returns>The package. If the package could not be read, an exception is thrown.</returns>
    public static PackageInfo ReadPackageFromCommandLine(bool isUnstableVersion, Version packageVersion, string[] args)
    {
      var result = new PackageInfo();
      result.IsUnstableVersion = isUnstableVersion;
      result.Version = packageVersion;

      var properties = new Dictionary<string, string>();
      foreach (var arg in args)
      {
        var kv = arg.Split('=');
        var key = kv[0].Trim();
        var val = kv[1].Trim();

        if (key.ToLowerInvariant() == PropertyKeyNetFrameworkVersion.ToLowerInvariant())
        {
          result.RequiredNetFrameworkVersion = Version.Parse(val);
        }
        else if (key.ToLowerInvariant() == PropertyKeyDotNetVersion.ToLowerInvariant())
        {
          result.RequiredDotNetVersion = Version.Parse(val);
        }
        else if (key.ToLowerInvariant() == PropertyKeyArchitecture.ToLowerInvariant())
        {
          var varr = val.Split([';'], StringSplitOptions.RemoveEmptyEntries);
          result.RequiredArchitectures = varr.Select(name => (Architecture)Enum.Parse(typeof(Architecture), name)).ToArray();
        }
        else if (key.ToLowerInvariant() == PropertyKeyOperatingSystem.ToLowerInvariant())
        {
          var varr = val.Split([';'], StringSplitOptions.RemoveEmptyEntries);
          result.RequiredOperatingSystems = new (OSPlatform Platform, Version Version)[varr.Length];

          for (int i = 0; i < varr.Length; i++)
          {
            var warr = varr[i].Split(['_'], 2);
            var osPlatform = warr[0].ToLowerInvariant() switch
            {
              "freebsd" => OSPlatform.FreeBSD,
              "linux" => OSPlatform.Linux,
              "osx" => OSPlatform.OSX,
              "windows" => OSPlatform.Windows,
              _ => throw new NotImplementedException($"OS platform {warr[0]} is not implemented"),
            };
            var osVersion = Version.Parse(warr[1]);

            result.RequiredOperatingSystems[i] = (osPlatform, osVersion);
          }
        }
        else if (key.ToLowerInvariant() == PropertyKeyIsOldStyleFile.ToLowerInvariant())
        {
          result.IsOldStyleFile = val.ToLowerInvariant() == "true";
        }
      }

      return result;
    }

    /// <summary>
    /// Sets the file length and the file hash of the package file, after it was created.
    /// </summary>
    /// <param name="fileLength">Length of the package file.</param>
    /// <param name="hashName">Name of the hash algorithm, e.g. SHA1, SHA256, etc.</param>
    /// <param name="hash">The hash value.</param>
    internal void SetLengthAndHash(long fileLength, HashAlgorithmName hashName, byte[] hash)
    {
      FileLength = fileLength;
      HashName = hashName;
      Hash = GetHashAsString(hash);
    }

    #endregion

    #region Helper functions

    /// <summary>Determines whether the provided string designates either the stable or the unstable build.</summary>
    /// <param name="s">The string..</param>
    /// <param name="isUnstable">On output, this returns true if the string designates the unstable build.</param>
    /// <returns><c>true</c> if the provided string designates either the stable or the unstable build; otherwise, <c>false</c>.</returns>
    public static bool IsValidStableIdentifier(string s, out bool isUnstable)
    {
      if (0 == string.Compare(s, UnstableIdentifier, true))
      {
        isUnstable = true;
        return true;
      }
      else if (0 == string.Compare(s, StableIdentifier, true))
      {
        isUnstable = false;
        return true;
      }
      else
      {
        isUnstable = false;
        return false;
      }
    }

    /// <summary>Gets the hash as hexadecimal string.</summary>
    /// <param name="hash">The hash.</param>
    /// <returns>Hexadecimal string representing the hash.</returns>
    public static string GetHashAsString(byte[] hash)
    {
      var stb = new StringBuilder(hash.Length * 2);

      foreach (var b in hash)
        stb.Append(b.ToString("X2"));
      return stb.ToString();
    }



    /// <summary>Gets the stable identifier (either 'Unstable' or 'Stable').</summary>
    /// <param name="loadUnstable">Determines either to return 'Unstable' or 'Stable'</param>
    /// <returns>'Unstable' if <paramref name="loadUnstable"/> is <c>true</c>, otherwise 'Stable'.</returns>
    public static string GetStableIdentifier(bool loadUnstable)
    {
      return loadUnstable ? UnstableIdentifier : StableIdentifier;
    }



    /// <summary>Gets the last time a check for new updates was made</summary>
    /// <param name="loadUnstable">If set to <c>true</c>, the time of the last check time for an unstable version is returned, otherwise the last check time for a stable version is returned.</param>
    /// <returns>The last check time (as Utc). If no check was made before, <see cref="DateTime.MinValue"/> is returned.</returns>
    public static DateTime GetLastUpdateCheckTimeUtc(bool loadUnstable)
    {
      try
      {
        var fileName = Path.Combine(GetDownloadDirectory(loadUnstable), VersionFileName);
        var info = new FileInfo(fileName);
        if (info.Exists)
          return info.LastWriteTimeUtc;
      }
      catch (Exception)
      {
      }

      return DateTime.MinValue;
    }

    /// <summary>Gets the directory where to store the downloaded update package.</summary>
    /// <param name="loadUnstableVersion"><c>true</c> when to get the folder for the unstable version; false otherwise.</param>
    /// <returns>The directory where to store the downloaded package.</returns>
    public static string GetDownloadDirectory(bool loadUnstableVersion)
    {
      return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), $"Altaxo\\AutoUpdates\\{GetStableIdentifier(loadUnstableVersion)}");
    }

    /// <summary>
    /// Verifies the length and hash of package zip file. If length or hash differ, an exception is thrown.
    /// </summary>
    /// <param name="storagePath">The full name of the directory where the package file can be found.</param>
    /// <param name="leavePackageFileStreamOpen">If true, the file stream to the package file is left open.
    /// You are responsible then for disposing the returned file stream.</param>
    /// <returns>If the argument <paramref name="leavePackageFileStreamOpen"/> was true, the opened (read-only) stream of the package file.
    /// You must not forget to dispose that stream. Otherwise, the return value is null.</returns>
    /// <exception cref="System.IO.InvalidDataException">
    /// The file {FileNameOfPackageZipFile} can't be found in directory {storagePath}
    /// or
    /// The downloaded file {fileInfo.FullName} has an unexpected length of {fileInfo.Length}. The expected value is {FileLength}
    /// or
    /// The downloaded file {fileInfo.FullName} has a different hash and is considered corrupted.
    /// </exception>
    /// <exception cref="System.NotImplementedException">The downloaded file {fileInfo.FullName} was hashed with algorithm {HashName}, which is not implemented here.</exception>
    public FileStream? VerifyLengthAndHashOfPackageZipFileInFolder(string storagePath, bool leavePackageFileStreamOpen = false)
    {
      // test, if the file exists and has the right Hash
      var fileInfo = new FileInfo(Path.Combine(storagePath, FileNameOfPackageZipFile));
      if (!fileInfo.Exists)
        throw new InvalidDataException($"The file {FileNameOfPackageZipFile} can't be found in directory {storagePath}");

      // test for the appropriate length
      if (fileInfo.Length != FileLength)
        throw new InvalidDataException($"The downloaded file {fileInfo.FullName} has an unexpected length of {fileInfo.Length}. The expected value is {FileLength}");
      HashAlgorithm hashProvider;

      if (HashName == HashAlgorithmName.SHA1 && IsOldStyleFile) // allow SHA1 only for old style files
        hashProvider = SHA1.Create();
      else if (HashName == HashAlgorithmName.SHA256)
        hashProvider = SHA256.Create();
      else if (HashName == HashAlgorithmName.SHA384)
        hashProvider = SHA384.Create();
      else if (HashName == HashAlgorithmName.SHA512)
        hashProvider = SHA512.Create();
      else
        throw new NotImplementedException($"The downloaded file {fileInfo.FullName} was hashed with algorithm {HashName}, which is not implemented here.");

      byte[] hash;
      FileStream? packageFileStream = null;
      if (leavePackageFileStreamOpen)
      {
        packageFileStream = new FileStream(Path.Combine(storagePath, FileNameOfPackageZipFile), FileMode.Open, FileAccess.Read, FileShare.Read);
        hash = hashProvider.ComputeHash(packageFileStream);
        packageFileStream.Seek(0, SeekOrigin.Begin);
      }
      else
      {
        using (var packageFile = new FileStream(Path.Combine(storagePath, FileNameOfPackageZipFile), FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          hash = hashProvider.ComputeHash(packageFile);
        }
      }

      if (GetHashAsString(hash) != Hash)
      {
        packageFileStream?.Dispose();
        packageFileStream = null;
        throw new InvalidDataException($"The downloaded file {fileInfo.FullName} has a different hash and is considered corrupted.");
      }

      return packageFileStream;
    }

    #endregion

  }
}
