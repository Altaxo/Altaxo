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
using System.Linq;
using Altaxo;
using Altaxo.Main.Services;

namespace Altaxo.AddInItems
{
  /// <summary>
  /// Represents a node in the add in tree that can produce an item.
  /// </summary>
  public class Codon
  {
    private AddIn _addIn;
    private string _name;
    private Properties _properties;
    private IReadOnlyList<ICondition> _conditions;

    public string Name
    {
      get
      {
        return _name;
      }
    }

    public AddIn AddIn
    {
      get
      {
        return _addIn;
      }
    }

    public string Id
    {
      get
      {
        return _properties["id"];
      }
    }

    public string InsertAfter
    {
      get
      {
        return _properties["insertafter"];
      }
    }

    public string InsertBefore
    {
      get
      {
        return _properties["insertbefore"];
      }
    }

    public string this[string key]
    {
      get
      {
        return _properties[key];
      }
    }

    public Properties Properties
    {
      get
      {
        return _properties;
      }
    }

    public IReadOnlyList<ICondition> Conditions
    {
      get
      {
        return _conditions;
      }
    }

    public Codon(AddIn addIn, string name, Properties properties, IReadOnlyList<ICondition> conditions)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (properties == null)
        throw new ArgumentNullException("properties");
      this._addIn = addIn;
      this._name = name;
      this._properties = properties;
      this._conditions = conditions;
    }

    internal object? BuildItem(BuildItemArgs args)
    {
      if (!_addIn.AddInTree.Doozers.TryGetValue(Name, out var doozer))
        throw new BaseException("Doozer " + Name + " not found! " + ToString());

      if (!doozer.HandleConditions)
      {
        ConditionFailedAction action = Condition.GetFailedAction(args.Conditions, args.Parameter);
        if (action != ConditionFailedAction.Nothing)
        {
          return null;
        }
      }
      return doozer.BuildItem(args);
    }

    public override string ToString()
    {
      return string.Format("[Codon: name = {0}, id = {1}, addIn={2}]",
                           _name,
                           Id,
                           _addIn.FileName);
    }
  }
}
