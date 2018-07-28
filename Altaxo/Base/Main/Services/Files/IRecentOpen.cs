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
using System.Collections.Generic;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Interface to a class that stores recently open files and projects.
  /// </summary>
  /// <see cref="IFileService.RecentOpen" />
  public interface IRecentOpen
  {
    /// <summary>
    /// Gets the collection of recently opened files (other than project files).
    /// </summary>
    IReadOnlyList<FileName> RecentFiles { get; }

    /// <summary>
    /// Gets the collection of recently opened project files.
    /// </summary>
    IReadOnlyList<FileName> RecentProjects { get; }

    /// <summary>
    /// Clears the collection of recently opened files (other than project files).
    /// </summary>
    void ClearRecentFiles();

    /// <summary>
    /// Clears the collection of recently opened project files.
    /// </summary>
    void ClearRecentProjects();

    /// <summary>
    /// Removes one project file from the collection of recently opened project files.
    /// </summary>
    void RemoveRecentProject(FileName fileName);

    /// <summary>
    /// Adds a file to the collection of recently opened files (other than project files).
    /// </summary>
    void AddRecentFile(FileName fileName);

    /// <summary>
    /// Adds a project file to the collection of recently opened project files.
    /// </summary>
    void AddRecentProject(FileName fileName);
  }
}
