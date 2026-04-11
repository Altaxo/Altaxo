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

    /// <summary>
    /// Gets the codon name.
    /// </summary>
    public string Name
    {
      get
      {
        return _name;
      }
    }

    /// <summary>
    /// Gets the add-in that owns the codon.
    /// </summary>
    public AddIn AddIn
    {
      get
      {
        return _addIn;
      }
    }

    /// <summary>
    /// Gets the codon identifier.
    /// </summary>
    public string Id
    {
      get
      {
        return _properties["id"];
      }
    }

    /// <summary>
    /// Gets the identifier of the codon that this codon should be inserted after.
    /// </summary>
    public string InsertAfter
    {
      get
      {
        return _properties["insertafter"];
      }
    }

    /// <summary>
    /// Gets the identifier of the codon that this codon should be inserted before.
    /// </summary>
    public string InsertBefore
    {
      get
      {
        return _properties["insertbefore"];
      }
    }

    /// <summary>
    /// Gets a property value by key.
    /// </summary>
    /// <param name="key">The property name.</param>
    public string this[string key]
    {
      get
      {
        return _properties[key];
      }
    }

    /// <summary>
    /// Gets the codon properties.
    /// </summary>
    public Properties Properties
    {
      get
      {
        return _properties;
      }
    }

    /// <summary>
    /// Gets the conditions attached to the codon.
    /// </summary>
    public IReadOnlyList<ICondition> Conditions
    {
      get
      {
        return _conditions;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Codon"/> class.
    /// </summary>
    /// <param name="addIn">The add-in that owns the codon.</param>
    /// <param name="name">The codon name.</param>
    /// <param name="properties">The codon properties.</param>
    /// <param name="conditions">The conditions attached to the codon.</param>
    public Codon(AddIn addIn, string name, Properties properties, IReadOnlyList<ICondition> conditions)
    {
      if (name is null)
        throw new ArgumentNullException("name");
      if (properties is null)
        throw new ArgumentNullException("properties");
      this._addIn = addIn;
      this._name = name;
      this._properties = properties;
      this._conditions = conditions;
    }

    /// <summary>
    /// Builds the item represented by this codon.
    /// </summary>
    /// <param name="args">The arguments used for the build operation.</param>
    /// <returns>The built item, or <c>null</c> if no item is created.</returns>
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

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format("[Codon: name = {0}, id = {1}, addIn={2}]",
                           _name,
                           Id,
                           _addIn.FileName);
    }
  }
}
