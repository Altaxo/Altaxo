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
using Altaxo.AddInItems;
using Altaxo.Gui;
using Altaxo.Gui.Settings;

namespace Altaxo.Gui.Workbench
{
  public class DefaultOptionPanelDescriptor : IOptionPanelDescriptor
  {
    private string _id = string.Empty;
    private List<IOptionPanelDescriptor>? _optionPanelDescriptors = null;
    private IOptionPanel? _optionPanel = null;

    public string ID
    {
      get
      {
        return _id;
      }
    }

    public string Label { get; set; }

    public IEnumerable<IOptionPanelDescriptor>? ChildOptionPanelDescriptors
    {
      get
      {
        return _optionPanelDescriptors;
      }
    }

    private AddIn? _addin;
    private object? _owner;
    private string? _optionPanelPath;

    public IOptionPanel? OptionPanel
    {
      get
      {
        if (_optionPanelPath is not null)
        {
          if (_optionPanel is null)
          {
            _optionPanel = (IOptionPanel?)_addin?.CreateObject(_optionPanelPath);
            if (_optionPanel is not null)
            {
              _optionPanel.Initialize(_owner);
            }
          }
          _optionPanelPath = null;
          _addin = null;
        }
        return _optionPanel;
      }
    }

    public bool HasOptionPanel
    {
      get
      {
        return _optionPanelPath is not null;
      }
    }

    public DefaultOptionPanelDescriptor(string id, string label)
    {
      this._id = id;
      Label = label;
    }

    public DefaultOptionPanelDescriptor(string id, string label, List<IOptionPanelDescriptor> dialogPanelDescriptors)
      : this(id, label)
    {
      _optionPanelDescriptors = dialogPanelDescriptors;
    }

    public DefaultOptionPanelDescriptor(string id, string label, AddIn addin, object owner, string optionPanelPath)
      : this(id, label)
    {
      this._addin = addin;
      this._owner = owner;
      this._optionPanelPath = optionPanelPath;
    }
  }
}
