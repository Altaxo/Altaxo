﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Interface for classes that store Navigational information for
  /// the <see cref="NavigationService"/>.
  /// </summary>
  public interface INavigationPoint : IComparable
  {
    /// <summary>
    /// The path to the file containing the <see cref="INavigationPoint"/>
    /// </summary>
    string FileName
    {
      get;
    }

    /// <summary>
    /// Gets the text that will appear in the drop-down menu to select
    /// this <see cref="INavigationPoint"/>.
    /// </summary>
    string Description
    {
      get;
    }

    /// <summary>
    /// Gets more detailed text that cam be used to describe
    /// this <see cref="INavigationPoint"/>.
    /// </summary>
    string FullDescription
    {
      get;
    }

    /// <summary>
    /// Gets the tool tip for the navigation point.
    /// </summary>
    /// <value>
    /// The tool tip.
    /// </value>
    string ToolTip
    {
      get;
    }

    /// <summary>
    /// Gets the specific data, if any, needed to
    /// navigate to this <see cref="INavigationPoint"/>.
    /// </summary>
    object NavigationData
    {
      get;
    }

    int Index
    {
      get;
    }

    /// <summary>
    /// Navigates to this <see cref="INavigationPoint"/>.
    /// </summary>
    void JumpTo();

    /// <summary>
    /// Updates the <see cref="FileName"/>.
    /// </summary>
    /// <param name="newName"></param>
    void FileNameChanged(string newName);

    /// <summary>
    /// Responsible for updating the internal data of the
    /// <see cref="INavigationPoint"/> to synch it with
    /// changes in the IViewContent containing the point.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ContentChanging(object sender, EventArgs e);
  }
}
