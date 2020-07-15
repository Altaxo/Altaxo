#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Altaxo.Serialization
{
  /// <summary>
  /// Holds an absolute file name, plus the same file name but relative to the project file.
  /// </summary>
  public class AbsoluteAndRelativeFileName : Main.ICopyFrom
  {
    private string _absoluteFileName;
    private string? _relativeFileName;

    #region Serialization

    /// <summary>
    /// 2014-07-28 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AbsoluteAndRelativeFileName), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AbsoluteAndRelativeFileName)obj;

        s.TrySetRelativeFileName(); // set the relative file name now, because project name may have changed in the last seconds

        info.AddValue("AbsoluteName", s._absoluteFileName);
        info.AddValue("RelativeName", s._relativeFileName);
      }

      protected virtual AbsoluteAndRelativeFileName SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var absoluteFilePath = info.GetString("AbsolutePath");

        var s = (AbsoluteAndRelativeFileName?)o ?? new AbsoluteAndRelativeFileName(absoluteFilePath);

        s._absoluteFileName = absoluteFilePath;
        s._relativeFileName = info.GetString("RelativePath");

        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteAndRelativeFileName"/> class.
    /// </summary>
    /// <param name="fileName">The absolute file name.</param>
    public AbsoluteAndRelativeFileName(string fileName)
    {
      AbsoluteFileName = fileName;
    }

    /// <summary>
    /// Try to copy from another object. Should try to copy even if the object to copy from is not of
    /// the same type, but a base type. In this case only the base properties should be copied.
    /// </summary>
    /// <param name="obj">Object to copy from.</param>
    /// <returns>
    /// True if at least parts of the object could be copied, false if the object to copy from is incompatible.
    /// </returns>
    public bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as AbsoluteAndRelativeFileName;
      if (null != from)
      {
        _absoluteFileName = from._absoluteFileName;
        _relativeFileName = from._relativeFileName;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
      return MemberwiseClone();
    }

    /// <summary>
    /// Gets or sets the absolute file name. When getting this property, no attempt is made to resolve the file name. If you intend to resolve the file name, use <see cref="GetResolvedFileNameOrNull"/> instead.
    /// </summary>
    /// <value>
    /// The absolute file name.
    /// </value>
    /// <exception cref="System.ArgumentNullException">Path is null or empty</exception>
    /// <exception cref="System.ArgumentException">Path has to be an absolute path, i.e. it must be rooted!</exception>
    public string AbsoluteFileName
    {
      get
      {
        return _absoluteFileName;
      }
      [MemberNotNull(nameof(_absoluteFileName))]
      set
      {
        if (string.IsNullOrEmpty(value))
          throw new ArgumentNullException("Path is null or empty");
        if (!Path.IsPathRooted(value))
          throw new ArgumentException("Path has to be an absolute path, i.e. it must be rooted!");

        _absoluteFileName = value;
      }
    }

    /// <summary>
    /// Gets the absolute resolved file name or null. In order to resolve the name, at first it is tested whether a file with the stored absolute file name exists. If it exists, the absolute  file name is returned.
    /// If such a file does not exist, the file name is evaluated from the stored relative file name and the name of the project file. Again, if a file under this name exist, the absolute file name of this file is returned.
    /// If neither the absolute nor the relative file name leads to an existing file, <c>null</c> is returned.
    /// </summary>
    /// <returns>The absolute file name of an existing file, or <c>null</c>.</returns>
    public string? GetResolvedFileNameOrNull()
    {
      if (File.Exists(_absoluteFileName))
        return _absoluteFileName;

      var projectFile = Current.IProjectService.CurrentProjectFileName;

      if (string.IsNullOrEmpty(projectFile))
        return null;

      if (string.IsNullOrEmpty(_relativeFileName))
        return null;

      var projectDirectory = Path.GetDirectoryName(projectFile) ?? throw new InvalidOperationException($"Error GetDirectoryName from file name {projectFile}");

      var combinedPath = Path.Combine(projectDirectory, _relativeFileName);

      if (File.Exists(combinedPath))
      {
        _absoluteFileName = ConvertToNormalizedAbsolutePath(combinedPath);
        return _absoluteFileName;
      }

      return null;
    }

    /// <summary>
    ///Gets rid of relative path parts like '.' or '..'.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>A new path without relative path parts like '.' or '..'.</returns>
    private string ConvertToNormalizedAbsolutePath(string path)
    {
      var root = System.IO.Path.GetPathRoot(path) ?? throw new InvalidOperationException($"Error getting path root of path {path}");
      var rest = path.Substring(root.Length);

      var restParted = rest.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
      var stack = new Stack<string>();
      for (int i = 0; i < restParted.Length; ++i)
      {
        if (restParted[i] == "..")
        {
          stack.Pop();
        }
        else if (restParted[i] == ".")
        {
        }
        else
        {
          stack.Push(restParted[i]);
        }
      }

      var result = Path.Combine(root, string.Join(Path.DirectorySeparatorChar.ToString(), stack.Reverse().ToArray()));

      return result;
    }

    /// <summary>
    /// Sets the relative file name by calculating it from the absolute file name and the name of the current project file.
    /// </summary>
    /// <returns><c>True</c> if the relative file name could be calculated (if the project has a name, and the project file is in the same volume as the absolute file name). Otherwise , <c>false</c> is returned.</returns>
    private bool TrySetRelativeFileName()
    {
      string? projectFile = Current.IProjectService.CurrentProjectFileName;

      if (string.IsNullOrEmpty(projectFile))
      {
        _relativeFileName = null;
        return false;
      }

      var partThis = _absoluteFileName.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
      var partProj = projectFile.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

      int firstDeviatingIdx = 0;
      int len = Math.Min(partThis.Length, partProj.Length);

      for (firstDeviatingIdx = 0; firstDeviatingIdx < len; ++firstDeviatingIdx)
      {
        if (partThis[firstDeviatingIdx] != partProj[firstDeviatingIdx])
          break;
      }

      if (firstDeviatingIdx == 0)
      {
        _relativeFileName = null;
        return false;
      }

      // i now designates the point where both path diverge for the first time.

      var stb = new System.Text.StringBuilder();

      int stepsBack = partProj.Length - firstDeviatingIdx - 1;

      if (stepsBack > 0)
      {
        for (int i = 0; i < stepsBack; ++i)
        {
          stb.Append("..");
          stb.Append(Path.DirectorySeparatorChar);
        }
      }
      else
      {
        stb.Append(".");
        stb.Append(Path.DirectorySeparatorChar);
      }

      for (int i = firstDeviatingIdx; i < partThis.Length - 1; ++i)
      {
        stb.Append(partThis[i]);
        stb.Append(Path.DirectorySeparatorChar);
      }

      stb.Append(partThis[partThis.Length - 1]);

      _relativeFileName = stb.ToString();

      return true;
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
      return obj is AbsoluteAndRelativeFileName from &&
             from._absoluteFileName == _absoluteFileName &&
             from._relativeFileName == _relativeFileName;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
      return _absoluteFileName.GetHashCode();
    }
  }
}
