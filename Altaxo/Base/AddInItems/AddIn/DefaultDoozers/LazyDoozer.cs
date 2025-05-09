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

#nullable enable
using System;
using Altaxo.Main.Services;

namespace Altaxo.AddInItems
{
  /// <summary>
  /// This doozer lazy-loads another doozer when it has to build an item.
  /// It is used internally to wrap doozers specified in addins.
  /// </summary>
  internal sealed class LazyLoadDoozer : IDoozer
  {
    private AddIn _addIn;
    private string _name;
    private string _className;

    public string Name
    {
      get
      {
        return _name;
      }
    }

    public LazyLoadDoozer(AddIn addIn, Properties properties)
    {
      this._addIn = addIn;
      _name = properties["name"];
      _className = properties["class"];
    }

    /// <summary>
    /// Gets if the doozer handles codon conditions on its own.
    /// If this property return false, the item is excluded when the condition is not met.
    /// </summary>
    public bool HandleConditions
    {
      get
      {
        var doozer = (IDoozer?)_addIn.CreateObject(_className);
        if (doozer is null)
        {
          return false;
        }
        _addIn.AddInTree.Doozers[_name] = doozer;
        return doozer.HandleConditions;
      }
    }

    public object? BuildItem(BuildItemArgs args)
    {
      var doozer = (IDoozer?)_addIn.CreateObject(_className);
      if (doozer is null)
      {
        return null;
      }
      _addIn.AddInTree.Doozers[_name] = doozer;
      return doozer.BuildItem(args);
    }

    public override string ToString()
    {
      return string.Format("[LazyLoadDoozer: className = {0}, name = {1}]",
                           _className,
                           _name);
    }
  }
}
