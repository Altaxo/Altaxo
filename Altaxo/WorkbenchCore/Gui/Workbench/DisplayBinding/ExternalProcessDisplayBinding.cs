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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Display binding for opening a file in an external process.
  /// </summary>
  [TypeConverter(typeof(ExternalProcessDisplayBindingConverter))]
  public sealed class ExternalProcessDisplayBinding : IDisplayBinding
  {
    /// <summary>
    /// Gets or sets the file extension handled by the binding.
    /// </summary>
    public string? FileExtension { get; set; }
    /// <summary>
    /// Gets or sets the command line used to launch the external process.
    /// </summary>
    public string? CommandLine { get; set; }
    /// <summary>
    /// Gets or sets the display title.
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// Gets or sets the binding identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <inheritdoc/>
    public bool CanCreateContentForFile(FileName fileName)
    {
      return string.Equals(Path.GetExtension(fileName), FileExtension, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public IViewContent? CreateContentForFile(OpenedFile file)
    {
      if (file.IsDirty)
      {
        // TODO: warn user that the file must be saved
      }
      try
      {
        CommandLine ??= string.Empty;
        string cmd;
        if (CommandLine.Contains("%1"))
          cmd = CommandLine.Replace("%1", file.FileName);
        else
          cmd = CommandLine + " \"" + file.FileName + "\"";
        StartCommandLine(cmd, Path.GetDirectoryName(file.FileName));
      }
      catch (Exception ex)
      {
        MessageService.ShowError(ex.Message);
      }
      return null;
    }

    private static void StartCommandLine(string cmd, string? workingDir)
    {
      Current.Log.Debug("ExternalProcessDisplayBinding> " + cmd);
      cmd = cmd.Trim();
      if (cmd.Length == 0)
        return;
      var info = new ProcessStartInfo();
      if (cmd[0] == '"')
      {
        int pos = cmd.IndexOf('"', 1);
        info.FileName = cmd.Substring(1, pos - 1);
        info.Arguments = cmd.Substring(pos + 1).TrimStart();
      }
      else
      {
        int pos = cmd.IndexOf(' ', 0);
        info.FileName = cmd.Substring(0, pos);
        info.Arguments = cmd.Substring(pos + 1);
      }
      if (!string.IsNullOrEmpty(workingDir))
      {
        info.WorkingDirectory = workingDir;
      }

      Process.Start(info);
    }

    /// <inheritdoc/>
    public bool IsPreferredBindingForFile(FileName fileName)
    {
      return false;
    }

    /// <inheritdoc/>
    public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
    {
      return double.NegativeInfinity;
    }
  }

  /// <summary>
  /// Converts <see cref="ExternalProcessDisplayBinding"/> instances to and from their persisted string representation.
  /// </summary>
  internal sealed class ExternalProcessDisplayBindingConverter : TypeConverter
  {
    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      if (sourceType == typeof(string))
        return true;
      else
        return base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof(string))
        return true;
      else
        return base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc/>
    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        var binding = (ExternalProcessDisplayBinding)value;
        return binding.Id + "|" + binding.FileExtension + "|" + binding.Title + "|" + binding.CommandLine;
      }
      else
      {
        return base.ConvertTo(context, culture, value, destinationType);
      }
    }

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
      if (value is string valueString)
      {
        string[] values = valueString.Split('|');
        if (values.Length == 4)
        {
          return new ExternalProcessDisplayBinding
          {
            Id = values[0],
            FileExtension = values[1],
            Title = values[2],
            CommandLine = values[3]
          };
        }
        else
        {
          return null;
        }
      }
      else
      {
        return base.ConvertFrom(context, culture, value);
      }
    }
  }
}
