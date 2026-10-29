#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Threading.Tasks;
using Altaxo.Chat;

namespace Altaxo
{
  internal class Program
  {
    private static async Task Main(string[] args)
    {
      string? logfileName = null;
      for (int i = 0; i < args.Length; i++)
      {
        var larg = args[i].ToLowerInvariant();

        if (larg == "--attachdebugger")
        {
          System.Diagnostics.Debugger.Launch();
        }
        else if (larg == "--logfile")
        {
          if ((i + 1) < args.Length)
          {
            logfileName = args[i + 1];
            i++;
          }
          else
          {
            Console.Error.WriteLine("Missing logfile name after --logfile argument.");
            // set the return code to 1 to indicate an error
            Environment.ExitCode = 1;
            return;
          }
          break;
        }
      }


      var server = new MCPServerProxy
      {
        LogfileName = logfileName
      };
      await server.RunAsync();
    }
  }
}
