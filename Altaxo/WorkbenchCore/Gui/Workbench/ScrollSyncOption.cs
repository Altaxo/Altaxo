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

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Specifies how scrolling is synchronized between views.
  /// </summary>
  public enum ScrollSyncOption
  {
    /// <summary>
    /// Synchronize vertical scrolling.
    /// </summary>
    Vertical,

    /// <summary>
    /// Synchronize horizontal scrolling.
    /// </summary>
    Horizontal,

    /// <summary>
    /// Synchronize vertical scrolling to horizontal scrolling.
    /// </summary>
    VerticalToHorizontal,

    /// <summary>
    /// Synchronize horizontal scrolling to vertical scrolling.
    /// </summary>
    HorizontalToVertical,

    /// <summary>
    /// Synchronize both vertical and horizontal scrolling.
    /// </summary>
    Both,

    /// <summary>
    /// Synchronize both directions with axes interchanged.
    /// </summary>
    BothInterchanged
  }
}
