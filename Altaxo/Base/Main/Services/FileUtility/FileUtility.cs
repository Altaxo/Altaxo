// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Altaxo.Collections;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Defines how file-operation errors should be reported to the user.
  /// </summary>
  public enum FileErrorPolicy
  {
    /// <summary>
    /// Inform the user about the error without offering an alternative target.
    /// </summary>
    Inform,

    /// <summary>
    /// Allow the user to provide an alternative target when an error occurs.
    /// </summary>
    ProvideAlternative
  }

  /// <summary>
  /// Represents the result of an observed file operation.
  /// </summary>
  public enum FileOperationResult
  {
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    OK,

    /// <summary>
    /// The operation failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The operation succeeded after saving to an alternative location.
    /// </summary>
    SavedAlternatively
  }

  /// <summary>
  /// Represents a file operation without parameters.
  /// </summary>
  public delegate void FileOperationDelegate();

  /// <summary>
  /// Represents a file operation that takes a <see cref="FileName"/>.
  /// </summary>
  /// <param name="fileName">The file name argument.</param>
  public delegate void NamedFileOperationDelegate(FileName fileName);

  /// <summary>
  /// Represents a file or folder operation that takes a <see cref="PathName"/>.
  /// </summary>
  /// <param name="fileName">The file or folder name argument.</param>
  public delegate void NamedFileOrFolderOperationDelegate(PathName fileName);


  /// <summary>
  /// A utility class related to file utilities.
  /// </summary>
  public static partial class FileUtility
  {
    private static readonly char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
    private static string? applicationRootPath = AppDomain.CurrentDomain.BaseDirectory;
    private const string fileNameRegEx = @"^([a-zA-Z]:)?[^:]+$";

    /// <summary>
    /// Gets or sets the application root path.
    /// </summary>
    public static string ApplicationRootPath
    {
      get
      {
        return applicationRootPath ?? throw new InvalidOperationException("Application root path could not be retrieved.");
      }
      set
      {
        applicationRootPath = value;
      }
    }

    /// <summary>
    /// Determines whether the specified path is a URL.
    /// </summary>
    /// <param name="path">The path to test.</param>
    /// <returns><c>true</c> if the path is a URL; otherwise, <c>false</c>.</returns>
    public static bool IsUrl(string path)
    {
      if (path is null)
        throw new ArgumentNullException("path");
      return path.IndexOf("://", StringComparison.Ordinal) > 0;
    }

    /// <summary>
    /// Determines whether two file names are equal.
    /// </summary>
    /// <param name="fileName1">The first file name.</param>
    /// <param name="fileName2">The second file name.</param>
    /// <returns><c>true</c> if both file names are equal; otherwise, <c>false</c>.</returns>
    public static bool IsEqualFileName(FileName fileName1, FileName fileName2)
    {
      return fileName1 == fileName2;
    }

    /// <summary>
    /// Gets the common base directory shared by two directories.
    /// </summary>
    /// <param name="dir1">The first directory.</param>
    /// <param name="dir2">The second directory.</param>
    /// <returns>The common base directory, or <c>null</c> if none exists.</returns>
    public static string? GetCommonBaseDirectory(string dir1, string dir2)
    {
      if (dir1 is null || dir2 is null)
        return null;
      if (IsUrl(dir1) || IsUrl(dir2))
        return null;

      dir1 = NormalizePath(dir1);
      dir2 = NormalizePath(dir2);

      string[] aPath = dir1.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
      string[] bPath = dir2.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
      var result = new StringBuilder();
      int indx = 0;
      for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
      {
        if (bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
        {
          if (result.Length > 0)
            result.Append(Path.DirectorySeparatorChar);
          result.Append(aPath[indx]);
        }
        else
        {
          break;
        }
      }
      if (indx == 0)
        return null;
      else
        return result.ToString();
    }



    /// <summary>
    /// Converts a given absolute path and a given base path to a path that leads
    /// from the base path to the absolute path as a relative path.
    /// </summary>
    /// <param name="baseDirectoryPath">The base directory path.</param>
    /// <param name="absPath">The absolute path.</param>
    /// <returns>The relative path from <paramref name="baseDirectoryPath"/> to <paramref name="absPath"/>.</returns>
    public static string GetRelativePath(string baseDirectoryPath, string absPath)
    {
      if (string.IsNullOrEmpty(baseDirectoryPath))
      {
        return absPath;
      }
      if (IsUrl(absPath) || IsUrl(baseDirectoryPath))
      {
        return absPath;
      }

      baseDirectoryPath = NormalizePath(baseDirectoryPath);
      absPath = NormalizePath(absPath);

      string[] bPath = baseDirectoryPath != "." ? baseDirectoryPath.Split(separators) : new string[0];
      string[] aPath = absPath != "." ? absPath.Split(separators) : new string[0];
      int indx = 0;
      for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
      {
        if (!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
          break;
      }

      if (indx == 0 && (Path.IsPathRooted(baseDirectoryPath) || Path.IsPathRooted(absPath)))
      {
        return absPath;
      }

      if (indx == bPath.Length && indx == aPath.Length)
      {
        return ".";
      }
      var erg = new StringBuilder();
      for (int i = indx; i < bPath.Length; ++i)
      {
        erg.Append("..");
        erg.Append(Path.DirectorySeparatorChar);
      }
      erg.Append(string.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx));
      if (erg[erg.Length - 1] == Path.DirectorySeparatorChar)
        erg.Length -= 1;
      return erg.ToString();
    }

    /// <summary>
    /// Combines baseDirectoryPath with relPath and normalizes the resulting path.
    /// </summary>
    /// <param name="baseDirectoryPath">The base directory path.</param>
    /// <param name="relPath">The relative path.</param>
    /// <returns>The normalized absolute path.</returns>
    public static string GetAbsolutePath(string baseDirectoryPath, string relPath)
    {
      return NormalizePath(Path.Combine(baseDirectoryPath, relPath));
    }

    /// <summary>
    /// Replaces the base directory of a path if it starts with a specified source directory.
    /// </summary>
    /// <param name="fileName">The path to update.</param>
    /// <param name="oldDirectory">The original base directory.</param>
    /// <param name="newDirectory">The new base directory.</param>
    /// <returns>The updated path, or the original path if the base directory does not match.</returns>
    public static string RenameBaseDirectory(string fileName, string oldDirectory, string newDirectory)
    {
      fileName = NormalizePath(fileName);
      oldDirectory = NormalizePath(oldDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
      newDirectory = NormalizePath(newDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
      if (IsBaseDirectory(oldDirectory, fileName))
      {
        if (fileName.Length == oldDirectory.Length)
        {
          return newDirectory;
        }
        return Path.Combine(newDirectory, fileName.Substring(oldDirectory.Length + 1));
      }
      return fileName;
    }

    /// <summary>
    /// Copies a directory and all its contents recursively.
    /// </summary>
    /// <param name="sourceDirectory">The source directory.</param>
    /// <param name="destinationDirectory">The destination directory.</param>
    /// <param name="overwrite">If set to <c>true</c>, existing files are overwritten.</param>
    public static void DeepCopy(string sourceDirectory, string destinationDirectory, bool overwrite)
    {
      if (!Directory.Exists(destinationDirectory))
      {
        Directory.CreateDirectory(destinationDirectory);
      }
      foreach (string fileName in Directory.GetFiles(sourceDirectory))
      {
        File.Copy(fileName, Path.Combine(destinationDirectory, Path.GetFileName(fileName)), overwrite);
      }
      foreach (string directoryName in Directory.GetDirectories(sourceDirectory))
      {
        DeepCopy(directoryName, Path.Combine(destinationDirectory, Path.GetFileName(directoryName)), overwrite);
      }
    }


    /// <summary>
    /// Lazily enumerates files in a directory that match the specified mask.
    /// </summary>
    /// <param name="directory">The directory to search.</param>
    /// <param name="filemask">The search pattern.</param>
    /// <param name="searchSubdirectories">If set to <c>true</c>, subdirectories are searched as well.</param>
    /// <param name="ignoreHidden">If set to <c>true</c>, hidden files and folders are ignored.</param>
    /// <returns>A lazy sequence of matching file names.</returns>
    public static IEnumerable<FileName> LazySearchDirectory(string directory, string filemask, bool searchSubdirectories = true, bool ignoreHidden = true)
    {
      return SearchDirectoryInternal(directory, filemask, searchSubdirectories, ignoreHidden);
    }

    /// <summary>
    /// Finds all files which are valid to the mask <paramref name="filemask"/> in the path
    /// <paramref name="directory"/> and all subdirectories
    /// (if <paramref name="searchSubdirectories"/> is true).
    /// If <paramref name="ignoreHidden"/> is true, hidden files and folders are ignored.
    /// </summary>
    private static IEnumerable<FileName> SearchDirectoryInternal(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
    {
      // If Directory.GetFiles() searches the 8.3 name as well as the full name so if the filemask is
      // "*.xpt" it will return "Template.xpt~"
      bool isExtMatch = filemask is not null && Regex.IsMatch(filemask, @"^\*\.[\w\d_]{3}$");
      string? ext = null;
      if (isExtMatch)
        ext = filemask!.Substring(1);


      IEnumerable<string> dir = new[] { directory };

      if (searchSubdirectories)
        dir = dir.FlattenFromRootToLeaves(
            d =>
            {
              try
              {
                if (ignoreHidden)
                  return Directory.EnumerateDirectories(d).Where(IsNotHidden);
                else
                  return Directory.EnumerateDirectories(d);
              }
              catch (UnauthorizedAccessException)
              {
                return new string[0];
              }
            });
      foreach (string d in dir)
      {
        IEnumerable<string> files;
        try
        {
          if (string.IsNullOrEmpty(filemask))
            files = Directory.EnumerateFiles(d);
          else
            files = Directory.EnumerateFiles(d, filemask);
        }
        catch (UnauthorizedAccessException)
        {
          continue;
        }
        foreach (string f in files)
        {
          if (ext is not null && !f.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
            continue; // file extension didn't match
          if (!ignoreHidden || IsNotHidden(f))
            yield return new FileName(f);
        }
      }
    }

    private static bool IsNotHidden(string dir)
    {
      try
      {
        return (File.GetAttributes(dir) & FileAttributes.Hidden) != FileAttributes.Hidden;
      }
      catch (UnauthorizedAccessException)
      {
        return false;
      }
    }

    // This is an arbitrary limitation built into the .NET Framework.
    // Windows supports paths up to 32k length.
    /// <summary>
    /// Gets the maximum path length supported by this utility.
    /// </summary>
    public static readonly int MaxPathLength = 260;

    /// <summary>
    /// This method checks if a path (full or relative) is valid.
    /// </summary>
    /// <param name="fileName">The path to validate.</param>
    /// <returns><c>true</c> if the path is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidPath(string? fileName)
    {
      // Fixme: 260 is the hardcoded maximal length for a path on my Windows XP system
      //        I can't find a .NET property or method for determining this variable.

      if (fileName is null || fileName.Length == 0 || fileName.Length >= MaxPathLength)
      {
        return false;
      }

      // platform independend : check for invalid path chars

      if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
      {
        return false;
      }
      if (fileName.IndexOf('?') >= 0 || fileName.IndexOf('*') >= 0)
      {
        return false;
      }

      if (!Regex.IsMatch(fileName, fileNameRegEx))
      {
        return false;
      }

      if (fileName[fileName.Length - 1] == ' ')
      {
        return false;
      }

      if (fileName[fileName.Length - 1] == '.')
      {
        return false;
      }

      // platform dependend : Check for invalid file names (DOS)
      // this routine checks for follwing bad file names :
      // CON, PRN, AUX, NUL, COM1-9 and LPT1-9

      string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
      if (nameWithoutExtension is not null)
      {
        nameWithoutExtension = nameWithoutExtension.ToUpperInvariant();
      }
      else
      {
        nameWithoutExtension = string.Empty;
      }

      if (nameWithoutExtension == "CON" ||
              nameWithoutExtension == "PRN" ||
              nameWithoutExtension == "AUX" ||
              nameWithoutExtension == "NUL")
      {
        return false;
      }

      char ch = nameWithoutExtension.Length == 4 ? nameWithoutExtension[3] : '\0';

      return !((nameWithoutExtension.StartsWith("COM", StringComparison.Ordinal) ||
                          nameWithoutExtension.StartsWith("LPT", StringComparison.Ordinal)) &&
                       char.IsDigit(ch));
    }

    /// <summary>
    /// Checks that a single directory name (not the full path) is valid.
    /// </summary>
    /// <param name="name">The directory entry name to validate.</param>
    /// <returns><c>true</c> if the directory entry name is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValidDirectoryEntryName(string name)
    {
      if (!IsValidPath(name))
      {
        return false;
      }
      if (name.IndexOfAny(new char[] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar, Path.VolumeSeparatorChar }) >= 0)
      {
        return false;
      }
      if (name.Trim(' ').Length == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Tests whether the specified file exists and shows a warning if it does not.
    /// </summary>
    /// <param name="filename">The file name to test.</param>
    /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
    public static bool TestFileExists(string filename)
    {
      if (!File.Exists(filename))
      {
        var messageService = Altaxo.Current.GetRequiredService<IMessageService>();
        messageService.ShowWarning(StringParser.Parse("${res:Fileutility.CantFindFileError}", new StringTagPair("FILE", filename)) ?? string.Empty);
        return false;
      }
      return true;
    }

    /// <summary>
    /// Determines whether the specified path refers to an existing directory.
    /// </summary>
    /// <param name="filename">The path to test.</param>
    /// <returns><c>true</c> if the path refers to an existing directory; otherwise, <c>false</c>.</returns>
    public static bool IsDirectory(string filename)
    {
      if (!Directory.Exists(filename))
      {
        return false;
      }
      FileAttributes attr = File.GetAttributes(filename);
      return (attr & FileAttributes.Directory) != 0;
    }

    //TODO This code is Windows specific
    private static bool MatchN(string src, int srcidx, string pattern, int patidx)
    {
      int patlen = pattern.Length;
      int srclen = src.Length;
      char next_char;

      for (; ; )
      {
        if (patidx == patlen)
          return (srcidx == srclen);
        next_char = pattern[patidx++];
        if (next_char == '?')
        {
          if (srcidx == src.Length)
            return false;
          srcidx++;
        }
        else if (next_char != '*')
        {
          if ((srcidx == src.Length) || (src[srcidx] != next_char))
            return false;
          srcidx++;
        }
        else
        {
          if (patidx == pattern.Length)
            return true;
          while (srcidx < srclen)
          {
            if (MatchN(src, srcidx, pattern, patidx))
              return true;
            srcidx++;
          }
          return false;
        }
      }
    }

    private static bool Match(string src, string pattern)
    {
      if (pattern[0] == '*')
      {
        // common case optimization
        int i = pattern.Length;
        int j = src.Length;
        while (--i > 0)
        {
          if (pattern[i] == '*')
            return MatchN(src, 0, pattern, 0);
          if (j-- == 0)
            return false;
          if ((pattern[i] != src[j]) && (pattern[i] != '?'))
            return false;
        }
        return true;
      }
      return MatchN(src, 0, pattern, 0);
    }

    /// <summary>
    /// Determines whether a file name matches one or more wildcard patterns.
    /// </summary>
    /// <param name="filename">The file name to test.</param>
    /// <param name="pattern">The semicolon-separated wildcard pattern list.</param>
    /// <returns><c>true</c> if the file name matches at least one pattern; otherwise, <c>false</c>.</returns>
    public static bool MatchesPattern(string filename, string pattern)
    {
      filename = filename.ToUpperInvariant();
      pattern = pattern.ToUpperInvariant();
      string[] patterns = pattern.Split(';');
      foreach (string p in patterns)
      {
        if (Match(filename, p))
        {
          return true;
        }
      }
      return false;
    }

    // Observe SAVE functions
    /// <summary>
    /// Executes a save operation and reports failures according to the specified policy.
    /// </summary>
    /// <param name="saveFile">The save operation.</param>
    /// <param name="fileName">The target file or folder name.</param>
    /// <param name="message">The message shown when an error occurs.</param>
    /// <param name="policy">The error-handling policy.</param>
    /// <returns>The result of the save operation.</returns>
    public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, PathName fileName, string message, FileErrorPolicy policy = FileErrorPolicy.Inform)
    {
      System.Diagnostics.Debug.Assert(IsValidPath(fileName));
      try
      {
        saveFile();
        RaiseFileSaved(new PathNameEventArgs(fileName));
        return FileOperationResult.OK;
      }
      catch (IOException e)
      {
        return ObservedSaveHandleException(e, saveFile, fileName, message, policy);
      }
      catch (UnauthorizedAccessException e)
      {
        return ObservedSaveHandleException(e, saveFile, fileName, message, policy);
      }
    }

    private static FileOperationResult ObservedSaveHandleException(Exception e, FileOperationDelegate saveFile, PathName fileName, string message, FileErrorPolicy policy)
    {
      var messageService = Altaxo.Current.GetRequiredService<IMessageService>();
      switch (policy)
      {
        case FileErrorPolicy.Inform:
          messageService.InformSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e);
          break;

        case FileErrorPolicy.ProvideAlternative:
          ChooseSaveErrorResult r = messageService.ChooseSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e, false);
          if (r.IsRetry)
          {
            return ObservedSave(saveFile, fileName, message, policy);
          }
          else if (r.IsIgnore)
          {
            return FileOperationResult.Failed;
          }
          break;
      }
      return FileOperationResult.Failed;
    }

    /// <summary>
    /// Executes a save operation for a file and reports failures according to the specified policy.
    /// </summary>
    /// <param name="saveFile">The save operation.</param>
    /// <param name="fileName">The target file name.</param>
    /// <param name="policy">The error-handling policy.</param>
    /// <returns>The result of the save operation.</returns>
    public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, FileName fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
    {
      return ObservedSave(
        saveFile,
        fileName,
        Current.ResourceService.GetString("Services.FileUtilityService.CantSaveFileStandardText") ?? string.Empty,
        policy);
    }

    /// <summary>
    /// Executes a save-as operation and reports failures according to the specified policy.
    /// </summary>
    /// <param name="saveFileAs">The save-as operation.</param>
    /// <param name="fileName">The target file or folder name.</param>
    /// <param name="message">The message shown when an error occurs.</param>
    /// <param name="policy">The error-handling policy.</param>
    /// <returns>The result of the save-as operation.</returns>
    public static FileOperationResult ObservedSave(NamedFileOrFolderOperationDelegate saveFileAs, PathName fileName, string message, FileErrorPolicy policy = FileErrorPolicy.Inform)
    {
      System.Diagnostics.Debug.Assert(IsValidPath(fileName));
      try
      {
        if (fileName.GetParentDirectory() is { } parentDirectory)
          Directory.CreateDirectory(parentDirectory.ToString());

        saveFileAs(fileName);
        RaiseFileSaved(new PathNameEventArgs(fileName));
        return FileOperationResult.OK;
      }
      catch (IOException e)
      {
        return ObservedSaveHandleError(e, saveFileAs, fileName, message, policy);
      }
      catch (UnauthorizedAccessException e)
      {
        return ObservedSaveHandleError(e, saveFileAs, fileName, message, policy);
      }
    }


    private static FileOperationResult ObservedSaveHandleError(Exception e, NamedFileOrFolderOperationDelegate saveFileAs, PathName fileName, string message, FileErrorPolicy policy)
    {
      message = message + Environment.NewLine + Environment.NewLine + e.Message;
      var messageService = Altaxo.Current.GetRequiredService<IMessageService>();
      switch (policy)
      {
        case FileErrorPolicy.Inform:
          messageService.InformSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e);
          break;

        case FileErrorPolicy.ProvideAlternative:
          ChooseSaveErrorResult r = messageService.ChooseSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e, true);
          if (r.IsRetry)
          {
            return ObservedSave(saveFileAs, fileName, message, policy);
          }
          else if (r.IsIgnore)
          {
            return FileOperationResult.Failed;
          }
          else if (r.AlternativeFileName is not null)
          {
            return ObservedSave(saveFileAs, r.AlternativeFileName, message, policy);
          }
          break;
      }
      return FileOperationResult.Failed;
    }

    /// <summary>
    /// Executes a save-as operation and reports failures using a standard save error message.
    /// </summary>
    /// <param name="saveFileAs">The save-as operation.</param>
    /// <param name="fileName">The target file or folder name.</param>
    /// <param name="policy">The error-handling policy.</param>
    /// <returns>The result of the save-as operation.</returns>
    public static FileOperationResult ObservedSave(NamedFileOrFolderOperationDelegate saveFileAs, PathName fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
    {
      return ObservedSave(
        saveFileAs,
        fileName,
        Current.ResourceService.GetString("Services.FileUtilityService.CantSaveFileStandardText") ?? string.Empty,
        policy);
    }

    // Observe LOAD functions
    /// <summary>
    /// Executes a load operation and reports failures according to the specified policy.
    /// </summary>
    /// <param name="loadFile">The load operation.</param>
    /// <param name="fileName">The source file name.</param>
    /// <param name="message">The message shown when an error occurs.</param>
    /// <param name="policy">The error-handling policy.</param>
    /// <returns>The result of the load operation.</returns>
    public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, FileName fileName, string message, FileErrorPolicy policy)
    {
      try
      {
        loadFile();
        OnFileLoaded(new PathNameEventArgs(fileName));
        return FileOperationResult.OK;
      }
      catch (IOException e)
      {
        return ObservedLoadHandleException(e, loadFile, fileName, message, policy);
      }
      catch (UnauthorizedAccessException e)
      {
        return ObservedLoadHandleException(e, loadFile, fileName, message, policy);
      }
      catch (FormatException e)
      {
        return ObservedLoadHandleException(e, loadFile, fileName, message, policy);
      }
    }

    private static FileOperationResult ObservedLoadHandleException(Exception e, FileOperationDelegate loadFile, FileName fileName, string message, FileErrorPolicy policy)
    {
      message = message + Environment.NewLine + Environment.NewLine + e.Message;
      var messageService = Altaxo.Current.GetRequiredService<IMessageService>();
      switch (policy)
      {
        case FileErrorPolicy.Inform:
          messageService.InformSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileLoading}", e);
          break;

        case FileErrorPolicy.ProvideAlternative:
          ChooseSaveErrorResult r = messageService.ChooseSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileLoading}", e, false);
          if (r.IsRetry)
            return ObservedLoad(loadFile, fileName, message, policy);
          else if (r.IsIgnore)
            return FileOperationResult.Failed;
          break;
      }
      return FileOperationResult.Failed;
    }

    /// <summary>
    /// Executes a load operation and reports failures using a standard load error message.
    /// </summary>
    /// <param name="loadFile">The load operation.</param>
    /// <param name="fileName">The source file name.</param>
    /// <param name="policy">The error-handling policy.</param>
    /// <returns>The result of the load operation.</returns>
    public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, FileName fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
    {
      return ObservedLoad(
        loadFile,
        fileName,
        Current.ResourceService.GetString("Services.FileUtilityService.CantLoadFileStandardText") ?? string.Empty,
        policy);
    }

    /// <summary>
    /// Executes a load operation that accepts a file name and reports failures according to the specified policy.
    /// </summary>
    /// <param name="saveFileAs">The load operation.</param>
    /// <param name="fileName">The source file name.</param>
    /// <param name="message">The message shown when an error occurs.</param>
    /// <param name="policy">The error-handling policy.</param>
    /// <returns>The result of the load operation.</returns>
    public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, FileName fileName, string message, FileErrorPolicy policy = FileErrorPolicy.Inform)
    {
      return ObservedLoad(new FileOperationDelegate(delegate
      { saveFileAs(fileName); }), fileName, message, policy);
    }

    /// <summary>
    /// Executes a load operation that accepts a file name and reports failures using a standard load error message.
    /// </summary>
    /// <param name="saveFileAs">The load operation.</param>
    /// <param name="fileName">The source file name.</param>
    /// <param name="policy">The error-handling policy.</param>
    /// <returns>The result of the load operation.</returns>
    public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, FileName fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
    {
      return ObservedLoad(
        saveFileAs,
        fileName,
        Current.ResourceService.GetString("Services.FileUtilityService.CantLoadFileStandardText") ?? string.Empty,
        policy);
    }

    private static void OnFileLoaded(PathNameEventArgs e)
    {
      FileLoaded?.Invoke(null, e);
    }

    /// <summary>
    /// Raises the <see cref="FileSaved"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    public static void RaiseFileSaved(PathNameEventArgs e)
    {
      FileSaved?.Invoke(null, e);
    }

    /// <summary>
    /// Occurs after a file has been loaded successfully.
    /// </summary>
    public static event EventHandler<PathNameEventArgs>? FileLoaded;

    /// <summary>
    /// Occurs after a file has been saved successfully.
    /// </summary>
    public static event EventHandler<PathNameEventArgs>? FileSaved;
  }
}
