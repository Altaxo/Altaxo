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
using System.Xml;
using Altaxo.Main.Services;

namespace Altaxo.AddInItems
{
  /// <summary>
  /// Represents all contributions to a Path in a single .addin file.
  /// </summary>
  public class ExtensionPath
  {
    private string _name;
    private AddIn _addIn;
    private List<List<Codon>> _codons = new List<List<Codon>>();

    public AddIn AddIn
    {
      get
      {
        return _addIn;
      }
    }

    public string Name
    {
      get
      {
        return _name;
      }
    }

    public IEnumerable<Codon> Codons
    {
      get
      {
        return
          from list in _codons
          from c in list
          select c;
      }
    }

    /// <summary>
    /// Gets the codons separated by the groups they were created in.
    /// i.e. if two addins add the codons to the same path they will be in diffrent group.
    /// if the same addin adds the codon in diffrent path elements they will be in diffrent groups.
    /// </summary>
    public IEnumerable<IEnumerable<Codon>> GroupedCodons
    {
      get
      {
        return _codons.AsReadOnly();
      }
    }

    public ExtensionPath(string name, AddIn addIn)
    {
      this._addIn = addIn;
      this._name = name;
    }

    public static void SetUp(ExtensionPath extensionPath, XmlReader reader, string endElement)
    {
      extensionPath.DoSetUp(reader, endElement, extensionPath._addIn);
    }

    private void DoSetUp(XmlReader reader, string endElement, AddIn addIn)
    {
      var conditionStack = new Stack<ICondition>();
      var innerCodons = new List<Codon>();
      while (reader.Read())
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.EndElement:
            if (reader.LocalName == "Condition" || reader.LocalName == "ComplexCondition")
            {
              conditionStack.Pop();
            }
            else if (reader.LocalName == endElement)
            {
              if (innerCodons.Count > 0)
                _codons.Add(innerCodons);
              return;
            }
            break;

          case XmlNodeType.Element:
            string elementName = reader.LocalName;
            if (elementName == "Condition")
            {
              conditionStack.Push(Condition.Read(reader, addIn));
            }
            else if (elementName == "ComplexCondition")
            {
              var cc = Condition.ReadComplexCondition(reader, addIn);
              if (!(cc is null))
                conditionStack.Push(cc);
            }
            else
            {
              var newCodon = new Codon(AddIn, elementName, Properties.ReadFromAttributes(reader), conditionStack.ToArray());
              innerCodons.Add(newCodon);
              if (!reader.IsEmptyElement)
              {
                ExtensionPath subPath = AddIn.GetExtensionPath(Name + "/" + newCodon.Id);
                subPath.DoSetUp(reader, elementName, addIn);
              }
            }
            break;
        }
      }
      if (innerCodons.Count > 0)
        _codons.Add(innerCodons);
    }
  }
}
