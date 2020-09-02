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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Altaxo.Main.Services;


namespace Altaxo.AddInItems
{
  public class Runtime
  {
    private string? _hintPath;
    private string _assembly;
    private Assembly? _loadedAssembly = null;

    private List<LazyLoadDoozer> _definedDoozers = new List<LazyLoadDoozer>();
    private List<LazyConditionEvaluator> _definedConditionEvaluators = new List<LazyConditionEvaluator>();
    private ICondition[]? _conditions;
    private IAddInTree _addInTree;
    private bool _isActive = true;
    private bool _isAssemblyLoaded;
    private readonly object _lockObj = new object(); // used to protect mutable parts of runtime

    public bool IsActive
    {
      get
      {
        lock (_lockObj)
        {
          if (_conditions is not null)
          {
            _isActive = Condition.GetFailedAction(_conditions, this) == ConditionFailedAction.Nothing;
            _conditions = null;
          }
          return _isActive;
        }
      }
    }

    public Runtime(IAddInTree addInTree, string assembly, string? hintPath)
    {
      if (addInTree is null)
        throw new ArgumentNullException(nameof(addInTree));
      if (assembly is null)
        throw new ArgumentNullException(nameof(assembly));
      _addInTree = addInTree;
      _assembly = assembly;
      _hintPath = hintPath;
    }

    public string Assembly
    {
      get { return _assembly; }
    }

    /// <summary>
    /// Gets whether the assembly belongs to the host application (':' prefix).
    /// </summary>
    public bool IsHostApplicationAssembly
    {
      get { return !string.IsNullOrEmpty(_assembly) && _assembly[0] == ':'; }
    }

    /// <summary>
    /// Force loading the runtime assembly now.
    /// </summary>
    public void Load()
    {
      lock (_lockObj)
      {
        if (!_isAssemblyLoaded)
        {
          if (!IsActive)
            throw new InvalidOperationException("Cannot load inactive AddIn runtime");

          _isAssemblyLoaded = true;

          try
          {
            if (_assembly[0] == ':')
            {
              _loadedAssembly = LoadAssembly(_assembly.Substring(1));
            }
            else if (_assembly[0] == '$')
            {
              int pos = _assembly.IndexOf('/');
              if (pos < 0)
                throw new BaseException("Expected '/' in path beginning with '$'!");
              string referencedAddIn = _assembly.Substring(1, pos - 1);
              foreach (var addIn in _addInTree.AddIns)
              {
                if (addIn.Enabled && addIn.Manifest.Identities.ContainsKey(referencedAddIn))
                {
                  string assemblyFile = Path.Combine(Path.GetDirectoryName(addIn.FileName) ?? string.Empty,
                                                     _assembly.Substring(pos + 1));
                  _loadedAssembly = LoadAssemblyFrom(assemblyFile);
                  break;
                }
              }
              if (_loadedAssembly is null)
              {
                throw new FileNotFoundException("Could not find referenced AddIn " + referencedAddIn);
              }
            }
            else
            {
              _loadedAssembly = LoadAssemblyFrom(Path.Combine(_hintPath ?? string.Empty, _assembly));
            }

            // register all resources that are directly included into the assembly
            Current.GetRequiredService<IResourceService>().RegisterAssemblyResources(_loadedAssembly);

#if DEBUG
            // preload assembly to provoke FileLoadException if dependencies are missing
            _loadedAssembly.GetExportedTypes();
#endif
          }
          catch (FileNotFoundException ex)
          {
            ShowError("The addin '" + _assembly + "' could not be loaded:\n" + ex.ToString());
          }
          catch (FileLoadException ex)
          {
            ShowError("The addin '" + _assembly + "' could not be loaded:\n" + ex.ToString());
          }
        }
      }
    }

    public Assembly? LoadedAssembly
    {
      get
      {
        if (IsActive)
        {
          Load(); // load the assembly, if not already done
          return _loadedAssembly;
        }
        else
        {
          return null;
        }
      }
    }

    public IEnumerable<KeyValuePair<string, IDoozer>> DefinedDoozers
    {
      get
      {
        return _definedDoozers.Select(d => new KeyValuePair<string, IDoozer>(d.Name, d));
      }
    }

    public IEnumerable<KeyValuePair<string, IConditionEvaluator>> DefinedConditionEvaluators
    {
      get
      {
        return _definedConditionEvaluators.Select(c => new KeyValuePair<string, IConditionEvaluator>(c.Name, c));
      }
    }

