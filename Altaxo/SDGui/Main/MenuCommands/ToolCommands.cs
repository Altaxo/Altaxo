using System;
using System.Collections.Generic;
using System.Text;

using ICSharpCode.Core;

namespace Altaxo.Main.Commands
{
  public class AddTemporaryUserAssembly : AbstractMenuCommand
  {
    public override void Run()
    {
      Settings.Scripting.ReferencedAssembliesCommands.ShowAddTemporaryAssemblyDialog();
    }
  }
  
}
