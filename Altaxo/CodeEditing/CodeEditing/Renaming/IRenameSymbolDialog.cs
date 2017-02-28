using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.Renaming
{
	public interface IRenameSymbolDialog
	{
		bool ShouldRename { get; }
		string SymbolName { get; set; }

		void Initialize(string symbolName);

		void Show(object topLevelWindow);
	}
}