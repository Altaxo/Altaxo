using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
namespace Altaxo.Settings
{
	/// <summary>
	/// Manages the settings for a culture, i.e. number and DateTime formats etc.
	/// </summary>
	public class CultureSettings
	{
		/// <summary>Gets or sets a value indicating whether to override the parent culture or use it.</summary>
		/// <value> If <see langword="true"/>, the culture that is stored in this object is applied. Otherwise, the culture of the superior object (for instance the operating system) is used.</value>
		public bool OverrideParentCulture { get; set; }


		/// <summary>Gets or sets the culture info.</summary>
		/// <value>The culture info.</value>
		public System.Globalization.CultureInfo CultureInfo { get; set; }


		public CultureSettings()
		{
			CultureInfo = System.Globalization.CultureInfo.CurrentUICulture;
		}
	}
}
