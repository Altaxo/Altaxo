using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
	public class LanguageDependentString
	{
		private string _originalString;
		private string _transformedString;

		public event EventHandler ValueChanged;

		public LanguageDependentString(string originalString)
		{
			_originalString = originalString ?? throw new ArgumentNullException(nameof(originalString));

			LanguageChangeWeakEventManager.LanguageChanged += EhLanguageChanged;
			_transformedString = StringParser.Parse(_originalString);
		}

		private void EhLanguageChanged()
		{
			_transformedString = StringParser.Parse(_originalString);
			ValueChanged.Invoke(this, EventArgs.Empty);
		}

		public string Value
		{
			get
			{
				return _transformedString;
			}
		}
	}
}