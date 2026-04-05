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
using System.Xml;
using Altaxo;
using Altaxo.Main.Services;

namespace Altaxo.AddInItems
{
  /// <summary>
  /// Represents a condition attached to a codon in the add-in tree.
  /// </summary>
  public class Condition : ICondition
  {
    private string _name;
    private Properties _properties;
    private ConditionFailedAction _action;

    /// <summary>
    /// Gets the add-in that owns the condition.
    /// </summary>
    public AddIn AddIn { get; private set; }

    /// <summary>
    /// Returns the action which occurs, when this condition fails.
    /// </summary>
    public ConditionFailedAction Action
    {
      get
      {
        return _action;
      }
      set
      {
        _action = value;
      }
    }

    /// <summary>
    /// Gets the condition name.
    /// </summary>
    public string Name
    {
      get
      {
        return _name;
      }
    }

    /// <summary>
    /// Gets a property value by key.
    /// </summary>
    public string this[string key]
    {
      get
      {
        return _properties[key];
      }
    }

    /// <summary>
    /// Gets the condition properties.
    /// </summary>
    public Properties Properties
    {
      get
      {
        return _properties;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Condition"/> class.
    /// </summary>
    public Condition(string name, Properties properties, AddIn addIn)
    {
      AddIn = addIn;
      this._name = name;
      this._properties = properties;
      _action = properties.Get("action", ConditionFailedAction.Exclude);
    }

    /// <inheritdoc/>
    public bool IsValid(object? parameter)
    {
      try
      {
        var addInTree = Altaxo.Current.GetRequiredService<IAddInTree>();
        return addInTree.ConditionEvaluators[_name].IsValid(parameter, this);
      }
      catch (KeyNotFoundException)
      {
        throw new BaseException("Condition evaluator " + _name + " not found!");
      }
    }

    /// <summary>
    /// Reads a simple condition from XML.
    /// </summary>
    public static ICondition Read(XmlReader reader, AddIn addIn)
    {
      var properties = Properties.ReadFromAttributes(reader);
      string conditionName = properties["name"];
      return new Condition(conditionName, properties, addIn);
    }

    /// <summary>
    /// Reads a complex condition from XML.
    /// </summary>
    public static ICondition? ReadComplexCondition(XmlReader reader, AddIn addIn)
    {
      var properties = Properties.ReadFromAttributes(reader);
      reader.Read();
      ICondition? condition = null;
      while (reader.Read())
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.Element:
            switch (reader.LocalName)
            {
              case "And":
                condition = AndCondition.Read(reader, addIn);
                goto exit;
              case "Or":
                condition = OrCondition.Read(reader, addIn);
                goto exit;
              case "Not":
                condition = NegatedCondition.Read(reader, addIn);
                goto exit;
              default:
                throw new AddInLoadException("Invalid element name '" + reader.LocalName
                                             + "', the first entry in a ComplexCondition " +
                                             "must be <And>, <Or> or <Not>");
            }
        }
      }
exit:
      if (condition is not null)
      {
        ConditionFailedAction action = properties.Get("action", ConditionFailedAction.Exclude);
        condition.Action = action;
      }
      return condition;
    }

    /// <summary>
    /// Reads a list of conditions from XML.
    /// </summary>
    public static ICondition[] ReadConditionList(XmlReader reader, string endElement, AddIn addIn)
    {
      var conditions = new List<ICondition>();
      while (reader.Read())
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.EndElement:
            if (reader.LocalName == endElement)
            {
              return conditions.ToArray();
            }
            break;

          case XmlNodeType.Element:
            switch (reader.LocalName)
            {
              case "And":
                conditions.Add(AndCondition.Read(reader, addIn));
                break;

              case "Or":
                conditions.Add(OrCondition.Read(reader, addIn));
                break;

              case "Not":
                conditions.Add(NegatedCondition.Read(reader, addIn));
                break;

              case "Condition":
                conditions.Add(Condition.Read(reader, addIn));
                break;

              default:
                throw new AddInLoadException("Invalid element name '" + reader.LocalName
                                             + "', entries in a <" + endElement + "> " +
                                             "must be <And>, <Or>, <Not> or <Condition>");
            }
            break;
        }
      }
      return conditions.ToArray();
    }

    /// <summary>
    /// Gets the action to take when one of the conditions fails.
    /// </summary>
    public static ConditionFailedAction GetFailedAction(IEnumerable<ICondition> conditionList, object? parameter)
    {
      ConditionFailedAction action = ConditionFailedAction.Nothing;
      foreach (ICondition condition in conditionList)
      {
        if (!condition.IsValid(parameter))
        {
          if (condition.Action == ConditionFailedAction.Disable)
          {
            action = ConditionFailedAction.Disable;
          }
          else
          {
            return ConditionFailedAction.Exclude;
          }
        }
      }
      return action;
    }
  }
}
