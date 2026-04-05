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

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Event arguments that carry a <see cref="PathName"/> value.
  /// </summary>
  public class PathNameEventArgs : System.EventArgs
  {
    private PathName _pathName;

    /// <summary>
    /// Gets the path name.
    /// </summary>
    public PathName PathName
    {
      get
      {
        return _pathName;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathNameEventArgs"/> class.
    /// </summary>
    /// <param name="pathName">The path name.</param>
    public PathNameEventArgs(PathName pathName)
    {
      this._pathName = pathName;
    }


  }


  /// <summary>
  /// Event arguments that carry a <see cref="FileName"/> value.
  /// </summary>
  public class FileNameEventArgs : System.EventArgs
  {
    private FileName fileName;

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public FileName FileName
    {
      get
      {
        return fileName;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileNameEventArgs"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    public FileNameEventArgs(FileName fileName)
    {
      this.fileName = fileName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileNameEventArgs"/> class.
    /// </summary>
    /// <param name="fileName">The file name as string.</param>
    public FileNameEventArgs(string fileName)
    {
      this.fileName = FileName.Create(fileName);
    }
  }
}