    public Type? FindType(string className)
    {
      return LoadedAssembly is { } asm ? asm.GetType(className) : null;
    }

    internal static List<Runtime> ReadSection(XmlReader reader, AddIn addIn, string? hintPath)
    {
      var runtimes = new List<Runtime>();
      var conditionStack = new Stack<ICondition>();
      while (reader.Read())
      {
        switch (reader.NodeType)
        {
          case XmlNodeType.EndElement:
            if (reader.LocalName == "Condition" || reader.LocalName == "ComplexCondition")
            {
              conditionStack.Pop();
            }
            else if (reader.LocalName == "Runtime")
            {
              return runtimes;
            }
            break;

          case XmlNodeType.Element:
            switch (reader.LocalName)
            {
              case "Condition":
                conditionStack.Push(Condition.Read(reader, addIn));
                break;

              case "ComplexCondition":
                var cc = Condition.ReadComplexCondition(reader, addIn);
                if (!(cc is null))
                  conditionStack.Push(cc);
                break;

              case "Import":
                runtimes.Add(Runtime.Read(addIn, reader, hintPath, conditionStack));
                break;

              case "DisableAddIn":
                if (Condition.GetFailedAction(conditionStack, addIn) == ConditionFailedAction.Nothing)
                {
                  // The DisableAddIn node not was not disabled by a condition
                  addIn.CustomErrorMessage = reader.GetAttribute("message");
                }
                break;

              default:
                throw new AddInLoadException("Unknown node in runtime section :" + reader.LocalName);
            }
            break;
        }
      }
      return runtimes;
    }

    internal static Runtime Read(AddIn addIn, XmlReader reader, string? hintPath, Stack<ICondition> conditionStack)
    {
      if (reader.AttributeCount != 1)
      {
        throw new AddInLoadException("Import node requires ONE attribute.");
      }
      var runtime = new Runtime(addIn.AddInTree, reader.GetAttribute(0), hintPath);
      if (conditionStack.Count > 0)
      {
        runtime._conditions = conditionStack.ToArray();
      }
      if (!reader.IsEmptyElement)
      {
        while (reader.Read())
        {
          switch (reader.NodeType)
          {
            case XmlNodeType.EndElement:
              if (reader.LocalName == "Import")
              {
                return runtime;
              }
              break;

            case XmlNodeType.Element:
              string nodeName = reader.LocalName;
              var properties = Properties.ReadFromAttributes(reader);
              switch (nodeName)
              {
                case "Doozer":
                  if (!reader.IsEmptyElement)
                  {
                    throw new AddInLoadException("Doozer nodes must be empty!");
                  }
                  runtime._definedDoozers.Add(new LazyLoadDoozer(addIn, properties));
                  break;

                case "ConditionEvaluator":
                  if (!reader.IsEmptyElement)
                  {
                    throw new AddInLoadException("ConditionEvaluator nodes must be empty!");
                  }
                  runtime._definedConditionEvaluators.Add(new LazyConditionEvaluator(addIn, properties));
                  break;

                default:
                  throw new AddInLoadException("Unknown node in Import section:" + nodeName);
              }
              break;
          }
        }
      }
      return runtime;
    }

    protected virtual Assembly LoadAssembly(string assemblyString)
    {
#if NETFRAMEWORK
      var assembly = System.Reflection.Assembly.Load(assemblyString);
#else
      var assembly = AssemblyLoaderService.Instance.LoadAssemblyFromPartialName(assemblyString, this._hintPath ?? string.Empty);
#endif

#if VerboseInfo_AssemblyLoading
      System.Diagnostics.Debug.WriteLine($"AssemblyLoader called with assemblyString={assemblyString}, result is {assembly.Location}");
#endif

      if (assembly is null)
        throw new ApplicationException($"Can not load assembly {assemblyString}!");

      return assembly;
    }

    protected virtual Assembly LoadAssemblyFrom(string assemblyFile)
    {
      var assembly = AssemblyLoaderService.Instance.LoadAssemblyFromFullySpecifiedName(assemblyFile);

#if VerboseInfo_AssemblyLoading
      System.Diagnostics.Debug.WriteLine($"Attention: {nameof(LoadAssemblyFrom)} called with assemblyFile={assemblyFile}, result is {assembly.Location}");
#endif
      return assembly;

    }

    protected virtual void ShowError(string message)
    {
      Altaxo.Current.GetRequiredService<IMessageService>().ShowError(message);
    }
  }
}
