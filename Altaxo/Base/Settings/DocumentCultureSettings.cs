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
	public class DocumentCultureSettings : CultureSettingsBase, ICloneable
	{
		/// <summary>
		/// Storage path for storing this instance in the application properties.
		/// </summary>
		public static string SettingsStoragePath = "Altaxo.Options.DocumentCulture";

		static readonly string InvariantCultureThreeLetterISOLanguageName = CultureInfo.InvariantCulture.ThreeLetterISOLanguageName;

		/// <summary>
		/// Original UI culture, must be initialized before it can be used.
		/// </summary>
		static CultureInfo _originalCulture;

		/// <summary>Initializes this class with the original culture (i.e. the culture that the user has chosen in system control panel). This must be done before the current UI culture is changed by some routine.</summary>
		/// <param name="originalCulture">The original culture.</param>
		public static void InitializeOriginalCulture(CultureInfo originalCulture)
		{
			_originalCulture = (CultureInfo)originalCulture.Clone();
		}

		/// <summary>Initializes a new instance of the <see cref="DocumentCultureSettings"/> class with nothing initialized.</summary>
		private DocumentCultureSettings()
		{
		}

		public override CultureInfo OriginalCulture
		{
			get
			{
				if (null == _originalCulture)
					throw new InvalidOperationException("DocumentCultureSettings have not been initialized with the original culture. This must be done at the very beginning of the program. Please report this bug!");
				return _originalCulture;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="DocumentCultureSettings"/> class from the current UI culture.</summary>
		/// <returns>An instance of this class with the current UI culture set and the <see cref="P:Altaxo.Settings.CultureSettingsBase.OverrideParentCulture"/> flag set to false.</returns>
		public static DocumentCultureSettings FromDefault()
		{
			var result = new DocumentCultureSettings();
			result._overrideParentCulture = false;
			result.SetMembersFromCulture(result.OriginalCulture);
			return result;
		}

		/// <summary>Initializes a new instance of the <see cref="DocumentCultureSettings"/> class with a given culture.</summary>
		/// <param name="c">An instance of this class with the provided UI culture set and the <see cref="P:Altaxo.Settings.CultureSettingsBase.OverrideParentCulture"/> flag set to true.</param>
		/// <returns></returns>
		public static DocumentCultureSettings FromCulture(CultureInfo c)
		{
			var result = new DocumentCultureSettings();
			result._overrideParentCulture = true;
			result.SetMembersFromCulture(c);
			return result;
		}

		public override void SetMembersFromCulture(CultureInfo c)
		{
			base.SetMembersFromCulture(c);
			if (c.ThreeLetterISOLanguageName == InvariantCultureThreeLetterISOLanguageName)
				this._cultureName = c.ThreeLetterISOLanguageName;
		}

		/// <summary>Creates a new object that is a copy of the current instance.</summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public object Clone()
		{
			var result = new DocumentCultureSettings();
			result.CopyFrom(this);
			return result;
		}

		/// <summary>Assembles the members of this instance to a resulting culture.</summary>
		/// <returns>A culture info which contains the values of the member variables.</returns>
		public CultureInfo ToCulture()
		{
			CultureInfo result;
			if (!OverrideParentCulture)
			{
				result = OriginalCulture;
			}
			else
			{
				try
				{
					if (_cultureName == InvariantCultureThreeLetterISOLanguageName)
						result = (CultureInfo)CultureInfo.InvariantCulture.Clone();
					else
						result = new CultureInfo(CultureName);
				}
				catch (CultureNotFoundException)
				{
					result = (CultureInfo)CultureInfo.InvariantCulture.Clone();
				}
				result.NumberFormat.NumberDecimalSeparator = NumberDecimalSeparator;
				result.NumberFormat.NumberGroupSeparator = NumberGroupSeparator;
			}
			return result;
		}



		
	}
}
