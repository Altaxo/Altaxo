using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Main.Services.ScriptCompilation
{
  public interface IScriptCompilerResult
  {
    int ScriptTextCount { get; }

    string ScriptText(int i);

    string ScriptTextHash { get; }
  }
}
