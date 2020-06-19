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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Text;
using Altaxo.AddInItems;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// This class parses internal ${xyz} tags of #Develop.
  /// All environment variables are avaible under the name env.[NAME]
  /// where [NAME] represents the string under which it is avaiable in
  /// the environment.
  /// </summary>
  public static class StringParser
  {
    private static readonly ConcurrentDictionary<string, IStringTagProvider> prefixedStringTagProviders
        = InitializePrefixedStringTagProviders();

    // not really a stack - we only use Add and GetEnumerator
    private static readonly ConcurrentStack<IStringTagProvider> stringTagProviders = new ConcurrentStack<IStringTagProvider>();

    private static ConcurrentDictionary<string, IStringTagProvider> InitializePrefixedStringTagProviders()
    {
      var dict = new ConcurrentDictionary<string, IStringTagProvider>(StringComparer.OrdinalIgnoreCase);

      // entryAssembly == null might happen in unit test mode
      var entryAssembly = Assembly.GetEntryAssembly();
      if (entryAssembly != null)
      {
        string exeName = entryAssembly.Location;
        dict["exe"] = new PropertyObjectTagProvider(FileVersionInfo.GetVersionInfo(exeName));
      }

      return dict;
    }

    /// <summary>
    /// Escapes all occurrences of '${' to '${$}{'.
    /// </summary>
    public static string Escape(string input)
    {
      if (input is null)
        throw new ArgumentNullException("input");
      return input.Replace("${", "${$}{");
    }

    /// <summary>
    /// Expands ${xyz} style property values.
    /// </summary>
    [return: NotNullIfNotNull("input")]
    public static string? Parse(string? input)
    {
      return Parse(input, null);
    }

    public static void RegisterStringTagProvider(IStringTagProvider tagProvider)
    {
      if (tagProvider == null)
        throw new ArgumentNullException("tagProvider");
      stringTagProviders.Push(tagProvider);
    }

    public static void RegisterStringTagProvider(string prefix, IStringTagProvider tagProvider)
    {
      if (prefix is null)
        throw new ArgumentNullException(nameof(prefix));
      if (tagProvider is null)
        throw new ArgumentNullException(nameof(tagProvider));
      prefixedStringTagProviders[prefix] = tagProvider;
    }

    //readonly static Regex pattern = new Regex(@"\$\{([^\}]*)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Expands ${xyz} style property values.
    /// </summary>
    [return: NotNullIfNotNull("input")]
    public static string? Parse(string? input, params StringTagPair[]? customTags)
    {
      if (input is null)
        return null;
      int pos = 0;
      StringBuilder? output = null; // don't use StringBuilder if input is a single property
      do
      {
        int oldPos = pos;
        pos = input.IndexOf("${", pos, StringComparison.Ordinal);
        if (pos < 0)
        {
          if (output is null)
          {
            return input;
          }
          else
          {
            if (oldPos < input.Length)
            {
              // normal text after last property
              output.Append(input, oldPos, input.Length - oldPos);
            }
            return output.ToString();
          }
        }
        if (output is null)
        {
          if (pos == 0)
            output = new StringBuilder();
          else
            output = new StringBuilder(input, 0, pos, pos + 16);
        }
        else
        {
          if (pos > oldPos)
          {
            // normal text between two properties
            output.Append(input, oldPos, pos - oldPos);
          }
        }
        int end = input.IndexOf('}', pos + 1);
        if (end < 0)
        {
          output.Append("${");
          pos += 2;
        }
        else
        {
          string property = input.Substring(pos + 2, end - pos - 2);
          string? val = GetValue(property, customTags);
          if (val is null)
          {
            output.Append("${");
            output.Append(property);
            output.Append('}');
          }
          else
          {
            output.Append(val);
          }
          pos = end + 1;
        }
      } while (pos < input.Length);
      return output.ToString();
    }

    /// <summary>
    /// Evaluates a property using the StringParser. Equivalent to StringParser.Parse("${" + propertyName + "}");
    /// </summary>
    public static string? GetValue(string propertyName, params StringTagPair[]? customTags)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));
      if (propertyName == "$")
        return "$";

      if (customTags != null)
      {
        foreach (StringTagPair pair in customTags)
        {
          if (propertyName.Equals(pair.Tag, StringComparison.OrdinalIgnoreCase))
          {
            return pair.Value;
          }
        }
      }

      int k = propertyName.IndexOf(':');
      if (k <= 0)
      {
        // it's property without prefix

        if (propertyName.Equals("DATE", StringComparison.OrdinalIgnoreCase))
          return DateTime.Today.ToShortDateString();
        if (propertyName.Equals("TIME", StringComparison.OrdinalIgnoreCase))
          return DateTime.Now.ToShortTimeString();
        if (propertyName.Equals("ProductName", StringComparison.OrdinalIgnoreCase))
          return Altaxo.Current.GetRequiredService<IMessageService>().ProductName;
        if (propertyName.Equals("GUID", StringComparison.OrdinalIgnoreCase))
          return Guid.NewGuid().ToString().ToUpperInvariant();
        if (propertyName.Equals("USER", StringComparison.OrdinalIgnoreCase))
          return Environment.UserName;
        if (propertyName.Equals("Version", StringComparison.OrdinalIgnoreCase))
          return RevisionClass.FullVersion;
        if (propertyName.Equals("CONFIGDIRECTORY", StringComparison.OrdinalIgnoreCase))
          return Altaxo.Current.GetRequiredService<IPropertyService>().ConfigDirectory;

        foreach (IStringTagProvider provider in stringTagProviders)
        {
          var result = provider.ProvideString(propertyName, customTags);
          if (result != null)
            return result;
        }

        return null;
      }
      else
      {
        // it's a prefixed property

        // res: properties are quite common, so optimize by testing for them first
        // before allocaing the prefix/propertyName strings
        // All other prefixed properties {prefix:Key} shoulg get handled in the switch below.
        if (propertyName.StartsWith("res:", StringComparison.OrdinalIgnoreCase))
        {
          var resourceService = Current.ResourceService;
          if (resourceService == null)
            return null;
          try
          {
            return Parse(resourceService.GetString(propertyName.Substring(4)), customTags);
          }
          catch (ResourceNotFoundException)
          {
            return null;
          }
        }

        string prefix = propertyName.Substring(0, k);
        propertyName = propertyName.Substring(k + 1);
        switch (prefix.ToUpperInvariant())
        {
          case "ADDINPATH":
            foreach (var addIn in Altaxo.Current.GetRequiredService<IAddInTree>().AddIns)
            {
              if (addIn.Manifest.Identities.ContainsKey(propertyName))
              {
                return System.IO.Path.GetDirectoryName(addIn.FileName);
              }
            }
            return null;

          case "DATE":
            try
            {
              return DateTime.Now.ToString(propertyName, CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
              return ex.Message;
            }
          case "ENV":
            return Environment.GetEnvironmentVariable(propertyName);

          case "PROPERTY":
            return GetProperty(propertyName);

          default:

            if (prefixedStringTagProviders.TryGetValue(prefix, out var provider))
              return provider.ProvideString(propertyName, customTags);
            else
              return null;
        }
      }
    }

    /// <summary>
    /// Applies the StringParser to the formatstring; and then calls <c>string.Format</c> on the result.
    ///
    /// This method is equivalent to:
    /// <code>return string.Format(StringParser.Parse(formatstring), formatitems);</code>
    /// but additionally includes error handling.
    /// </summary>
    public static string Format(string formatstring, params object[] formatitems)
    {
      try
      {
        var fmt = StringParser.Parse(formatstring);
        return string.Format(fmt, formatitems);
      }
      catch (FormatException ex)
      {
        Current.Log.Warn(ex);

        var b = new StringBuilder(StringParser.Parse(formatstring));
        foreach (object formatitem in formatitems)
        {
          b.Append("\nItem: ");
          b.Append(formatitem);
        }
        return b.ToString();
      }
    }

    /// <summary>
    /// Allow special syntax to retrieve property values:
    /// ${property:PropertyName}
    /// ${property:PropertyName??DefaultValue}
    /// ${property:ContainerName/PropertyName}
    /// ${property:ContainerName/PropertyName??DefaultValue}
    /// A container is a Properties instance stored in the PropertyService. This is
    /// used by many AddIns to group all their properties into one container.
    /// </summary>
    private static string GetProperty(string propertyName)
    {
      return string.Empty;
    }
  }

  public struct StringTagPair
  {
    private readonly string _tag;
    private readonly string _value;

    public string Tag
    {
      get { return _tag; }
    }

    public string Value
    {
      get { return _value; }
    }

    public StringTagPair(string tag, string value)
    {
      this._tag = tag ?? throw new ArgumentNullException(nameof(tag));
      this._value = value ?? throw new ArgumentNullException(nameof(value));
    }
  }
}
