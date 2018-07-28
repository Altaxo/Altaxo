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

namespace Altaxo.Gui.Startup
{
  /// <summary>
  /// This class contains properties you can use to control how the applicaiton is launched.
  /// </summary>
  public class StartupArguments // Important- do not derive here from another class or interface: we want to use this class in the earliest stages of startup!
  {
    public string ApplicationName { get; private set; }
    public string[] RequestedFileList { get; private set; } = new string[0];
    public string[] ParameterList { get; private set; } = new string[0];
    public string[] StartupArgs { get; private set; } = new string[0];

    public StartupArguments(StartupArguments from)
    {
      this.ApplicationName = from.ApplicationName;
      this.RequestedFileList = from.RequestedFileList;
      this.ParameterList = from.ParameterList;
      this.StartupArgs = from.StartupArgs;
    }

    public StartupArguments(string applicationName, string[] args)
    {
      if (string.IsNullOrEmpty(applicationName))
        throw new ArgumentNullException(nameof(applicationName));

      ApplicationName = applicationName;
      StartupArgs = args;

      var requestedFileList = new List<string>();
      var parameterList = new List<string>();
      parameterList.Clear();

      foreach (string arg in args)
      {
        if (arg.Length == 0)
          continue;
        if (arg[0] == '-' || arg[0] == '/')
        {
          int markerLength = 1;

          if (arg.Length >= 2 && arg[0] == '-' && arg[1] == '-')
          {
            markerLength = 2;
          }

          string param = arg.Substring(markerLength);
          // The AddIn project template uses /addindir:"c:\temp\"
          // but that actually means the last quote is escaped.
          // This HACK makes this work anyways by replacing the trailing quote
          // with a backslash:
          if (param.EndsWith("\"", StringComparison.Ordinal))
            param = param.Substring(0, param.Length - 1) + "\\";
          parameterList.Add(param);
        }
        else
        {
          requestedFileList.Add(arg);
        }
      }

      RequestedFileList = requestedFileList.ToArray();
      ParameterList = parameterList.ToArray();
    }
  }
}
