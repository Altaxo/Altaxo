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

using System;
using System.Globalization;
using Altaxo.Gui.Workbench;

namespace Altaxo.Workbench
{
  /// <summary>
  /// Default implementation for classes that wrap navigational
  /// information for the navigational services.
  /// </summary>
  public class DefaultNavigationPoint : INavigationPoint
  {
    private string _fileName;
    private object? _data;

    #region constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultNavigationPoint"/> class.
    /// </summary>
    public DefaultNavigationPoint() : this(string.Empty, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultNavigationPoint"/> class for the specified file.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    public DefaultNavigationPoint(string fileName) : this(fileName, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultNavigationPoint"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="data">The associated navigation data.</param>
    public DefaultNavigationPoint(string fileName, object? data)
    {
      this._fileName = fileName is null ? string.Empty : fileName;
      this._data = data;
    }

    #endregion constructor

    #region overrides

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format(CultureInfo.CurrentCulture,
                           "[{0}: {1}]",
                           GetType().Name,
                           Description);
    }

    #endregion overrides

    #region INavigationPoint implementation

    /// <summary>
    /// Gets the file name represented by this navigation point.
    /// </summary>
    public virtual string FileName
    {
      get
      {
        return _fileName;
      }
    }

    /// <summary>
    /// Gets a short description of the navigation point.
    /// </summary>
    public virtual string Description
    {
      get
      {
        return string.Format(CultureInfo.CurrentCulture,
                             "{0}: {1}", _fileName, _data);
      }
    }

    /// <summary>
    /// Gets the full description of the navigation point.
    /// </summary>
    public virtual string FullDescription
    {
      get
      {
        return Description;
      }
    }

    /// <summary>
    /// Gets the tool tip text for the navigation point.
    /// </summary>
    public virtual string ToolTip
    {
      get
      {
        return Description;
      }
    }

    //		public string TabName {
    //			get {
    //				return tabName;
    //			}
    //		}

    /// <summary>
    /// Gets the sort index of the navigation point.
    /// </summary>
    public virtual int Index
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets or sets the associated navigation data.
    /// </summary>
    public object? NavigationData
    {
      get
      {
        return _data;
      }
      set
      {
        _data = value;
      }
    }

    /// <summary>
    /// Jumps to the navigation point.
    /// </summary>
    public virtual void JumpTo()
    {
      Altaxo.Current.GetRequiredService<IFileService>().JumpToFilePosition(new Altaxo.Main.Services.FileName(FileName), 0, 0);
    }

    /// <summary>
    /// Updates the file name stored by this navigation point.
    /// </summary>
    /// <param name="newName">The new file name.</param>
    public void FileNameChanged(string newName)
    {
      _fileName = newName is null ? string.Empty : newName;
    }

    /// <summary>
    /// Called when the related content is changing.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    public virtual void ContentChanging(object sender, EventArgs e)
    {
      //throw new NotImplementedException();
    }

    #endregion INavigationPoint implementation

    #region Equality

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      var b = obj as DefaultNavigationPoint;
      if (object.ReferenceEquals(b, null))
        return false;
      return FileName == b.FileName;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return FileName.GetHashCode();
    }

    #endregion Equality

    #region IComparable

    /// <summary>
    /// Compares this navigation point to another object.
    /// </summary>
    /// <param name="obj">The object to compare to.</param>
    /// <returns>A signed integer that indicates the relative order.</returns>
    public virtual int CompareTo(object? obj)
    {
      if (obj is null)
        return 1;
      else if (obj is DefaultNavigationPoint defNavPt)
        return FileName.CompareTo(defNavPt.FileName);
      else   // if of different types, sort the types by name
        return GetType().Name.CompareTo(obj.GetType().Name);
    }

    // Omitting any of the following operator overloads
    // violates rule: OverrideMethodsOnComparableTypes.
    /// <summary>
    /// Determines whether two navigation points are equal.
    /// </summary>
    public static bool operator ==(DefaultNavigationPoint? p1, DefaultNavigationPoint? p2)
    {
      return object.Equals(p1, p2); // checks for null and calls p1.Equals(p2)
    }

    /// <summary>
    /// Determines whether two navigation points are not equal.
    /// </summary>
    public static bool operator !=(DefaultNavigationPoint? p1, DefaultNavigationPoint? p2)
    {
      return !(p1 == p2);
    }

    /// <summary>
    /// Determines whether the left navigation point sorts before the right one.
    /// </summary>
    public static bool operator <(DefaultNavigationPoint? p1, DefaultNavigationPoint? p2)
    {
      return p1 is null ? !(p2 is null) : (p1.CompareTo(p2) < 0);
    }

    /// <summary>
    /// Determines whether the left navigation point sorts after the right one.
    /// </summary>
    public static bool operator >(DefaultNavigationPoint? p1, DefaultNavigationPoint? p2)
    {
      return p1 is null ? false : (p1.CompareTo(p2) > 0);
    }

    #endregion IComparable
  }
}
