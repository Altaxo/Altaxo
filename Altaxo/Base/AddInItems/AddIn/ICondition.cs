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

namespace Altaxo.AddInItems
{
  /// <summary>
  /// Default actions when a condition fails.
  /// </summary>
  public enum ConditionFailedAction
  {
    /// <summary>
    /// Performs no action.
    /// </summary>
    Nothing,
    /// <summary>
    /// Excludes the affected item.
    /// </summary>
    Exclude,
    /// <summary>
    /// Disables the affected item.
    /// </summary>
    Disable
  }

  /// <summary>
  /// Represents a single condition or a complex condition.
  /// </summary>
  public interface ICondition
  {
    /// <summary>
    /// Gets the condition name.
    /// </summary>
    string Name
    {
      get;
    }

    /// <summary>
    /// Gets or sets the action that occurs when this condition fails.
    /// </summary>
    ConditionFailedAction Action
    {
      get;
      set;
    }

    /// <summary>
    /// Determines whether the condition is valid for the specified parameter.
    /// </summary>
    /// <param name="parameter">The parameter to validate against the condition.</param>
    /// <returns><see langword="true"/> if the condition is valid; otherwise, <see langword="false"/>.</returns>
    bool IsValid(object? parameter);
  }
}
